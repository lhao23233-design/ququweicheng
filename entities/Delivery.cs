using Godot;
using CoopDefense.Core;
using CoopDefense.Logistics; // 引入网格系统命名空间
using System.Collections.Generic;

namespace CoopDefense.Entities
{
    public partial class Delivery : CharacterBody2D
    {   
        private GameLoop _gameLoop;
        private GridSystem _gridSystem;
        private float Health = Config.Health;
        private List<Vector2I> _currentPath;
        private int _pathIndex = 0;
        private Vector2I _lastGridPos = new Vector2I(-1, -1);//记录上一次
        private float _spawnFreezeTimer = 0.5f; // 出生后先冻结半秒
        private static readonly Vector2I DestinationGrid = new(15, GridSystem.GridHeight - 1);


        public override void _Ready()
        {
            AddToGroup("Deliveries");
            this.CollisionLayer = 1 << 2; // 代表我是“运输单位”层
            this.CollisionMask = 1 << 0 ; // 代表我可以与“敌人”发生碰撞
            // 动态加碰撞框
            var collision = new CollisionShape2D();
            collision.Shape = new RectangleShape2D { Size = new Vector2(Config.NpcCollisionSize, Config.NpcCollisionSize) };
            AddChild(collision);

            _gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
            _gridSystem = GetTree().Root.FindChild("GridSystem", true, false) as GridSystem;
        }
        public override void _PhysicsProcess(double delta)
        {
            if (_gridSystem == null) return;
            if (_spawnFreezeTimer > 0)
            {
                _spawnFreezeTimer -= (float)delta;
                Velocity = Vector2.Zero; // 绝对不动
                MoveAndSlide();
                return; // 冻结期间直接跳过后续所有寻路和移动
            }
            // 获取当前脚下的网格
            Vector2I currentGrid = _gridSystem.WorldToGrid(GlobalPosition);
            Vector2 currentCellCenter = _gridSystem.GridToWorld(currentGrid);
            bool isCenteredOnCurrentCell = GlobalPosition.DistanceTo(currentCellCenter) <= 1f;
            float currentSpeed = Velocity.Length();

            if (currentGrid == DestinationGrid &&
                GlobalPosition.DistanceTo(currentCellCenter) <= currentSpeed * (float)delta + 1f)
            {
                GlobalPosition = currentCellCenter;
                ReachDestination();
                return;
            }

            // Replan on cell changes only after reaching that cell's center. Otherwise
            // the next waypoint can skip the center of the cell just entered.
            if (_currentPath == null || _pathIndex >= _currentPath.Count ||
                (currentGrid != _lastGridPos && isCenteredOnCurrentCell))
            {
                _lastGridPos = currentGrid; // 更新足迹
                var blocked = new HashSet<Vector2I>(); 
                
                _currentPath = Systems.Pathfinder.FindFastestPath(
                    currentGrid, 
                    DestinationGrid, 
                    _gridSystem.RoadCells, 
                    blocked, 
                    _gridSystem.CellCosts
                );
                _pathIndex = 0;
            }

            // 3. 如果算出来没路（说明玩家乱铺或者还没铺），就减速往死里走
            if (_currentPath == null || _currentPath.Count == 0)
            {
                Vector2 fallbackTarget = GlobalPosition + Vector2.Down;
                Velocity = SmoothMovement.GetVelocity(
                    GlobalPosition,
                    fallbackTarget,
                    Config.MoveSpeed * Config.NpcOffRoadSpeedMultiplier,
                    Config.NpcAcceleration,
                    currentSpeed,
                    (float)delta);
                MoveAndSlide();
                return;
            }

            Vector2I targetGrid = _currentPath[_pathIndex];
            Vector2 targetPos = _gridSystem.GridToWorld(targetGrid);
            float targetSpeed = _gridSystem.RoadCells.Contains(targetGrid)
                ? Config.MoveSpeed
                : Config.MoveSpeed * Config.NpcOffRoadSpeedMultiplier;

            // 计算当前到目标中心的距离
            float distToTarget = GlobalPosition.DistanceTo(targetPos);
            Velocity = SmoothMovement.GetVelocity(
                GlobalPosition,
                targetPos,
                targetSpeed,
                Config.NpcAcceleration,
                currentSpeed,
                (float)delta);

            // 核心修复：如果下一帧的步长会跨越目标点，就直接吸附到中心！
            if (distToTarget <= Velocity.Length() * (float)delta + 1f)
            {
                GlobalPosition = targetPos; // 强行精准吸附到网格正中心
                _pathIndex++;
                return; // 这一帧不进行物理移动，下一帧朝新目标走
            }
            MoveAndSlide();
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            GD.Print($"运钞车被攻击！剩余血量: {Health}");
            
            if (Health <= 0)
            {
                GD.Print("运钞车被摧毁！资源丢失。");
                QueueFree(); 
            }
        }

        private void ReachDestination()
        {
            GD.Print("运钞车安全抵达传输口！");
            if (_gameLoop != null)
            {
                _gameLoop.State.Player1.Coins += 30;
            }
            QueueFree();
        }
    }
}