using Godot;
using System.Collections.Generic;

namespace CoopDefense.Systems
{
    public static class Pathfinder
    {
        // 寻路节点：记录当前坐标和已经花费的时间
        private class Node
        {
            public Vector2I Position;
            public float TimeCost;
            
            public Node(Vector2I pos, float time)
            {
                Position = pos;
                TimeCost = time;
            }
        }

        // 接收：起点、终点、路网、路障、以及每种格子的耗时字典
        public static List<Vector2I> FindFastestPath(
            Vector2I start, 
            Vector2I end, 
            HashSet<Vector2I> roadCells, 
            HashSet<Vector2I> blockedCells,
            Dictionary<Vector2I, float> cellCosts) // 新增：格子耗时字典
        {
            var path = new List<Vector2I>();
            var openList = new PriorityQueue<Node, float>(); // 优先队列：耗时越少的越先出队
            var costSoFar = new Dictionary<Vector2I, float>(); // 记录到每个格子已知的最短耗时
            var cameFrom = new Dictionary<Vector2I, Vector2I>(); // 记录路径前驱

            openList.Enqueue(new Node(start, 0), 0);
            costSoFar[start] = 0;

            Vector2I[] directions = { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };

            while (openList.Count > 0)
            {
                var current = openList.Dequeue();

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

                    // 如果被挡住，直接跳过
                    if (blockedCells.Contains(next)) continue;

                    // 获取走到这个格子的时间成本（如果没有路，或者字典里没配，给个默认高耗时）
                    float stepCost = 10f; 
                    if (roadCells.Contains(next) && cellCosts.ContainsKey(next))
                    {
                        stepCost = cellCosts[next]; // 玩家铺的路，取具体耗时
                    }
                    else if (!roadCells.Contains(next))
                    {
                        stepCost = 99f; // 没路的地方，极其耗时，逼它尽量走铺好的路
                    }

                    float newCost = current.TimeCost + stepCost;

                    // 如果发现了一条耗时更短的新路线
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        cameFrom[next] = current.Position;
                        openList.Enqueue(new Node(next, newCost), newCost);
                    }
                }
            }

            return path; // 走不通返回空
        }
    }
}