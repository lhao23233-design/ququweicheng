using Godot;
using System.Collections.Generic;
using CoopDefense.Core;

namespace CoopDefense.Systems
{
    public static class Pathfinder
    {
        // 寻路节点：记录当前坐标和起点到当前点的实际耗时
        private class Node
        {
            public Vector2I Position;
            public float CostFromStart;
            
            public Node(Vector2I pos, float costFromStart)
            {
                Position = pos;
                CostFromStart = costFromStart;
            }
        }

        // 接收：起点、终点、路网、路障、以及每种格子的耗时字典
        public static List<Vector2I> FindFastestPath(
            Vector2I start, 
            Vector2I end, 
            HashSet<Vector2I> roadCells, 
            HashSet<Vector2I> blockedCells,
            Dictionary<Vector2I, float> cellCosts,
            float defaultSpeed = Config.MoveSpeed * Config.NpcOffRoadSpeedMultiplier,
            bool useRoadCosts = true)
        {
            var path = new List<Vector2I>();
            if (!IsInBounds(start) || !IsInBounds(end) ||
                blockedCells.Contains(start) || blockedCells.Contains(end))
            {
                return path;
            }

            float defaultStepCost = Config.CellSize / defaultSpeed;
            float minimumStepCost = defaultStepCost;
            if (useRoadCosts)
            {
                foreach (Vector2I roadCell in roadCells)
                {
                    if (cellCosts.TryGetValue(roadCell, out float roadCost) &&
                        roadCost > 0 && roadCost < minimumStepCost)
                    {
                        minimumStepCost = roadCost;
                    }
                }
            }

            var openList = new PriorityQueue<Node, float>();
            var costSoFar = new Dictionary<Vector2I, float>(); // 记录到每个格子已知的最短耗时
            var cameFrom = new Dictionary<Vector2I, Vector2I>(); // 记录路径前驱

            openList.Enqueue(new Node(start, 0), EstimateRemainingCost(start, end, minimumStepCost));
            costSoFar[start] = 0;

            Vector2I[] directions = { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };

            while (openList.Count > 0)
            {
                var current = openList.Dequeue();
                if (current.CostFromStart > costSoFar[current.Position])
                {
                    continue;
                }

                if (current.Position == end)
                {
                    // 回溯路径
                    Vector2I temp = end;
                    while (temp != start)
                    {
                        path.Add(temp);
                        temp = cameFrom[temp];
                    }
                    path.Reverse();
                    return path;
                }

                foreach (Vector2I dir in directions)
                {
                    Vector2I next = current.Position + dir;

                    if (!IsInBounds(next) || blockedCells.Contains(next)) continue;

                    float stepCost = defaultStepCost;
                    if (useRoadCosts && roadCells.Contains(next) &&
                        cellCosts.TryGetValue(next, out float roadCost) && roadCost > 0)
                    {
                        stepCost = roadCost;
                    }

                    float newCost = current.CostFromStart + stepCost;

                    // 如果发现了一条耗时更短的新路线
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        cameFrom[next] = current.Position;
                        float estimatedTotalCost = newCost +
                            EstimateRemainingCost(next, end, minimumStepCost);
                        openList.Enqueue(new Node(next, newCost), estimatedTotalCost);
                    }
                }
            }

            return path; // 走不通返回空
        }

        private static float EstimateRemainingCost(
            Vector2I position,
            Vector2I end,
            float minimumStepCost)
        {
            int manhattanDistance =
                Mathf.Abs(position.X - end.X) + Mathf.Abs(position.Y - end.Y);
            return manhattanDistance * minimumStepCost;
        }

        private static bool IsInBounds(Vector2I position)
        {
            return position.X >= 0 && position.X < Config.GridWidth &&
                   position.Y >= 0 && position.Y < Config.GridHeight;
        }
    }
}