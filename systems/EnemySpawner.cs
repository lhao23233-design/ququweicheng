using Godot;
using CoopDefense.Core;
using CoopDefense.Data;
using CoopDefense.Logistics;
using System.Collections.Generic;

namespace CoopDefense.Combat
{
	public partial class EnemySpawner : Node
	{
		private GameLoop _gameLoop;
		private float _spawnTimer = 0f;
		private float _spawnInterval = 1f; // 每1秒刷一只怪
		private Queue<EnemyTrait> _spawnQueue = new Queue<EnemyTrait>();
		private bool _isWaveInitialized = false; // 标记这一波有没有初始化
		private GridSystem _gridSystem;

		public override void _Ready()
		{
			_gameLoop = GetNode<GameLoop>("../GameLoop");
			_gridSystem = GetTree().Root.FindChild("GridSystem", true, false) as GridSystem;
			if (_gridSystem == null)
			{
				GD.PushError($"{nameof(EnemySpawner)} requires a GridSystem node to locate enemy spawns.");
			}
		}
		private int GetCurrentRound()
		{
			return _gameLoop.CurrentRound;
		}
		public override void _Process(double delta)
		{
			if (_gameLoop.CurrentPhase == Phase.Combat)
			{
				// 1. 只有在刚进入开展期的第一帧，才清空队列并读取剧本！
				if (!_isWaveInitialized)
				{
					_spawnQueue.Clear();
						// 1. 去 WaveConfig 查表
					if (WaveConfig.WaveSchedule.ContainsKey(_gameLoop.CurrentRound))
					{
						var groups = WaveConfig.WaveSchedule[_gameLoop.CurrentRound];
						
						// 2. 遍历组，计算波动，入队
						foreach (var group in groups)
						{
							// 波动 = BaseCount + 随机(-MaxVariance 到 MaxVariance)
							int variance = GD.RandRange(-group.MaxVariance, group.MaxVariance);
							int finalCount = group.BaseCount + variance;
							
							// 防止波动成负数或0
							if (finalCount < 1) finalCount = 1; 

							for (int i = 0; i < finalCount; i++)
							{
								_spawnQueue.Enqueue(group.Trait);
							}
						}
						GD.Print($"第 {_gameLoop.CurrentRound} 回合初始化完成，待出怪总数: {_spawnQueue.Count}");
					}
					else
					{
						GD.PrintErr($"警告：第 {_gameLoop.CurrentRound} 回合没有配置剧本！");
					}
					_isWaveInitialized = true;
					_spawnTimer = 0f; // 重置计时器，准备刷第一只怪
				}

				// 2. 正常走刷怪倒计时
				_spawnTimer -= (float)delta;
				if (_spawnTimer <= 0)
				{
					if (_spawnQueue.Count > 0)
					{
						// 从队列里取出一只怪的特质
						EnemyTrait nextTrait = _spawnQueue.Dequeue();
						SpawnEnemy(nextTrait); // 传给生成方法
						_spawnTimer = _spawnInterval; 
					}
				}
		}
		else
		{
		// 如果回到了休整期或结算期，重置标记，为下一波做准备
		_isWaveInitialized = false;
		}
		}

		private void SpawnEnemy(EnemyTrait trait)
		{
			var enemy = new Enemy();
			// 使用动态获取的坐标！
			enemy.Position = _gridSystem.GridToWorld(_gridSystem.EnemySpawnPos);
			enemy.Trait = trait; // 赋值

			// 画色块
			var visual = new ColorRect { Size = new Vector2(64, 64) };
			visual.Position = new Vector2(-32, -32);

			if (trait == EnemyTrait.CargoHunter)
				visual.Color = Colors.Purple; // 拦截怪紫色
			else
				visual.Color = Colors.Red;    // 普通怪红色

			enemy.AddChild(visual);
			AddChild(enemy); // 或者 GetTree().CurrentScene.AddChild(enemy);
		}
	}
}
