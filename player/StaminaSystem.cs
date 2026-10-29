using Godot;
using CoopDefense.Core;

namespace CoopDefense.Player
{
    public partial class StaminaSystem : Node
    {
        private GameLoop _gameLoop;
        private PlayerState _player;

        public void Initialize(GameLoop gameLoop, PlayerState player)
        {
            _gameLoop = gameLoop;
            _player = player;
        }

        public override void _Process(double delta)
        {
            if (_player == null || _player.IsEliminated) return;

            float restoreRate = 0f;

            if (_gameLoop.CurrentPhase == Phase.Rest)
                restoreRate = Config.StaminaRestRestore;
            else if (_gameLoop.CurrentPhase == Phase.Combat)
                restoreRate = Config.StaminaCombatRestore;

            if (restoreRate > 0)
            {
                _player.CurrentStamina = Mathf.Min(_player.CurrentStamina + restoreRate * (float)delta, _player.MaxStamina);
            }
        }
    }
}