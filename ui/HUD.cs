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

		// 2. 尝试通过路径找
		_gameLoop = GetNodeOrNull<GameLoop>("../GameLoop");
		
		if (_gameLoop == null)
		{
			GD.Print("方法1失败：没有找到平级的 GameLoop。");
			
			// 3. 用终极绝招：全图雷达搜索
			_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
			if (_gameLoop != null)
				GD.Print("方法2成功：全图搜索找到了 GameLoop！");
			else
				GD.Print("方法2也失败：整个游戏里根本没有叫 GameLoop 的节点！");
		}
		else
		{
			GD.Print("方法1成功：找到了平级的 GameLoop！");
		}
	}
	public override void _Process(double delta)
	{
		if (_gameLoop == null) return;

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
