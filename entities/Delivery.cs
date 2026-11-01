using Godot;
using CoopDefense.Core;
using CoopDefense.Logistics; // 引入网格系统命名空间

namespace CoopDefense.Entities
{
    public partial class Delivery : CharacterBody2D
    {
        public float Health = 30f; 
        public float MoveSpeed = 100f; 
        
        private GameLoop _gameLoop;
        private GridSystem _gridSystem;

        public override void _Ready()
        {
            AddToGroup("Deliveries");
            
            // 动态加碰撞框
            var collision = new CollisionShape2D();
            collision.Shape = new RectangleShape2D { Size = new Vector2(Config.NpcCollisionSize, Config.NpcCollisionSize) };
            AddChild(collision);

            _gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
            _gridSystem = GetTree().Root.FindChild("GridSystem", true, false) as GridSystem;
        }

        public override void _PhysicsProcess(double delta)
        {
            float currentSpeed = MoveSpeed;

            // 核心逻辑：如果不在路上，就减速
            if (_gridSystem != null)
            {
                // 取脚下稍微偏中心的位置检测（加上半个碰撞框的高度）
                Vector2 checkPos = Position + new Vector2(0, Config.NpcCollisionSize / 2);

                if (!_gridSystem.IsWorldPositionRoad(checkPos))
                {
                    // 没路的情况下，速度降低到 30%
                    currentSpeed = MoveSpeed * 0.3f; 
                }
            }

            // 应用实际速度
            Velocity = new Vector2(0, currentSpeed); 
            MoveAndSlide();

            // 到达底部传输口
            if (Position.Y >= Config.NpcEndY) 
            {
                ReachDestination();
            }
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