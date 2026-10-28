using Godot;
using System;
using CoopDefense.Core;

namespace CoopDefense.Core
{
	public enum Phase
	{
		Rest,
		Combat,
		Settlement
	}

	public partial class GameLoop : Node
	{
		public Phase CurrentPhase { get; private set; } = Phase.Rest;
		public float PhaseTimer { get; private set; }

		// 信号：状态切换时通知 UI
		[Signal]
		public delegate void PhaseChangedEventHandler(int newPhase);

		public override void _Ready()
		{
			GD.Print("GameLoop 启动，进入休整期");
			PhaseTimer = Config.RestPhaseDuration;
			EmitSignal(SignalName.PhaseChanged, (int)CurrentPhase);
		}

		public override void _Process(double delta)
		{
			PhaseTimer -= (float)delta;

			if (PhaseTimer <= 0)
			{
				TransitionToNextPhase();
			}
		}

		private void TransitionToNextPhase()
		{
			switch (CurrentPhase)
			{
				case Phase.Rest:
					CurrentPhase = Phase.Combat;
					PhaseTimer = Config.CombatPhaseDuration;
					GD.Print("进入开展期");
					break;
				case Phase.Combat:
					CurrentPhase = Phase.Settlement;
					PhaseTimer = 3f; // 结算期固定3秒，可改
					GD.Print("进入结算期");
					break;
				case Phase.Settlement:
					CurrentPhase = Phase.Rest;
					PhaseTimer = Config.RestPhaseDuration;
					GD.Print("进入休整期，新一轮开始");
					break;
			}
			EmitSignal(SignalName.PhaseChanged, (int)CurrentPhase);
		}
	}
}
