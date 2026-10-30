using Godot;
using CoopDefense.Core;
using CoopDefense.Player;

public partial class CardPanel : VBoxContainer
{
	private GameLoop _gameLoop;
	private CardSystem _cardSystem;

	// 创建一个数组来存动态生成的按钮，方便后续更新文字
	private Button[] _cardButtons = new Button[3];

	public override void _Ready()
	{
		// 1. 拿数据
		_gameLoop = GetNode<GameLoop>("../GameLoop");
		_cardSystem = new CardSystem(_gameLoop.State.Player1);

		// 2. 刷新卡池（终端会打印“卡池已刷新”）
		_cardSystem.RefreshPage();

		// 3. 动态生成 3 个按钮并绑定事件
		for (int i = 0; i < 3; i++)
		{
			_cardButtons[i] = new Button();
			AddChild(_cardButtons[i]); // 把按钮加到 VBoxContainer 里

			int index = i; // C# 闭包特性，必须用一个局部变量接住 i

			// 绑定点击事件
			_cardButtons[i].Pressed += () =>
			{
				// 尝试买卡
				bool success = _cardSystem.TryBuyCard(index);

				// 如果买成功，更新 UI 文字，并且扣体力（作为测试）
				if (success)
				{
					UpdateCardText(); // 刷新按钮文字（因为买完后可能代币变了，你要改文字也可以）
					_gameLoop.State.Player1.TryConsumeStamina(Config.StaminaDeploy);
				}
			};
		}

		// 4. 初始化文字（把真正的卡名和价格写上去）
		UpdateCardText();
	}

	// 专门用来刷新按钮文字的方法
	private void UpdateCardText()
	{
		for (int i = 0; i < 3; i++)
		{
			if (i < _cardSystem.CurrentPage.Count)
			{
				var card = _cardSystem.CurrentPage[i];
				// 显示：卡名 \n 价格
				_cardButtons[i].Text = $"{card.Name}\n花费: {card.Price}";
				_cardButtons[i].Disabled = false; // 启用按钮
			}
			else
			{
				_cardButtons[i].Text = "空卡槽";
				_cardButtons[i].Disabled = true; // 没卡了禁用按钮
			}
		}
	}
}
