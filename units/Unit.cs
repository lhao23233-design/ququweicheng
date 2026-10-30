using Godot;

namespace CoopDefense.Units
{
    public partial class Unit : Node2D
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

        private void AttackNearestEnemy()
        {
            // 找场景里的所有敌人
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
                GD.Print($"炮塔开火！攻击敌人 {nearestEnemy.Name}");
                // 这里暂时不写扣血逻辑，先证明能索敌
            }
        }
    }
}