using Godot;
using CoopDefense.Units;

namespace CoopDefense.Combat
{
	public partial class Enemy : Node2D
	{
		private float _attackTimer = 0f;
		private float _attackInterval = 1f;
		private float _damage = 10f;
		public override void _Ready()
		{
			AddToGroup("Enemies");
		}
		public override void _Process(double delta)
		{
			_attackTimer -= (float)delta;
			
			if (_attackTimer <= 0)
			{
				var npcs = GetTree().GetNodesInGroup("CardNPC");
				
				// 打印看看有没有找到 NPC
				GD.Print($"敌人正在索敌，场景里有 {npcs.Count} 个 CardNPC");
				
				foreach (Node node in npcs)
				{
					if (node is CardNPC npc)
					{
						float dist = Position.DistanceTo(npc.Position);
						GD.Print($"发现 NPC，当前距离: {dist}");
						
						if (dist < 150f)
						{
							GD.Print("距离小于 150，开火！");
							npc.TakeDamage(_damage);
							_attackTimer = _attackInterval;
							break;
						}
					}
				}
				// 如果没找到，也要重置计时器，防止每帧狂刷屏
				_attackTimer = _attackInterval; 
			}
		}
	}
}
