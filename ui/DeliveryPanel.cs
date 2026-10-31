using Godot;
using CoopDefense.Core;

namespace CoopDefense.UI
{
	public partial class DeliveryPanel : Button
	{
		private GameLoop _gameLoop;
		private CoopDefense.Logistics.DeliverySystem _deliverySystem;

		public override void _Ready()
		{
			GD.Print(">>> DeliveryPanel 启动！");
			_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
			_deliverySystem = GetTree().Root.FindChild("DeliverySystem", true, false) as CoopDefense.Logistics.DeliverySystem;

			// 设置按钮的初始文字
			UpdateButtonText();
			
			// 绑定点击事件
			Pressed += OnPressed;
		}

		private void OnPressed()
		{
			if (_deliverySystem == null)
			{
				GD.Print("错误：找不到 DeliverySystem！");
				return;
			}

			// 核心：UI 只负责喊话，具体扣钱和生成由系统决定
			bool success = _deliverySystem.RequestDelivery();
			if (success)
			{
				UpdateButtonText();
			}
		}

		// 每次点击后，刷新一下按钮上的代币提示（可选）
		private void UpdateButtonText()
		{
			if (_gameLoop != null)
			{
				Text = $"呼叫运钞车 (花费 {Config.NpcCallCost} 代币, 现有 {_gameLoop.State.Player1.Coins})";
			}
		}
	}
}
