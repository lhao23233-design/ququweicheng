using Godot;
using System.Collections.Generic;
public static class Config {
	// 时间
	public const float RestPhaseDuration = 10f;
	public const float CombatPhaseDuration = 10f;
	// 经济
	public const int CoinsBasePerRound = 10;
	public const int CoinsVariance = 3;
	public const int CardPriceBase = 5;
	public const int CardPageSizeBase = 3;
	public const int CardPageSizeMax = 6;
	public const int RefreshCost = 5;
    static public int StartingCoins = 10;
	public const int MaxBenchSize = 7;
	static public int StartingBenchSize = 3;

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
	public const float EnemyAttackRange = 64f;
	public const float EnemyIdleAttackRetryInterval = 0.1f;
	public const float EnemyDefenseDamageMultiplier = 5f;
	public const float EnemyCollisionSize = 60f;

	// 运输NPC
	public const float NpcCollisionSize = 60f;   // 碰撞框大小
    public const float MoveSpeed = 100f;
	public const float NpcOffRoadSpeedMultiplier = 0.3f;
	public const float NpcAcceleration = 400f;
	public const int DeliveriesLeftMAX=2;//初始次数上限为2
	// 资源抽取
	public const float ResourceTickInterval = 10f;

	//地块&寻路设计
	public const int CellSize = 64;
    // 地图网格的宽高（比如 30x17 个格子）
    public const int GridWidth = 30;
    public const int GridHeight = 17;
	public static int MapId=1;
}
