using Godot;
using CoopDefense.Core;
using CoopDefense.Units;

public partial class MapView : Node2D
{
	private GameLoop _gameLoop;

	public override void _Ready()
	{
		_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			var player = _gameLoop.State.Player1;
			
			// 判断是否在备战席选中了卡牌
			if (player.SelectedCardIndex >= 0)
			{
				// 1. 尝试扣体力
				if (player.TryConsumeStamina(Config.StaminaDeploy))
				{
					// 2. 生成方块
					Vector2 clickPos = GetGlobalMousePosition();
					SpawnTower(clickPos);
					
					GD.Print($"合成成功！部署了: {player.Bench[player.SelectedCardIndex].Name}");
					
					// 3. 重点：从备战席中删除这张卡，并清空选中状态
					player.Bench.RemoveAt(player.SelectedCardIndex);
					player.SelectedCardIndex = -1;
				}
				else
				{
					GD.Print("体力不足，无法合成！");
				}
			}
		}
	}
	private void SpawnTower(Vector2 pos)
	{
		// 用代码直接生成一个白方块测试
		var tower = new Node2D();
		var visual = new ColorRect { Color = Colors.White, Size = new Vector2(40, 40) };
		tower.AddChild(visual);
		tower.Position = pos;
		AddChild(tower);
	}
}
