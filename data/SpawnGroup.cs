namespace CoopDefense.Data //出怪
{
    public class SpawnGroup
    {
        public EnemyTrait Trait { get; private set; }    // 出什么怪
        public int BaseCount { get; private set; }       // 基础数量
        public int MaxVariance { get; private set; }     // 波动范围（±X）

        public SpawnGroup(EnemyTrait trait, int baseCount, int maxVariance = 0)
        {
            Trait = trait;
            BaseCount = baseCount;
            MaxVariance = maxVariance;
        }
    }
}