// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random State Event after an optional delay. Use this to transition to a random state from the current state.")]
	public class RandomEvent : FsmStateAction
	{
		[HasFloatSlider(0, 10)]
		public FsmFloat delay;

		DelayedEvent delayedEvent;

		public override void Reset()
		{
			delay = null;
		}

		public override void OnEnter()
		{
			if (State.Transitions.Length == 0) return;
			
			delayedEvent = new DelayedEvent(Fsm, GetRandomEvent(), delay.Value);
			delayedEvent.Update();
		}

		public override void OnUpdate()
		{
			if (delayedEvent != null)
			{
				delayedEvent.Update();

				if (delayedEvent.Finished)
				{
					Finish();
				}
			}
			else
			{
				Finish();
			}
		}

		string GetRandomEvent()
		{
			var randomIndex = Random.Range(0, State.Transitions.Length);
			return State.Transitions[randomIndex].FsmEvent.Name;
		}

	}
}