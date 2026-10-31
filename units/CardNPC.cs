using Godot;
using CoopDefense.Core;

namespace CoopDefense.Units
{
	public partial class CardNPC : CharacterBody2D // 改为物理节点
	{
		public override void _Ready()
		{
			AddToGroup("CardNPC");
			GD.Print("一个卡盒 NPC 诞生了，并加入了 CardNPC 分组！");

			// 新增：动态给它加一个物理碰撞框
			var collision = new CollisionShape2D();
			collision.Shape = new RectangleShape2D { Size = new Vector2(Config.NpcCollisionSize, Config.NpcCollisionSize) };
			AddChild(collision);
		}
	}
}
