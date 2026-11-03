//存模板：建一个文件，用字符串数组“画”地图。比如用 0 代表空地，用 1 代表墙。画一个 30x17 的二维数组。
//抽地图：在 MapGenerator 里加个随机数，开局从模板库里抽一个字符串数组出来。
//翻译坐标：MapGenerator 负责把抽中的字符串数组遍历一遍，把所有 1 的坐标找出来，返回一个“障碍物坐标集合”。
using Godot;
using System.Collections.Generic;

namespace CoopDefense.Map
{
    public static class MapGenerator
    {
        // 模板库：字符串数组表示地图，0=空地，1=墙，2=水晶，3=敌人出生点，4=运输单位出生点，5=运输单位出口点。30x17 的二维数组
        private static readonly Dictionary<string, string[]> mapTemplates = new Dictionary<string, string[]>
        {
            // 示例地图 1 MAp_1
            ["MAP_1"] = new string[]
            {
                "000000000000000040000000000000",
                "000000000000000000000000000000",
                "000000000000000000000000000000",
                "000000011111111111111100000000",
                "000000000000000000000100000000",
                "000000000000000000000100000000",
                "000000011100000000011100000000",
                "000000000000000000010100000000",
                "020000010000000000010100000030",
                "000000010000000000010000000000",
                "000000010000000000010100000000",
                "000000010000000000010100000000",
                "000000010000000000000100000000",
                "000000011111111111111100000000",
                "000000000000000000000000000000",
                "000000000000000000000000000000",
                "000000000000000005000000000000",
            }
        };
        // 生成地图：返回障碍物坐标集合
        private static string GetRandomMap()
        {
            if (Config.MapId > 0)
            {
            return "MAP_" + Config.MapId;
            }
        // 否则随机抽一个 Key
        var keys = new List<string>(mapTemplates.Keys);
        int randomIndex = (int)(GD.Randi() % keys.Count);
        return keys[randomIndex];
        }
        // 解析地图：把字符串数组翻译成障碍物坐标集合
        public static HashSet<Vector2I> GenerateObstacleCells()
        {
            string mapKey = GetRandomMap();
            if (!mapTemplates.ContainsKey(mapKey))
            {
                GD.PrintErr($"地图模板 {mapKey} 不存在！");
                return new HashSet<Vector2I>();
            }

            string[] mapData = mapTemplates[mapKey];
            var ObstacleCells = new HashSet<Vector2I>();

            for (int y = 0; y < mapData.Length; y++)
            {
                string row = mapData[y];
                for (int x = 0; x < row.Length; x++)
                {
                    if (row[x] == '1') // '1' 表示墙
                    {
                        ObstacleCells.Add(new Vector2I(x, y));
                        //把new Vector2I(x, y) 加入 HashSet并返回；
                    }
                    if(row[x]=='2') // '2' 表示水晶
                    {
                        Config.CrystalPos = new Vector2I(x, y);
                    }
                    if(row[x]=='3') // '3' 表示敌人出生点
                    {
                        Config.EnemySpawnPos = new Vector2I(x, y);
                    }
                    if(row[x]=='4') // '4' 表示运输单位出生点
                    {
                        Config.DeliveryEntryPos = new Vector2I(x, y);
                    }
                    if(row[x]=='5') // '5' 表示敌人出生点
                    {
                        Config.DeliveryExitPos = new Vector2I(x, y);
                    }
                //让 Config 里的关键位置变量都指向对应的 Vector2I 坐标。
                }
            }
            return ObstacleCells;
        }
    }
}