// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends the next event on the state each time the state is entered.")]
	public class SequenceEvent : FsmStateAction
	{
		[HasFloatSlider(0, 10)]
		public FsmFloat delay;

		DelayedEvent delayedEvent;
		int eventIndex;

		public override void Reset()
		{
			delay = null;
		}

		public override void OnEnter()
		{
			int eventCount = State.Transitions.Length;

			if (eventCount > 0)
			{
				string eventName = State.Transitions[eventIndex].EventName;
				delayedEvent = new DelayedEvent(Fsm, eventName, delay.Value);
				delayedEvent.Update();

				eventIndex++;
				if (eventIndex == eventCount)
					eventIndex = 0;
			}
		}

		public override void OnUpdate()
		{
			if (delayedEvent != null)
				delayedEvent.Update();
			
			if (delayedEvent.Finished)
				Finish();
		}
	}
}