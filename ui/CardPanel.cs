using Godot;
using CoopDefense.Core;
using CoopDefense.Player;
using CoopDefense.Units;

public partial class CardPanel : VBoxContainer
{
    private GameLoop _gameLoop;
    private CardSystem _cardSystem;
    private Button _btnStaminaTest; // 用来测试扣体力

    public override void _Ready()
    {
        // 1. 找到 GameLoop 和 玩家数据
        _gameLoop = GetNode<GameLoop>("../GameLoop");
        _cardSystem = new CardSystem(_gameLoop.State.Player1);
        
        // 2. 刷新卡池（会打印在终端）
        _cardSystem.RefreshPage();

        // 3. 找我们放在 VBoxContainer 里的三个按钮
        var children = GetChildren();
        
        // 绑定前三个按钮为购买卡牌
        for (int i = 0; i < 3 && i < children.Count; i++)
        {
            if (children[i] is Button btn)
            {
                int index = i; // 防止闭包问题
                btn.Text = $"买卡 {i+1}";
                // 绑定点击事件
                btn.Pressed += () => 
                {
                    _cardSystem.TryBuyCard(index);
                    // 同时扣点体力测试
                    _gameLoop.State.Player1.TryConsumeStamina(Config.StaminaDeploy);
                };
            }
        }

        // 如果节点不够3个，或者你想单独加个测试按钮
        // 4. 添加一个专门测试扣体力的按钮
        _btnStaminaTest = new Button();
        _btnStaminaTest.Text = "测试扣 10 体力";
        AddChild(_btnStaminaTest);
        _btnStaminaTest.Pressed += () => 
        {
            bool success = _gameLoop.State.Player1.TryConsumeStamina(10f);
            GD.Print(success ? "消耗体力成功！" : "体力不足！");
        };
    }
}