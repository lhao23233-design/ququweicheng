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
				// 这里未来要写：去 WaveConfig 里读剧本，把怪塞进队列
				_isWaveInitialized = true;
				_spawnTimer = 0f; // 重置计时器，准备刷第一只怪
			}

			// 2. 正常走刷怪倒计时
			_spawnTimer -= (float)delta;
			if (_spawnTimer <= 0)
			{
				// 这里未来要改成：如果队列里有东西，就抽出来刷
				SpawnEnemy(); 
				_spawnTimer = _spawnInterval;
			}
		}
		else
		{
			// 如果回到了休整期或结算期，重置标记，为下一波做准备
			_isWaveInitialized = false;
		}
	}

		private void SpawnEnemy()
		{
			if (_gridSystem == null)
			{
				return;
			}

			var enemy = new Enemy();
			GetTree().CurrentScene.AddChild(enemy);
			enemy.GlobalPosition = _gridSystem.GridToWorld(_gridSystem.EnemySpawnPos);
		}
	}
}
