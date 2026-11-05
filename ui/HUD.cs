using Godot;
using CoopDefense.Core;

public partial class HUD : RichTextLabel
{
	private GameLoop _gameLoop;

	public override void _Ready()
	{
		_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
		if (_gameLoop == null)
		{
			GD.PushError($"{nameof(HUD)} requires a GameLoop node.");
		}
	}

	public override void _Process(double delta)
	{
		if (_gameLoop == null) return;
		if (_gameLoop.State == null || _gameLoop.State.Player1 == null) return;

		string phaseName = _gameLoop.CurrentPhase switch
		{
			Phase.Rest => "Rest",
			Phase.Combat => "Combat",
			Phase.Settlement => "Settlement",
			_ => "unknown"
		};

		float stamina = _gameLoop.State.Player1.CurrentStamina;
		float maxStamina = _gameLoop.State.Player1.MaxStamina;
		int coins = _gameLoop.State.Player1.Coins;
		Text = $"Round: {_gameLoop.CurrentRound}\n" +
			$"Phase: {phaseName}\n" +
			$"Time: {(int)_gameLoop.PhaseTimer:F0}s\n" +
			$"Stamina: {stamina:F0}/{maxStamina:F0}\n" +
			$"Coins: {coins}";
	}
}
