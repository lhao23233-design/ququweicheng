using Godot;
using CoopDefense.Core;

namespace CoopDefense.Units
{
    public partial class Delivery : CharacterBody2D
    {
        public float Health = 30f; 
        public float MoveSpeed = 100f; 
        private GameLoop _gameLoop;
        public override void _Ready()
        {
            AddToGroup("Deliveries");
            
            // 动态加碰撞框
            var collision = new CollisionShape2D();
            collision.Shape = new RectangleShape2D { Size = new Vector2(Config.NpcCollisionSize, Config.NpcCollisionSize) };
            AddChild(collision);

            _gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
        }

        public override void _PhysicsProcess(double delta)
        {
            // 向下移动
            Velocity = new Vector2(0, MoveSpeed); 
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
                _gameLoop.State.Player1.Coins += 30; // 假设收益 30
            }
            QueueFree();
        }
    }
}