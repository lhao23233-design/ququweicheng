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
                "000000011111111011111100000000",
                "000000000000000000000100000000",
                "000000000000000000000100000000",
                "000000011100000000010100000000",
                "110000000000000000010100000011",
                "120000010000000000010100000031",
                "110000010000000000010000000011",
                "000000010000000000010100000000",
                "000000011111110000011100000000",
                "000000000000000050000000000000",
                "111111111111111111111111111111",
                "111111111111111111111111111111",
                "111111111111111111111111111111",
                "111111111111111111111111111111",
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
                // 接收：抽取哪个地图（传空字符串代表随机）
        public static MapData GenerateMapData(string specificMapId = "")
        {
            string mapKey = string.IsNullOrEmpty(specificMapId) ? GetRandomMap() : specificMapId;
            
            var mapDataResult = new MapData();
            mapDataResult.Layout = mapTemplates[mapKey];
            
            // 用来找墙
            var obstacleCells = new HashSet<Vector2I>();

            for (int y = 0; y < mapDataResult.Layout.Length; y++)
            {
                string row = mapDataResult.Layout[y];
                for (int x = 0; x < row.Length; x++)
                {
                    char c = row[x];
                    Vector2I pos = new Vector2I(x, y);

                    if (c == '1') obstacleCells.Add(pos);
                    else if (c == '2') mapDataResult.CrystalPos = pos;
                    else if (c == '3') mapDataResult.EnemySpawnPos = pos;
                    else if (c == '4') mapDataResult.DeliveryEntryPos = pos;
                    else if (c == '5') mapDataResult.DeliveryExitPos = pos;
                }
            }
            mapDataResult.ObstacleCells = obstacleCells;
            return mapDataResult;
        }
    }
    public class MapData
    {
        public string[] Layout;             // 字符画
        public Vector2I CrystalPos;         // 水晶位置
        public Vector2I EnemySpawnPos;      // 出怪口位置
        public Vector2I DeliveryEntryPos;   // 入口位置
        public Vector2I DeliveryExitPos;    // 出口位置
        public HashSet<Vector2I> ObstacleCells = new HashSet<Vector2I>(); 
    }
}