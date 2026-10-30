using CoopDefense.Core;
using System.Collections.Generic;
using CoopDefense.Units;

namespace CoopDefense.Player
{
    public class PlayerState
    {
        public int PlayerId { get; set; }
        public float CurrentStamina { get; set; }
        public float MaxStamina { get; set; } = Config.StaminaMax;
        public bool IsEliminated { get; set; } = false;
        public int Coins { get; set; }
        public int SelectedCardIndex { get; set; } = -1;
        public int SelectedCardPrice { get; set; } = 0;
        // 备战席（手牌）：用来存放买下来的材料卡
        public List<Card> Bench { get; private set; } = new List<Card>();
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