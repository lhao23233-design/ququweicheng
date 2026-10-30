using Godot;
using CoopDefense.Core;

public partial class BenchPanel : HBoxContainer
{
	private GameLoop _gameLoop;
	private Button[] _slots = new Button[5]; // 假设备战席最多显示 5 个槽位

	public override void _Ready()
	{
		_gameLoop = GetNode<GameLoop>("../GameLoop");

		// 动态生成 5 个槽位按钮
		for (int i = 0; i < 5; i++)
		{
			_slots[i] = new Button();
			_slots[i].Text = "空";
			_slots[i].Disabled = true;
			AddChild(_slots[i]);

			int indexbp = i;
			_slots[i].Pressed += () =>
			{
				// 点击备战席的卡，进入放置模式
				var player = _gameLoop.State.Player1;
				if (indexbp < player.Bench.Count)
				{
					player.SelectedCardIndex = indexbp; 
					GD.Print($"选中备战席材料: {player.Bench[indexbp].Name}，点击地图放置");
				}
			};
		}
	}

	public override void _Process(double delta)
	{
		var bench = _gameLoop.State.Player1.Bench;
		
		for (int i = 0; i < _slots.Length; i++)
		{
			if (i < bench.Count)
			{
				_slots[i].Text = bench[i].Name;
				_slots[i].Disabled = false;
			}
			else
			{
				_slots[i].Text = "空";
				_slots[i].Disabled = true;
			}
		}
	}
}
