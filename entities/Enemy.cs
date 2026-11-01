using Godot;
using CoopDefense.Core;
using CoopDefense.Units;
using CoopDefense.Entities;

namespace CoopDefense.Combat
{
	public partial class Enemy : CharacterBody2D
	{
		public float Health = Config.EnemyMaxHealth;
		public float MoveSpeed = Config.EnemyMoveSpeed;

		private float _attackTimer;
		private readonly float _attackInterval = Config.EnemyAttackInterval;
		private readonly float _damage = Config.EnemyAttackDamage;
		private GameLoop _gameLoop;

		public override void _Ready()
		{
			AddToGroup("Enemies");
			AddCollisionShape();
			// --- 新增视觉块代码开始 ---
			var visual = new ColorRect
			{
				Color = Colors.Red,
				Size = new Vector2(Config.EnemyCollisionSize, Config.EnemyCollisionSize)
			};
			// 关键偏移：让视觉块的正中心，对齐物理碰撞箱的正中心
			visual.Position = new Vector2(-Config.EnemyCollisionSize / 2, -Config.EnemyCollisionSize / 2);
			AddChild(visual);
			// --- 新增视觉块代码结束 ---

			// Layer 1 (值就是 1)：代表我是“敌人”层
            this.CollisionLayer = 1<<0; 
			this.CollisionMask = 1<<1 | 1<<2; // 代表我可以与“防御单位”和“运输单位”发生碰撞

			_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
			if (_gameLoop == null)
			{
				GD.PushError($"{nameof(Enemy)} requires a GameLoop node to apply crystal damage.");
			}
			GD.Print($"我出生在: {Position}");
		}

		public override void _PhysicsProcess(double delta)
		{
			MoveTowardCrystal();
			if (HandleLeak())
			{
				return;
			}

			UpdateAttackTimer((float)delta);
		}

		public void TakeDamage(float damage)
		{
			if (Health <= 0)
			{
				return;
			}

			Health -= damage;
			if (Health <= 0)
			{
				Die();
			}
		}

		private void AddCollisionShape()
		{
			var collision = new CollisionShape2D
			{
				Shape = new RectangleShape2D
				{
					Size = new Vector2(Config.EnemyCollisionSize, Config.EnemyCollisionSize)
				} 
			};
			AddChild(collision);
		}

		private void MoveTowardCrystal()
		{
			Velocity = Vector2.Left * MoveSpeed;
			MoveAndSlide();
		}

		private bool HandleLeak()
		{
			if (Position.X >= Config.EnemyLeakBoundaryX)
			{
				return false;
			}

			if (_gameLoop != null)
			{
				_gameLoop.State.CrystalHealth -= Config.SmallEnemyCrystalDamage;
				GD.Print($"漏怪了！水晶受到 {Config.SmallEnemyCrystalDamage} 点伤害，剩余耐久: {_gameLoop.State.CrystalHealth}");

				if (_gameLoop.State.CrystalHealth <= 0)
				{
					GD.Print("水晶被摧毁，游戏失败！(这里以后接结算UI)");
				}
			}

			QueueFree();
			return true;
		}

		private void UpdateAttackTimer(float delta)
		{
			_attackTimer -= delta;
			if (_attackTimer > 0)
			{
				return;
			}

			// 优先攻击挡路的防守单位，再攻击附近的运输单位。
			bool attacked = TryAttackDefenseUnit() || TryAttackDelivery();

			// 没有目标时短暂重试，避免每帧遍历场景分组。
			_attackTimer = attacked ? _attackInterval : Config.EnemyIdleAttackRetryInterval;
		}

		private bool TryAttackDefenseUnit()
		{
			foreach (Node node in GetTree().GetNodesInGroup("DefenseUnits"))
			{
				if (node is Unit tower && IsWithinAttackRange(tower))
				{
					GD.Print("怪物正在破坏挡路的装置！");
					tower.TakeDamage(_damage * Config.EnemyDefenseDamageMultiplier);
					return true;
				}
			}

			return false;
		}

		private bool TryAttackDelivery()
		{
			// 把 "Delivery" 改成 "Deliveries"
			foreach (Node node in GetTree().GetNodesInGroup("Deliveries"))
			{
				if (node is Delivery delivery && IsWithinAttackRange(delivery))
				{
					GD.Print("怪物正在攻击运钞车！"); // 加个打印方便排查
					delivery.TakeDamage(_damage);
					return true;
				}
			}
			return false;
		}

		private bool IsWithinAttackRange(Node2D target)
		{
			return Position.DistanceTo(target.Position) < Config.EnemyAttackRange;
		}

		private void Die()
		{
			GD.Print("敌人被消灭！");
			QueueFree();
		}
	}
}
