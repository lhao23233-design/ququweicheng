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
		// 1. 必须先计算坐标，后面才能用！
		Vector2I gridPos = _gridSystem.WorldToGrid(pos);
		Vector2 centerPos = _gridSystem.GridToWorld(gridPos);

		// 2. 防呆 1：不能建在墙上
		if (_gridSystem.ObstacleCells.Contains(gridPos))
		{
			GD.Print("这里不能建塔，这是墙！");
			return; 
		}

		// 3. 防呆 2：不能重叠建塔
		var existingTowers = GetTree().GetNodesInGroup("DefenseUnits");
		foreach (var node in existingTowers)
		{
			// 因为有了上一步的中心点计算，这里可以直接判断
			if (node is Node2D existingTower && existingTower.Position == centerPos)
			{
				GD.Print("这里已经有塔了！");
				return;
			}
		}

		// 4. 生成并挂载视觉
		var tower = new CoopDefense.Units.Unit();
		var visual = new ColorRect { Color = Colors.White, Size = new Vector2(64, 64) };
		visual.Position = new Vector2(-visual.Size.X / 2, -visual.Size.Y / 2);
		tower.AddChild(visual);

		// 5. 设置塔的位置并加入场景
		tower.Position = centerPos;
		AddChild(tower);
	}
}
