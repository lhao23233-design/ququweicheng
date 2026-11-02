using Godot;
using CoopDefense.Core;
using CoopDefense.Units;
using CoopDefense.Logistics;
public partial class MapView : Node2D
{
	private GameLoop _gameLoop;
	private GridSystem _gridSystem;
	private bool _isRoadBuildMode = false;

	public override void _Ready()
	{
		_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
		_gridSystem = GetTree().CurrentScene.FindChild("GridSystem", true, false) as GridSystem;
		if (_gridSystem == null)
			GD.Print("错误：MapView 找不到 GridSystem！");
		else
			GD.Print("成功找到 GridSystem！");
	}
	public override void _Input(InputEvent @event)
	{
		// 按键盘 R 键，切换铺路模式
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.R)
		{
			_isRoadBuildMode = !_isRoadBuildMode;
			GD.Print($"铺路模式: {(_isRoadBuildMode ? "开启" : "关闭")}");
		}
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton LeftmouseEvent && LeftmouseEvent.Pressed && LeftmouseEvent.ButtonIndex == MouseButton.Left)
		{
			Vector2 mousePos = GetGlobalMousePosition();
			Vector2I gridPos = _gridSystem.WorldToGrid(mousePos);
			GD.Print($"尝试在网格 {gridPos} 铺路");
			// 如果开启了铺路模式，就执行铺路
			if (_isRoadBuildMode)
			{
				if(!_gridSystem.RoadCells.Contains(gridPos) && !_gridSystem.ObstacleCells.Contains(gridPos)&&!Config.KeyPositions.Contains(gridPos))
				{
					_gridSystem.BuildRoad(gridPos);
					GD.Print($"铺路成功！网格 {gridPos} 已变为道路。");
				}
				else
				{
					GD.Print($"铺路失败！网格 {gridPos} 已经是道路或障碍物。");
				}
				return; // 铺完路直接返回，不执行建塔逻辑
			}
			var player = _gameLoop.State.Player1;
			// 否则，执行原本的“建炮塔”逻辑
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
		else if (@event is InputEventMouseButton rightMouseEvent && rightMouseEvent.Pressed && rightMouseEvent.ButtonIndex == MouseButton.Right)
		{
			Vector2 mousePos = GetGlobalMousePosition();
			Vector2I gridPos = _gridSystem.WorldToGrid(mousePos);
			GD.Print($"右键点击网格 {gridPos}");
			if (_isRoadBuildMode)
			{
			_gridSystem.DeleteRoad(gridPos);
			return; // 拆完路直接返回，不执行其他逻辑
			}
		}
	}
	private void SpawnTower(Vector2 pos)
	{
		// 用代码直接生成一个白方块测试
		var tower = new CoopDefense.Units.Unit();
		var visual = new ColorRect { Color = Colors.White, Size = new Vector2(40, 40) };
		visual.Position = new Vector2(-visual.Size.X / 2, -visual.Size.Y / 2);//先把方块的中心点对齐到塔的中心点
		tower.AddChild(visual);
		tower.Position = pos;
		AddChild(tower);
	}
}
