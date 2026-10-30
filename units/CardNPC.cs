using Godot;
using CoopDefense.Core;

namespace CoopDefense.Units
{
	public partial class CardNPC : Node2D
	{
		[Export] public float MoveSpeed = 10f; // 移动速度，10f特慢速度以测试npc死亡
		private float _targetX = -50f; // 目标位置：屏幕左侧外
		// 新增：NPC 的血量
		public float Health = 30f; 	
		public override void _Ready()
		{
			// 告诉 Godot，这个节点属于 CardNPC 分组
			AddToGroup("CardNPC");
			GD.Print("一个卡盒 NPC 诞生了，并加入了 CardNPC 分组！");
		}	
		public override void _Process(double delta)
		{
			// 每帧向左移动
			Position = new Vector2(Position.X - MoveSpeed * (float)delta, Position.Y);

			// 如果到达目标位置
			if (Position.X <= _targetX)
			{
				ReachDestination();
			}
		}
		 // 新增：被敌人攻击的方法
		public void TakeDamage(float damage)
		{
			Health -= damage;
			GD.Print($"卡盒 NPC 被攻击了！剩余血量: {Health}");
			
			if (Health <= 0)
			{
				GD.Print("卡盒 NPC 被摧毁了！玩家失去了本次收益。");
				// TODO: 这里以后可以加一个惩罚，比如扣除水晶耐久
				QueueFree(); 
			}
		}


		private void ReachDestination()
		{
			GD.Print("卡盒 NPC 成功抵达领地！");
			
			// 找到 GameLoop，直接发钱（测试用）
			var gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
			if (gameLoop != null)
			{
				gameLoop.State.Player1.Coins += 10;
				GD.Print($"获得 10 代币！当前代币: {gameLoop.State.Player1.Coins}");
			}
			
			QueueFree(); // 销毁自己，完成使命
		}
	}
}
