using Godot;
using CoopDefense.Core;
using CoopDefense.Player;

public partial class CardPanel : Control
{
	private GameLoop _gameLoop;
	private CardSystem _cardSystem;
	private readonly Button[] _cardButtons = new Button[7];
	private readonly Button[] _cardRefreshButton =new Button[1];

	public override void _Ready()
	{
		_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
		if (_gameLoop == null)
		{
			GD.PushError($"{nameof(CardPanel)} requires a GameLoop node.");
			return;
		}

		_cardSystem = new CardSystem(_gameLoop.State.Player1);
		for (int i = 0; i < _cardButtons.Length; i++)
		{
			_cardButtons[i] = GetNode<Button>($"Card{i + 1}Button");
			// 按钮点击：买进备战席
			int index = i;
			_cardButtons[i].Pressed += () =>
			{
				// 调用 CardSystem 扣钱并塞进 Bench
				bool success = _cardSystem.TryBuyCard(index);
				
				if (success)
				{
					GD.Print($"当前备战席有 {_gameLoop.State.Player1.Bench.Count} 张卡");
					UpdateCardText(); // 刷新按钮（可选：如果你想让买过的卡变成“已购买”或置灰）
				}
			};
		}
		//TODO: 绑定刷新按钮
		_cardRefreshButton[0] = GetNode<Button>("RefreshButton");
		_cardRefreshButton[0].Pressed += () =>
		{
			bool success = _cardSystem.TryRefreshShop();
			UpdateRefreshButtonText();
			// 刷新按钮文本
			if (success)
			{
				GD.Print($"刷新成功，当前金币: {_gameLoop.State.Player1.Coins}");
				UpdateCardText();
			}
			else
			{
				GD.Print($"刷新失败，当前金币: {_gameLoop.State.Player1.Coins}");
			}
		};
		UpdateCardText();
		UpdateRefreshButtonText();
	}

	private void UpdateCardText()
	{
		for (int i = 0; i < _cardButtons.Length; i++)
		{
			if (i < _cardSystem.CurrentPage.Count)
			{
				var card = _cardSystem.CurrentPage[i];
				_cardButtons[i].Text = $"{card.Name}\n花费: {card.Price}";
				_cardButtons[i].Disabled = false;
			}
			else
			{
				_cardButtons[i].Text = "空卡槽";
				_cardButtons[i].Disabled = true;
			}
		}
	}
	private void UpdateRefreshButtonText()
	{
		int refreshCost = Config.RefreshCost;
		_cardRefreshButton[0].Text = $"刷新\n花费: {refreshCost}";
	}
}
