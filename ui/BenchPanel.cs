using Godot;
using CoopDefense.Core;

public partial class BenchPanel : Control
{
	private GameLoop _gameLoop;
	private readonly Button[] _slots = new Button[5];

	public override void _Ready()
	{
		_gameLoop = GetTree().Root.FindChild("GameLoop", true, false) as GameLoop;
		if (_gameLoop == null)
		{
			GD.PushError($"{nameof(BenchPanel)} requires a GameLoop node.");
			return;
		}

		// Buttons are scene children so their positions remain editable in Godot.
		for (int i = 0; i < 5; i++)
		{
			_slots[i] = GetNode<Button>($"Slot{i + 1}Button");
			_slots[i].Text = "空";
			_slots[i].Disabled = true;

			int index = i;
			_slots[i].Pressed += () =>
			{
				// 点击备战席的卡，进入放置模式
				var player = _gameLoop.State.Player1;
				if (index < player.Bench.Count)
				{
					player.SelectedCardIndex = index;
					GD.Print($"选中备战席材料: {player.Bench[index].Name}，点击地图放置");
				}
			};
		}
	}

	public override void _Process(double delta)
	{
		if (_gameLoop == null)
		{
			return;
		}

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
