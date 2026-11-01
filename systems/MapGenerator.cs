using Godot;
using System.Collections.Generic;

namespace CoopDefense.Systems
{
    public static class MapGenerator
    {
        // 传入测试地图模板编号，返回不可通行的路障坐标
        public static HashSet<Vector2I> GenerateObstacles(int mapId, int width, int height)
        {
            var obstacles = new HashSet<Vector2I>();

            // 测试地图 1：一条带有死角绕弯的障碍
            if (mapId == 1)
            {
                // 假设我们在中间放一排障碍物，逼迫玩家或者 NPC 绕路
                for (int x = 5; x < 15; x++)
                {
                    obstacles.Add(new Vector2I(x, 8));
                }
            }

            return obstacles;
        }
    }
}