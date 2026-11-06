using CoopDefense.Core;
using System.Collections.Generic;

namespace CoopDefense.Data
{
    public class PlayerState
    {
        public int PlayerId { get; set; }
        public bool IsEliminated { get; set; } = false;
        public int Coins { get; set; }
        public int SelectedCardIndex { get; set; } = -1;
        public int SelectedCardPrice { get; set; } = 0;
        // 每回合运输车剩余次数与上限都从同一配置初始化。
        public int DeliveriesLeftThisRound { get; set; } = Config.DeliveriesLeftMAX;
        public int MaxDeliveriesThisRound { get; set; } = Config.DeliveriesLeftMAX;
        // 备战席（手牌）：用来存放买下来的材料卡
        public List<Card> Bench { get; private set; } = new List<Card>();
        public int Tier { get; set; } = 1;
        public int drawnCardCount { get; set; } = 3;// 每次抽卡数量，默认 3 张
        public PlayerState(int id)
        {
            PlayerId = id;
            Coins = Config.StartingCoins;
        }
    }
}