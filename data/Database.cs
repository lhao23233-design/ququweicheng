using System.Collections.Generic;

namespace CoopDefense.Data
{
    public static class Database
    {
        // 抽卡/炮塔池
        public static readonly Dictionary<int, string> CardNames = new Dictionary<int, string>
        {
            { 1, "基础炮塔" },
            { 2, "运输加速" },
            { 3, "重装步兵" }
        };

        // 怪物种类池（这里只写名字，不写属性）
        public static readonly Dictionary<int, string> EnemyNames = new Dictionary<int, string>
        {
            { 1, "近战普通小怪" },
            { 2, "远程普通小怪" },
            { 3, "拦截怪"}, 
            { 4, "嘲讽怪"}  
        };
    }
}