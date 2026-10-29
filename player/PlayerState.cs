using CoopDefense.Core;

namespace CoopDefense.Player
{
    public class PlayerState
    {
        public int PlayerId { get; set; }
        public float CurrentStamina { get; set; }
        public float MaxStamina { get; set; } = Config.StaminaMax;
        public bool IsEliminated { get; set; } = false;
        public int Coins { get; set; }
        public PlayerState(int id)
        {
            PlayerId = id;
            CurrentStamina = MaxStamina;
            Coins = Config.StartingCoins;
        }

        public bool TryConsumeStamina(float amount)
        {
            if (CurrentStamina >= amount)
            {
                CurrentStamina -= amount;
                return true;
            }
            return false;
        }
    }
}