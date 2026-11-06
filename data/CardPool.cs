using System.Collections.Generic;

namespace CoopDefense.Data
{
    public static class CardPool
    {
        // 所有的卡牌都在这，分好 Tier（阶级）和 Type（类型）
        public static readonly List<Card> AllCards = new List<Card>
        {
            // --- 1本卡 ---
            new Card("基础炮塔", 5, "防守领地", CardType.Tower, 1, 10),
            new Card("初级铺路包", 5, "获得 3 次铺路次数", CardType.Consumable, 1, 3),
            
            // --- 2本卡 ---
            new Card("重装步兵", 10, "强力抗伤", CardType.Tower, 2, 25)
        };

        // 根据玩家的科技等级，获取对应的卡池
        public static List<Card> GetCardsForTier(int techLevel)
        {
            // 比如玩家是 1 级，就返回所有 Tier <= 1 的卡
            // 简单做法：用 LINQ 过滤，或者直接遍历
            var result = new List<Card>();
            foreach (var card in AllCards)
            {
                if (card.Tier <= techLevel)
                {
                    result.Add(card);
                }
            }
            return result;
        }
    }
}