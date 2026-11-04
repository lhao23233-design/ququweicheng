using Godot;
using CoopDefense.Core;

public partial class HUD : Label
{
	private GameLoop _gameLoop;
	private ProgressBar _staminaBar;

		public override void _Ready()
	{
		// 1. 先看看我爸爸是谁，我到底叫什么
		GD.Print($"我的名字是: {Name}，我的父节点是: {GetParent().Name}");
		if (_gameLoop == null)
		{
			// 2. 直接全图雷达搜索，注意：别写重复的名字，Godot 里节点名是可以重复的
			_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
			if (_gameLoop != null)
				GD.Print("成功：全图搜索找到了 GameLoop！");
			else
				GD.Print("失败：整个游戏里根本没有叫 GameLoop 的节点！");
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
		Text = $"Round: {_gameLoop.CurrentRound} | Phase: {phaseName} | Time: {_gameLoop.PhaseTimer:F1}s\n" +
		$"Stamina: {stamina:F0}/{maxStamina} | Coins: {coins}";

		// 更新进度条
		if (_staminaBar != null)
			_staminaBar.Value = stamina;
	}
}
