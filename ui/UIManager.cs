using Godot;

namespace CoopDefense.UI
{
	public partial class UIManager : CanvasLayer
	{
		// 拿到里面那个铺满屏幕的 Control 容器
		private Control _uiContainer;
		private bool _isHudHidden = false; // 新增：记录当前是不是隐藏状态

		public override void _Ready()
		{
			// 假设你在 UI 场景里，把那个全屏的 Control 命名为 "UIContainer"
			_uiContainer = GetNode<Control>("UIContainer");
			var toggleBtn = GetNode<Button>("ToggleContainer/HideBtn"); // 假设你的开关按钮叫这个
			toggleBtn.Pressed += OnToggleBtnPressed;
		}

		// 暴露给外部的“隐形开关”方法
		public void ToggleVisibility(bool isVisible)
		{
			_uiContainer.Visible = isVisible;
		}
			private void OnToggleBtnPressed()
		{
			// 切换隐藏状态
			_isHudHidden = !_isHudHidden;
			
			// 把 UI 容器的可见性设置为状态的相反值
			// （如果 _isHudHidden 是 true 代表要隐藏，那 Visible 就是 false）
			_uiContainer.Visible = !_isHudHidden;
			
			GD.Print($"UI 隐藏状态: {_isHudHidden}");
		}
	}
}
