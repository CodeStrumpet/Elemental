// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event after an optional delay. NOTE: To send events between FSMs they must be marked as Global in the Events Browser.")]
	public class SendEvent : FsmStateAction
	{
		public FsmEventTarget eventTarget;
		[RequiredField]
		public FsmEvent sendEvent;
		[HasFloatSlider(0, 10)]
		public FsmFloat delay;

		DelayedEvent delayedEvent;

		public override void Reset()
		{
			sendEvent = null;
			delay = null;
		}

		public override void OnEnter()
		{
			if (delay.Value == 0f)
			{
				Fsm.Event(eventTarget, sendEvent);
				Finish();
			}
			else
			{
				delayedEvent = new DelayedEvent(Fsm, eventTarget, sendEvent, delay.Value);
			}
		}

		public override void OnUpdate()
		{
			delayedEvent.Update();
			
			if (delayedEvent.Finished)
			{
				Finish();
			}
		}
	}
}