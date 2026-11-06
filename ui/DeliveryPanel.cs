using Godot;
using CoopDefense.Core;
using CoopDefense.Logistics;

namespace CoopDefense.UI
{
	public partial class DeliveryPanel : Button
	{
		private GameLoop _gameLoop;
		private DeliverySystem _deliverySystem;

		public override void _Ready()
		{
			GD.Print(">>> DeliveryPanel 启动！");
			_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
			_deliverySystem = GetTree().Root.FindChild("DeliverySystem", true, false) as DeliverySystem;
			
			// 绑定点击事件
			Pressed += OnPressed;
			
			// 初始化文字
			UpdateButtonText();
		}

		public override void _Process(double delta)
		{
			// 让文字实时刷新（保证回合切换时次数会自动回满）
			UpdateButtonText();
		}

		private void OnPressed()
		{
			if (_deliverySystem == null)
			{
				GD.Print("错误：找不到 DeliverySystem！");
				return;
			}
			
			bool success = _deliverySystem.RequestDelivery();
			
			// 点击后立刻刷新一次文字（次数会瞬间减1）
			if (success)
			{
				UpdateButtonText();
			}
		}

		private void UpdateButtonText()
		{
			// 防呆：确保数据加载完毕
			if (_gameLoop == null || _gameLoop.State == null || _gameLoop.State.Player1 == null)
				return;

			var player = _gameLoop.State.Player1;
			int left = player.DeliveriesLeftThisRound;
			int max = player.MaxDeliveriesThisRound;

			// 如果次数用光了，按钮置灰（Disabled）
			if (left <= 0)
			{
				Text = $"呼叫运输车\n次数已用尽 ({left}/{max})";
				Disabled = true;
			}
			else
			{
				Text = $"呼叫运输车\n次数: {left}/{max}";
				Disabled = false;
			}
		}
	}
}
