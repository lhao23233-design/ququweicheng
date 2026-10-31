using CoopDefense.Player;

namespace CoopDefense.Core
{
    public class GameState
    {
        public PlayerState Player1 { get; set; }
        public float CrystalHealth { get; set; } = Config.CrystalDurabilityMax; // 初始 100 点
        public GameState()
        {
            Player1 = new PlayerState(1);
        }
    }
}
