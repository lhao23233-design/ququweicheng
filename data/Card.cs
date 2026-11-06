namespace CoopDefense.Data
{
    // 卡牌类型枚举
    public enum CardType
    {
        Tower,      // 防守塔
        Consumable, // 消耗品（比如铺路包）
        Building    // 局内建筑（提供全局 Buff）
    }

    public class Card
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        
        // --- 新增的核心逻辑属性 ---
        public CardType Type { get; set; } 
        public int Tier { get; set; }      // 阶级（1-6本）
        
        // 如果是消耗品或塔，这个数值代表攻击力 / 增加铺路数 / 血量等
        public int Value { get; set; }      

        public Card(string name, int price, string description, CardType type, int tier, int value)
        {
            Name = name;
            Price = price;
            Description = description;
            Type = type;
            Tier = tier;
            Value = value;
        }
    }
}