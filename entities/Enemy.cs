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
		public EnemyTrait Trait { get; set; } = EnemyTrait.Normal;
		private Delivery _lockedTarget = null; 
		private Vector2I _lockedTargetGrid;
		private int _pathStepsSinceLock = 0;
		private int _stepsSinceLastReplan = 0;

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

		private void MoveTowardDesitination(float delta)
		{
			// 如果当前路径为空，或者走完了，清空路径，下一帧重新算
			if (_currentPath == null || _pathIndex >= _currentPath.Count)
			{
				// 清空路径，下一帧重新算
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
				MoveAndSlide();
				return;
			}

			if (_currentPath == null ||
				(currentGrid != _lastGridPos && isCenteredOnCurrentCell && _stepsSinceLastReplan >= 2))
			{
				_lastGridPos = currentGrid;
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

			// TODO: 在这里按目标地块类型判断速度；当前怪物在所有可通行地块上都使用默认速度。
			float targetSpeed = MoveSpeed;
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

			MoveAndSlide();
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
		protected virtual Vector2I GetPathTarget()
		{
			// 如果我是拦截怪（CargoHunter）
			if (Trait == EnemyTrait.CargoHunter)
			{
				if (_lockedTarget == null || !IsInstanceValid(_lockedTarget))
        		{
					var deliveries = GetTree().GetNodesInGroup("Deliveries");
					if (deliveries.Count > 0 && deliveries[0] is Delivery car)
					{
						// 加上距离限制，防止它满地图追杀。假设 320 像素内才追。
						if (GlobalPosition.DistanceTo(car.GlobalPosition) < 320f)
						{
							_lockedTarget = car;
							// 返回车的当前网格坐标
							_lockedTargetGrid = _gridSystem.WorldToGrid(car.GlobalPosition);
						}
					}
				}
			if (_lockedTarget != null)
			{
				if (!IsInstanceValid(_lockedTarget))
				{
					_lockedTarget = null; 
				}
				// 如果车跑得太远（比如超过 640 像素），放弃追击
				else if (GlobalPosition.DistanceTo(_lockedTarget.GlobalPosition) > 640f)
				{
					_lockedTarget = null;
				}
				else
				{
					// 3. 一切正常，返回锁定的那个固定格子坐标（不再受车移动影响）
					return _lockedTargetGrid;
				}
			}
			}
			// 如果没有车，或者我是普通怪，就直奔水晶
			return _gridSystem.CrystalPos;
		}
	}
}
