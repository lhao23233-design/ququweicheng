using Godot;
using CoopDefense.Core;
using CoopDefense.Units;

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

			_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
			if (_gameLoop == null)
			{
				GD.PushError($"{nameof(Enemy)} requires a GameLoop node to apply crystal damage.");
			}
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

			// 优先攻击挡路的防守单位，再攻击附近的卡盒 NPC。
			bool attacked = TryAttackDefenseUnit() || TryAttackCardNpc();

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

		private bool TryAttackCardNpc()
		{
			foreach (Node node in GetTree().GetNodesInGroup("CardNPC"))
			{
				if (node is CardNPC npc && IsWithinAttackRange(npc))
				{
					npc.TakeDamage(_damage);
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
