using System.Collections.Generic;
using System;
using CoopDefense.Core;
using CoopDefense.Data;

namespace CoopDefense.Player
{
    public class CardSystem
    {
        private static readonly int[][] TierProbabilities =
        {
            new[] { 100, 0, 0, 0, 0 },
            new[] { 55, 45, 0, 0, 0 },
            new[] { 40, 35, 25, 0, 0 },
            new[] { 30, 30, 32, 8, 0 },
            new[] { 20, 22, 25, 25, 8 },
            new[] { 20, 20, 20, 20, 20 }
        };

        private readonly PlayerState _player;
        private readonly List<Card> _currentPage = new List<Card>();
        public IReadOnlyList<Card> CurrentPage => _currentPage;

        public CardSystem(PlayerState player)
        {
            _player = player;
        }

        public void RefreshPage()
        {
            _currentPage.Clear();

            for (int i = 0; i < 3; i++)
            {
                int targetTier = RollTargetTier();
                List<Card> availableCards = null;

                while (targetTier >= 1)
                {
                    var cardsAtTier = CardPool.GetCardsForTier(targetTier);
                    var exactTierCards = new List<Card>();
                    foreach (Card card in cardsAtTier)
                    {
                        if (card.Tier == targetTier)
                        {
                            exactTierCards.Add(card);
                        }
                    }

                    if (exactTierCards.Count > 0)
                    {
                        availableCards = exactTierCards;
                        break;
                    }

                    targetTier--;
                }

                if (availableCards == null)
                {
                    return;
                }

                int randomIndex = Random.Shared.Next(availableCards.Count);
                _currentPage.Add(availableCards[randomIndex]);
            }
        }

        public bool TryRefreshShop()
        {
            if (_player.Coins < Config.RefreshCost)
            {
                return false;
            }

            _player.Coins -= Config.RefreshCost;
            RefreshPage();
            return true;
        }

        private int RollTargetTier()
        {
            int playerTier = Math.Clamp(_player.Tier, 1, TierProbabilities.Length);
            int[] probabilities = TierProbabilities[playerTier - 1];
            int roll = Random.Shared.Next(1, 101);
            int cumulativeProbability = 0;

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulativeProbability += probabilities[i];
                if (roll <= cumulativeProbability)
                {
                    return i + 1;
                }
            }

            return probabilities.Length;
        }

        public bool TryBuyCard(int index)
        {
            if (index < 0 || index >= _currentPage.Count)
            {
                return false;
            }

            Card card = _currentPage[index];
            if (_player.Coins < card.Price)
            {
                return false;
            }

            _player.Coins -= card.Price;
            _player.Bench.Add(card);
            return true;
        }

        public void OnCombatEnd()
        {
            _currentPage.Clear();
        }
    }
}