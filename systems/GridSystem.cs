using Godot;
using System.Collections.Generic;

namespace CoopDefense.Logistics
{
	public partial class GridSystem : Node2D
	{
		public const int CellSize = Config.CellSize;
		public const int GridWidth = Config.GridWidth;
		public const int GridHeight = Config.GridHeight;
		// 记录每一格的移动耗时（秒）
		public Dictionary<Vector2I, float> CellCosts { get; private set; } = new Dictionary<Vector2I, float>();
		// 记录所有地块的视觉节点
		private Dictionary<Vector2I, ColorRect> _cells = new Dictionary<Vector2I, ColorRect>();
		public HashSet<Vector2I> RoadCells { get; private set; } = new HashSet<Vector2I>();

		public override void _Ready()
		{
			// 1. 生成所有网格
			for (int x = 0; x < GridWidth; x++)
			{
				for (int y = 0; y < GridHeight; y++)
				{
					var cell = new ColorRect
					{
						Color = new Color(0.15f, 0.15f, 0.15f), // 深灰色
						Size = new Vector2(CellSize-2, CellSize-2), // 强制 64
						Position = new Vector2(x * CellSize + 1, y * CellSize + 1),
						MouseFilter = Control.MouseFilterEnum.Ignore // 不让网格挡住鼠标点击！
					};
					// ------------------------------------------------

					AddChild(cell);
					_cells[new Vector2I(x, y)] = cell; // 记录这个地块
				}
			}

			// 2. 涂抹关键位置（注意要放在循环外面，只执行一次）
			PaintCell(new Vector2I(3, 14), Colors.Blue);   // 小水晶 (左下角)
			PaintCell(new Vector2I(29, 8), Colors.Red);    // 怪物出怪口 (右侧中间)
			PaintCell(new Vector2I(15, 0), Colors.Yellow); // 运钞车入口 (正上方)
			PaintCell(new Vector2I(15, 16), Colors.Green); // 传输口 (正下方)

			GD.Print("网格生成完毕！地标绘制完毕！");
		}

		// World positions are converted to grid-local coordinates before resolving the cell.
		public Vector2I WorldToGrid(Vector2 globalPosition)
		{
			Vector2 localPosition = ToLocal(globalPosition);
			int gridX = Mathf.FloorToInt(localPosition.X / CellSize);
			int gridY = Mathf.FloorToInt(localPosition.Y / CellSize);
			return new Vector2I(gridX, gridY);
		}

		// 新增：供运钞车调用的路况检测
		public bool IsWorldPositionRoad(Vector2 worldPosition)
		{
			Vector2I gridPos = WorldToGrid(worldPosition);
			return RoadCells.Contains(gridPos);
		}

		public void BuildRoad(Vector2I gridPos)
		{
			if (_cells.ContainsKey(gridPos) && !RoadCells.Contains(gridPos))
			{
				_cells[gridPos].Color = new Color(0.8f, 0.8f, 0.8f); // 变成浅灰色代表路
				RoadCells.Add(gridPos);
				CellCosts[gridPos] = CellSize / Config.MoveSpeed;
				GD.Print($"在 {gridPos} 铺了路。");
			}
		}

		public void PaintCell(Vector2I gridPos, Color color)
		{
			if (_cells.ContainsKey(gridPos))
			{
				_cells[gridPos].Color = color;
			}
		}
		public Vector2 GridToWorld(Vector2I gridPos)
		{
			Vector2 localCenter = new Vector2(
				gridPos.X * CellSize + CellSize / 2f,
				gridPos.Y * CellSize + CellSize / 2f);
			return ToGlobal(localCenter);
		}
	}
}
