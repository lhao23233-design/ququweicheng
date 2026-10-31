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
			// 动态用代码生成一个 Enemy 节点
			var enemy = new Enemy();
			
			// 给个视觉表现（红方块）
			var visual = new ColorRect { Color = Colors.Red, Size = new Vector2(40, 40) };
			visual.Position = new Vector2(visual.Size.X/2, visual.Size.Y/2); // 居中
			enemy.AddChild(visual);

			// 刷在屏幕最右侧，Y 随机浮动一点
			float randomY = (float)GD.RandRange(200.0, 400.0);
			enemy.Position = new Vector2(1100f, randomY); // 假设屏幕宽 1080

			// 加到主场景
			GetParent().AddChild(enemy);
			
			GD.Print("刷出一只怪！");
		}
	}
}
