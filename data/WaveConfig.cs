using System.Collections.Generic;

namespace CoopDefense.Data
{
    public static class WaveConfig
    {
        // 回合数 -> 出怪队列
        public static readonly Dictionary<int, List<SpawnGroup>> WaveSchedule = new Dictionary<int, List<SpawnGroup>>
        {
            {
                1, new List<SpawnGroup> 
                { 
                    new SpawnGroup(EnemyTrait.Normal, 5, 1) // 5 只普通怪，波动 ±1
                } 
            },
            {
                3, new List<SpawnGroup> 
                {
                    new SpawnGroup(EnemyTrait.Normal, 3, 0), 
                    new SpawnGroup(EnemyTrait.CargoHunter, 2, 1), // 2 只拦截怪，波动 ±1
                    new SpawnGroup(EnemyTrait.Normal, 2, 0)  
                }
            }
        };
    }
}