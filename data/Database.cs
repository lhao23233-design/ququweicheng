using Godot;
using System.Collections.Generic;

namespace CoopDefense.Data
{
    public static class Database
    {
        // 这里先存一下卡池的雏形（以后可以换成从 JSON 读取）
        public static readonly Dictionary<int, string> CardNames = new Dictionary<int, string>
        {
            { 1, "基础炮塔" },
            { 2, "运输加速" },
            { 3, "重装步兵" }
        };

        // 怪物种类（先占位）
        public static readonly Dictionary<int, string> EnemyNames = new Dictionary<int, string>
        {
            { 1, "普通小怪" },
            { 2, "精英怪" }
        };
    }
}