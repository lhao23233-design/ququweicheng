using Godot;
using System.Collections.Generic;
using CoopDefense.Core;
using CoopDefense.Units;
using CoopDefense.Entities;
using CoopDefense.Logistics;
using CoopDefense.Systems;
using CoopDefense.Data;

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
		private GridSystem _gridSystem;
		private List<Vector2I> _currentPath;
		private int _pathIndex;
		private Vector2I _lastGridPos = new Vector2I(-1, -1);
		private Vector2I _lastPathTargetGrid = new Vector2I(-1, -1);
		public EnemyTrait Trait { get; set; } = EnemyTrait.Normal;
		private Delivery _lockedTarget = null; 
		private Vector2I _lockedTargetGrid;
		private int _pathStepsSinceLock = 0;
		private int _stepsSinceLastReplan = 0;
		private float _currentSpeedMultiplier = 1.0f;
		private float _stuckTimer = 0f;

		public override void _Ready()
		{
			AddToGroup("Enemies");
			AddCollisionShape();
			MotionMode = MotionModeEnum.Floating;
			// Layer 1 (值就是 1)：代表我是“敌人”层
            this.CollisionLayer = 1<<0; 
			this.CollisionMask = 1<<1 | 1<<2; // 代表我可以与“防御单位”和“运输单位”发生碰撞

			_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
			if (_gameLoop == null)
			{
				GD.PushError($"{nameof(Enemy)} requires a GameLoop node to apply crystal damage.");
			}
			_gridSystem = GetTree().Root.FindChild("GridSystem", true, false) as GridSystem;
			if (_gridSystem == null)
			{
				GD.PushError($"{nameof(Enemy)} requires a GridSystem node for pathfinding.");
			}
			GD.Print($"我出生在: {Position}");
		}

		public override void _PhysicsProcess(double delta)
		{
			MoveTowardDesitination((float)delta);
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
		private float MoveSpeedMultiplier()
		{
			return _currentSpeedMultiplier;
		}

		private void MoveTowardDesitination(float delta)
		{
			// 路径尚未创建或已经走完时，准备按当前位置重新规划
			if (_currentPath == null ||
				(_currentPath.Count > 0 && _pathIndex >= _currentPath.Count))
			{
				_currentPath = null;
				_lastGridPos = new Vector2I(-1, -1);
				_stepsSinceLastReplan = 0;
			}
			if (_gridSystem == null)
			{
				Velocity = Vector2.Zero;
				return;
			}

			Vector2I currentGrid = _gridSystem.WorldToGrid(GlobalPosition);
			Vector2 currentCellCenter = _gridSystem.GridToWorld(currentGrid);
			bool isCenteredOnCurrentCell = GlobalPosition.DistanceTo(currentCellCenter) <= 1f;
			float currentSpeed = Velocity.Length();
			Vector2I destinationGrid = GetPathTarget();

			if (currentGrid == destinationGrid)
			{
				float distanceToCenter = GlobalPosition.DistanceTo(currentCellCenter);
				if (distanceToCenter <= currentSpeed * delta + 1f)
				{
					GlobalPosition = currentCellCenter;
					Velocity = Vector2.Zero;
					return;
				}

				Velocity = SmoothMovement.GetVelocity(
					GlobalPosition,
					currentCellCenter,
					MoveSpeed,
					Config.NpcAcceleration,
					currentSpeed,
					delta);
				MoveAndTrackStuck(delta);
				return;
			}

			bool pathTargetChanged = destinationGrid != _lastPathTargetGrid;
			if (_currentPath == null ||
				(currentGrid != _lastGridPos && isCenteredOnCurrentCell) ||
				pathTargetChanged)
			{
				if (isCenteredOnCurrentCell)
				{
					_lastGridPos = currentGrid;
				}
				_lastPathTargetGrid = destinationGrid;
				_currentPath = Pathfinder.FindFastestPath(
					currentGrid,
					destinationGrid,
					_gridSystem.RoadCells,
					_gridSystem.ObstacleCells,
					_gridSystem.CellCosts,
					MoveSpeed,
					useRoadCosts: false);
				_pathIndex = 0;
				_stepsSinceLastReplan = 0; // 重算后重置步数
			}

			if (_currentPath.Count == 0 || _pathIndex >= _currentPath.Count)
			{
				Velocity = Vector2.Zero;
				return;
			}

			Vector2I targetGrid = _currentPath[_pathIndex];
			Vector2 targetPos = _gridSystem.GridToWorld(targetGrid);

			//在这里按目标地块类型判断速度；当前怪物在所有可通行地块上都使用默认速度。
			float targetSpeed = MoveSpeed * MoveSpeedMultiplier();
			Velocity = SmoothMovement.GetVelocity(
				GlobalPosition,
				targetPos,
				targetSpeed,
				Config.NpcAcceleration,
				currentSpeed,
				delta);

			float distanceToTarget = GlobalPosition.DistanceTo(targetPos);
			if (distanceToTarget <= Velocity.Length() * delta + 1f)
			{
				GlobalPosition = targetPos;
				_pathIndex++;
				_stepsSinceLastReplan++; // <--- 新增：成功走完一格，步数+1
				return;
			}

			MoveAndTrackStuck(delta);
		}

		private void MoveAndTrackStuck(float delta)
		{
			Vector2 posBeforeMove = GlobalPosition;
			MoveAndSlide();
			float actualMovedDistance = GlobalPosition.DistanceTo(posBeforeMove);

			if (Velocity.Length() <= 1f || actualMovedDistance >= 1f)
			{
				_stuckTimer = 0f;
				return;
			}

			_stuckTimer += delta;
			if (_stuckTimer <= 1.0f)
			{
				return;
			}

			GD.Print("怪物卡死了！强制重置寻路！");
			_stuckTimer = 0f;
			_currentPath = null;
			_pathIndex = 0;
			_lastGridPos = new Vector2I(-1, -1);
		}

		private bool HandleLeak()
		{
			if (_gridSystem == null ||
				_gridSystem.WorldToGrid(GlobalPosition) != _gridSystem.CrystalPos ||
				GlobalPosition.DistanceTo(_gridSystem.GridToWorld(_gridSystem.CrystalPos)) > 1f)
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
			bool attacked = TryAttackDefenseUnit();

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
		private bool IsWithinAttackRange(Node2D target)
		{
			return Position.DistanceTo(target.Position) < Config.EnemyAttackRange;
		}

		private void Die()
		{
			GD.Print("敌人被消灭！");
			QueueFree();
		}
		protected virtual Vector2I GetPathTarget()
		{
			if (Trait == EnemyTrait.CargoHunter)
			{
				if (_lockedTarget != null &&
					(!IsInstanceValid(_lockedTarget) || _lockedTarget.IsQueuedForDeletion()))
				{
					_lockedTarget = null;
					_currentSpeedMultiplier = 1.0f;
				}

				if (_lockedTarget == null)
				{
					var deliveries = GetTree().GetNodesInGroup("Deliveries");
					if (deliveries.Count > 0 &&
						deliveries[0] is Delivery car &&
						!car.IsQueuedForDeletion())
					{
						if (GlobalPosition.DistanceTo(car.GlobalPosition) < 320f)
						{
							_lockedTarget = car;
							//追击加速至1.5f
							_currentSpeedMultiplier = 1.5f;
						}
					}
				}

				if (_lockedTarget != null)
				{
					_lockedTargetGrid = _gridSystem.WorldToGrid(_lockedTarget.GlobalPosition);
					return _lockedTargetGrid;
				}
			}
			//默认速度1f
			_currentSpeedMultiplier = 1.0f;
			return _gridSystem.CrystalPos;
		}
	}
}
