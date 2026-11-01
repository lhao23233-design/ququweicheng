using Godot;
using CoopDefense.Core;

namespace CoopDefense.Combat
{
	public partial class EnemySpawner : Node
	{
		private GameLoop _gameLoop;
		private float _spawnTimer = 0f;
		private float _spawnInterval = 1.5f; // 每 1.5 秒刷一只怪

		public override void _Ready()
		{
			_gameLoop = GetNode<GameLoop>("../GameLoop");
		}

		public override void _Process(double delta)
		{
			// 只有在“开展期”才刷怪
			if (_gameLoop.CurrentPhase == Phase.Combat)
			{
				_spawnTimer -= (float)delta;
				if (_spawnTimer <= 0)
				{
					SpawnEnemy();
					_spawnTimer = _spawnInterval;
				}
			}
		}

		private void SpawnEnemy()
		{
			var enemy = new Enemy();
			enemy.Position = new Vector2(1888, 544); // 或者你固定的网格坐标
			GetTree().CurrentScene.AddChild(enemy);
		}
	}
}
