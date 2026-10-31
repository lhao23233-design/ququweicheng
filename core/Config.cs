public static class Config {
	// 时间
	public const float RestPhaseDuration = 3f;
	public const float CombatPhaseDuration = 3f;

	// 体力
	public const float StaminaMax = 100f;
	public const float StaminaRestRestore = 20f;
	public const float StaminaCombatRestore = 2f;
	public const float StaminaDeploy = 10f;
	public const float StaminaMove = 5f;
	public const float StaminaCoopPerSec = 5f;
	public const float CourierStaminaMax = 50f;

	// 经济
	public const int CoinsBasePerRound = 10;
	public const int CoinsVariance = 3;
	public const int CardPriceBase = 5;
	public const int CardPageSizeBase = 3;
	public const int CardPageSizeMax = 6;
    static public int StartingCoins = 10;

	// 升本
	public const int AscendExpPerLevel = 10;
	public const int AscendCardDiscount = 1;
	public const int RelayExpDiscountMax = 3;

	// 敌人
	public const int WaveBaseCount = 5;
	public const int EliteDensityPerEvolution = 1;

	// 小Boss
	public const int MiniBossCrystalDamage = 5;
	public const int MiniBossBounty = 20;

	// 水晶
	public const int CrystalDurabilityMax = 100;
	public const int SmallEnemyCrystalDamage = 1;

	// 小怪
	public const float EnemyMaxHealth = 50f;
	public const float EnemyMoveSpeed = 80f;
	public const float EnemyAttackDamage = 10f;
	public const float EnemyAttackInterval = 1f;
	public const float EnemyAttackRange = 60f;
	public const float EnemyIdleAttackRetryInterval = 0.1f;
	public const float EnemyDefenseDamageMultiplier = 5f;
	public const float EnemyLeakBoundaryX = -50f;
	public const float EnemyCollisionSize = 40f;

	// 运输NPC
	public const int NpcCallCost = 10;           // 呼叫花费
	public const float NpcSpawnXMin = 300f;      // 出生点 X 坐标下限
	public const float NpcSpawnXMax = 400f;      // 出生点 X 坐标上限
	public const float NpcSpawnY = 0f;        // 出生点 Y 坐标
	public const float NpcCollisionSize = 30f;   // 碰撞框大小
	public const float NpcEndY = 900f;	//终点y坐标

	// 资源抽取
	public const float ResourceTickInterval = 10f;

	//地块&寻路设计
	public const int CellSize = 64;
    // 地图网格的宽高（比如 20x15 个格子）
    public const int GridWidth = 20;
    public const int GridHeight = 15;
}
