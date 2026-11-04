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
		}

		private void OnPressed()
		{
			if (_deliverySystem == null)
			{
				GD.Print("错误：找不到 DeliverySystem！");
				return;
			}
			bool success = _deliverySystem.RequestDelivery();
		// 核心：UI 只负责喊话，具体扣钱和生成由系统决定
		}

	}
}
