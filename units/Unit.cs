using Godot;

namespace CoopDefense.Units
{
    public partial class Unit : CharacterBody2D // 1. 改为 CharacterBody2D 才能有物理碰撞
    {
        [Export] public int OwnerPlayerId { get; set; } = 1;
        [Export] public float MaxHealth { get; set; } = 100f;
        [Export] public float AttackDamage { get; set; } = 10f;
        [Export] public float AttackRange { get; set; } = 200f;
        [Export] public float AttackInterval { get; set; } = 1f;

        private float _currentHealth;
        private float _attackTimer = 0f;

        public override void _Ready()
        {
            _currentHealth = MaxHealth;
            AddToGroup("DefenseUnits"); // 方便敌人索敌
            
            // 2. 动态加一个碰撞框，这样怪撞上来才会停下
            var collision = new CollisionShape2D();
            collision.Shape = new RectangleShape2D { Size = new Vector2(40, 40) };
            AddChild(collision);
        }

        public override void _Process(double delta)
        {
            _attackTimer -= (float)delta;

            if (_attackTimer <= 0)
            {
                AttackNearestEnemy();
                _attackTimer = AttackInterval;
            }
        }

        // 3. 新增：被怪物攻击时扣血
        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            GD.Print($"炮塔受损！剩余血量: {_currentHealth}");
            if (_currentHealth <= 0)
            {
                GD.Print("炮塔被摧毁！");
                QueueFree(); // 销毁自己，给怪让路
            }
        }

        private void AttackNearestEnemy()
        {
            var enemies = GetTree().GetNodesInGroup("Enemies");
            Node2D nearestEnemy = null;
            float minDistance = float.MaxValue;

            foreach (Node node in enemies)
            {
                if (node is Node2D enemy)
                {
                    float dist = Position.DistanceTo(enemy.Position);
                    if (dist < AttackRange && dist < minDistance)
                    {
                        minDistance = dist;
                        nearestEnemy = enemy;
                    }
                }
            }

            if (nearestEnemy != null)
            {
                GD.Print($"炮塔开火！攻击距离: {minDistance:F1}");        
                if (nearestEnemy is CoopDefense.Combat.Enemy enemyScript)
                {
                    enemyScript.TakeDamage(AttackDamage);
                }
            }
        }
    }
}