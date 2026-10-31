using Godot;
using CoopDefense.Core;
using CoopDefense.Player;

public partial class CardPanel : VBoxContainer
{
	private GameLoop _gameLoop;
	private CardSystem _cardSystem;
	private Button[] _cardButtons = new Button[3];

	public override void _Ready()
	{
		_gameLoop = GetNode<GameLoop>("../GameLoop");
		_cardSystem = new CardSystem(_gameLoop.State.Player1);
		_cardSystem.RefreshPage();

		for (int i = 0; i < 3; i++)
		{
			_cardButtons[i] = new Button();
			AddChild(_cardButtons[i]);

			int index = i;

			// 按钮点击：买进备战席
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
		UpdateCardText();
	}

	private void UpdateCardText()
	{
		for (int i = 0; i < 3; i++)
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
}
