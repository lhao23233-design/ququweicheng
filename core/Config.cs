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

	// 资源抽取
	public const float ResourceTickInterval = 10f;
}
