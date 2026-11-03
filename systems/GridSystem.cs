using Godot;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CoopDefense.Logistics
{
	public partial class GridSystem : Node2D
	{
		public const int CellSize = Config.CellSize;
		public const int GridWidth = Config.GridWidth;
		public const int GridHeight = Config.GridHeight;
		public Vector2I CrystalPos { get; private set; }
		public Vector2I EnemySpawnPos { get; private set; }
		public Vector2I DeliveryEntryPos { get; private set; }
		public Vector2I DeliveryExitPos { get; private set; }
		// 这是一个全局公开的只读集合，谁都可以查
		public HashSet<Vector2I> KeyPositions { get; private set; } = new HashSet<Vector2I>();
		public void SetupAnchors(Vector2I crystal, Vector2I spawn, Vector2I entry, Vector2I exit)
		{
			CrystalPos = crystal;
			EnemySpawnPos = spawn;
			DeliveryEntryPos = entry;
			DeliveryExitPos = exit;
		}
		// 记录每一格的移动耗时（秒）
		public Dictionary<Vector2I, float> CellCosts { get; private set; } = new Dictionary<Vector2I, float>();
		// 记录所有地块的视觉节点
		private Dictionary<Vector2I, ColorRect> _cells = new Dictionary<Vector2I, ColorRect>();
		public HashSet<Vector2I> RoadCells { get; private set; } = new HashSet<Vector2I>();
		public HashSet<Vector2I> ObstacleCells { get; private set; } = new HashSet<Vector2I>();

		public override void _Ready()
		{
			// 1. 先向 MapGenerator 要地图数据（这会动态抽一张图，并解析出坐标和墙）
			var mapData = Map.MapGenerator.GenerateMapData();
			
			// 2. 把解析出来的坐标，赋值给 GridSystem 自己的属性
			SetupAnchors(
				mapData.CrystalPos, 
				mapData.EnemySpawnPos, 
				mapData.DeliveryEntryPos, 
				mapData.DeliveryExitPos
			);

			// 3. 生成所有基础网格
			for (int x = 0; x < GridWidth; x++)
			{
				for (int y = 0; y < GridHeight; y++)
				{
					var cell = new ColorRect
					{
						Color = new Color(0.15f, 0.15f, 0.15f),
						Size = new Vector2(CellSize - 2, CellSize - 2),
						Position = new Vector2(x * CellSize + 1, y * CellSize + 1),
						MouseFilter = Control.MouseFilterEnum.Ignore
					};
					AddChild(cell);
					_cells[new Vector2I(x, y)] = cell;
				}
			}

			// 4. 涂抹关键位置（用刚赋值的动态属性，不要用 Config）
			PaintCell(CrystalPos, Colors.Blue);   
			PaintCell(EnemySpawnPos, Colors.Red);    
			PaintCell(DeliveryEntryPos, Colors.Yellow); 
			PaintCell(DeliveryExitPos, Colors.Green); 

			// 5. 临时构造一个 HashSet 用来检查
			KeyPositions = new HashSet<Vector2I> { CrystalPos, EnemySpawnPos, DeliveryEntryPos, DeliveryExitPos };

			// 6. 生成障碍物并绘制
			ObstacleCells = mapData.ObstacleCells; // 假设你在 MapData 里加了 ObstacleCells 属性
			foreach (var obstacle in ObstacleCells)
			{
				// 不能占用关键位置
				if (KeyPositions.Contains(obstacle))
				{
					GD.PrintErr($"障碍物 {obstacle} 占用了关键位置，已忽略！");
					continue;
				}
				PaintCell(obstacle, Colors.Black);
			}

			GD.Print($"网格生成完毕！当前使用地图，水晶在 {CrystalPos}。");
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
			if (_cells.ContainsKey(gridPos) && !RoadCells.Contains(gridPos)&& !ObstacleCells.Contains(gridPos))
			{
				_cells[gridPos].Color = new Color(0.8f, 0.8f, 0.8f); // 变成浅灰色代表路
				RoadCells.Add(gridPos);
				CellCosts[gridPos] = CellSize / Config.MoveSpeed;
				GD.Print($"在 {gridPos} 铺了路。");
			}
		}
		public void DeleteRoad(Vector2I gridPos)
		{
			if (RoadCells.Contains(gridPos))
			{
				_cells[gridPos].Color = new Color(0.15f, 0.15f, 0.15f); // 变回深灰色
				RoadCells.Remove(gridPos);
				CellCosts.Remove(gridPos);
				GD.Print($"在 {gridPos} 拆除了路。");
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
