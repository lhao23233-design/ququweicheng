using Godot;
using CoopDefense.Core;
using CoopDefense.Entities;

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
		public void SpawnDelivery()
		{
			var npc = new Delivery();
			
			// 视觉表现
			var visual = new ColorRect 
			{ 
				Color = Colors.Yellow, 
				// 大小强制用 64，严丝合缝填满网格
				Size = new Vector2(64, 64)
				
			};
			// 强制居中
			visual.Position = new Vector2(-32, -32); 
			npc.AddChild(visual);
			
			// 使用 GridSystem 的转换方法，确保逻辑绝对统一
			var gridSystem = GetTree().Root.FindChild("GridSystem", true, false) as GridSystem;
			GetTree().CurrentScene.AddChild(npc);
			npc.GlobalPosition = gridSystem.GridToWorld(new Vector2I(15, 0));
			GD.Print($"DeliverySystem：生成于 {npc.GlobalPosition}");
		}
	}
}
