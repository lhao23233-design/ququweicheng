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
		HandleEscapeInput(@event);
		// ================= 处理鼠标左键 =================
		if (@event is InputEventMouseButton leftEvent && leftEvent.Pressed && leftEvent.ButtonIndex == MouseButton.Left)
		{
			Vector2 mousePos = GetGlobalMousePosition();
			Vector2I gridPos = _gridSystem.WorldToGrid(mousePos);

			// 情况 A：铺路模式
			if (_isRoadBuildMode)
			{
				GD.Print($"尝试在网格 {gridPos} 铺路");
				if (!_gridSystem.RoadCells.Contains(gridPos) 
					&& !_gridSystem.ObstacleCells.Contains(gridPos) 
					&& !_gridSystem.KeyPositions.Contains(gridPos))
				{
					
					_gridSystem.BuildRoad(gridPos);
					GD.Print($"铺路成功！");
				}
				else
				{
					GD.Print($"铺路失败！该位置不可铺路。");
				}
				return; // 铺路模式处理完直接退出
			}

			// 情况 B：建塔模式
			var player = _gameLoop.State.Player1;
			if (player.SelectedCardIndex >= 0)
			{
				// 核心优化 1：先判断地形合不合法！
				if (_gridSystem.ObstacleCells.Contains(gridPos) || _gridSystem.KeyPositions.Contains(gridPos))
				{
					GD.Print($"建塔失败！网格 {gridPos} 是障碍物或关键位置。");
					return; 
				}
					string deployedCardName = player.Bench[player.SelectedCardIndex].Name;
					// 生成方块
					SpawnTower(mousePos);	
					// 从备战席中删除卡牌并清空选中
					player.Bench.RemoveAt(player.SelectedCardIndex);
					player.SelectedCardIndex = -1;
					GD.Print($"合成成功！部署了: {deployedCardName}");
			}
		}
		// ================= 处理鼠标右键 =================
		else if (@event is InputEventMouseButton rightEvent && rightEvent.Pressed && rightEvent.ButtonIndex == MouseButton.Right)
		{
			// 1. 铺路模式：拆路
			if (_isRoadBuildMode)
			{
				Vector2 mousePos = GetGlobalMousePosition();
				Vector2I gridPos = _gridSystem.WorldToGrid(mousePos);
				GD.Print($"右键拆除网格 {gridPos} 的路");
				_gridSystem.DeleteRoad(gridPos);
				return;
			}
			
			// 2. 普通模式：取消选中卡牌
			var player = _gameLoop.State.Player1;
			if (player.SelectedCardIndex >= 0)
			{
				player.SelectedCardIndex = -1;
				GD.Print("已取消选中，变回空手。");
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
	// 1. 独立的方法
	private void HandleEscapeInput(InputEvent @event)
	{
		// 判断是不是键盘事件
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			// 判断是不是按下了 ESC
			if (keyEvent.Keycode == Key.Escape)
			{
				if (_isRoadBuildMode)
				{
					_isRoadBuildMode = false;
					GD.Print("退出铺路模式");
					return;
				}
				var player = _gameLoop.State.Player1;
				if (player.SelectedCardIndex >= 0)
				{
					player.SelectedCardIndex = -1;
					GD.Print("按下了 ESC，已取消选中。");
					
					// 如果你有预览图，在这里关闭
					// _previewSprite.Visible = false;
				}
			}
		}
	}
}
