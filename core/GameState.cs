using CoopDefense.Player;

namespace CoopDefense.Core
{
    public class GameState
    {
        public PlayerState Player1 { get; set; }

        public GameState()
        {
            Player1 = new PlayerState(1);
        }
    }
}
