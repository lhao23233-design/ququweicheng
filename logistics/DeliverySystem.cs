using Godot;
using CoopDefense.Core;
using CoopDefense.Units;

namespace CoopDefense.Logistics
{
	public partial class DeliverySystem : Node
	{
		private GameLoop _gameLoop;

		public override void _Ready()
		{
			_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
		}

		// 暴露给 UI 调用的接口：检查钱够不够，够了就扣钱并生成
		public bool RequestDelivery()
		{
			var player = _gameLoop.State.Player1;
			
			if (player.Coins >= Config.NpcCallCost)
			{
				player.Coins -= Config.NpcCallCost;
				SpawnDelivery(); // 调用你自己的生成方法
				GD.Print("运钞车已出发！");
				return true;
			}
			else
			{
				GD.Print($"代币不足 {Config.NpcCallCost}，无法呼叫运钞车！");
				return false;
			}
		}

		// 你原来的生成逻辑
		public void SpawnDelivery()
		{
			var npc = new Delivery();
			// 视觉表现
			var visual = new ColorRect 
			{ 
				Color = Colors.Yellow, 
				Size = new Vector2(Config.NpcCollisionSize, Config.NpcCollisionSize) 
			};
			visual.Position = new Vector2(-visual.Size.X / 2, -visual.Size.Y / 2); 
			npc.AddChild(visual);
			
			// 位置
			float randomX = (float)GD.RandRange(Config.NpcSpawnXMin, Config.NpcSpawnXMax);
			npc.Position = new Vector2(randomX, Config.NpcSpawnY);
			
			// 加到主场景
			GetTree().CurrentScene.AddChild(npc);
			GD.Print("DeliverySystem：一辆运钞车已发出。");
		}
	}
}
