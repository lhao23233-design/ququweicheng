using System.Collections.Generic;
using Godot;
using CoopDefense.Core;
using CoopDefense.Units; // 如果你建了 Card 类，确保它在这个命名空间下

namespace CoopDefense.Player
{
    public class CardSystem
    {
        private PlayerState _player;
        private List<Card> _currentPage = new List<Card>();
        
        public IReadOnlyList<Card> CurrentPage => _currentPage;

        public CardSystem(PlayerState player)
        {
            _player = player;
        }

        public void RefreshPage()
        {
            _currentPage.Clear();
            _currentPage.Add(new Card("1", Config.CardPriceBase, "防守领地"));
            _currentPage.Add(new Card("2", Config.CardPriceBase, "加快物资"));
            _currentPage.Add(new Card("3", Config.CardPriceBase + 2, "强力抗伤"));
            GD.Print("卡池已刷新，当前有 3 张卡。");
        }

        public bool TryBuyCard(int index)
        {
            if (index < 0 || index >= _currentPage.Count) return false;
            
            Card card = _currentPage[index];
            if (_player.Coins >= card.Price)
            {
                _player.Coins -= card.Price;
                _player.Bench.Add(card); // 将卡片加入玩家的备战席
                GD.Print($"购买成功: {card.Name}，花费 {card.Price} 代币，已加入备战席。");
                return true;
            }
            GD.Print("代币不足！");
            return false;
        }
         public void OnCombatEnd()
        {
            _currentPage.Clear();
        }
    }
}