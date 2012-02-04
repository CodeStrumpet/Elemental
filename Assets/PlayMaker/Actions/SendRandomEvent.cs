// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event.")]
	public class SendRandomEvent : FsmStateAction
	{
		[CompoundArray("Events", "Event", "Weight")]
		public FsmEvent[] events;
		[HasFloatSlider(0, 1)]
		public FsmFloat[] weights;
		public FsmFloat delay;

		DelayedEvent delayedEvent;
		
		public override void Reset()
		{
			events = new FsmEvent[3];
			weights = new FsmFloat[3] {1,1,1};
			delay = null;
		}

		public override void OnEnter()
		{
			if (events.Length > 0)
			{
				int randomIndex = ActionHelpers.GetRandomWeightedIndex(weights);
			
				if (randomIndex != -1)
				{
					delayedEvent = new DelayedEvent(Fsm, events[randomIndex], delay.Value);
					delayedEvent.Update();
				
					if (delayedEvent.Finished)
						Finish();
					
					return;
				}
			}						
			
			Finish();
		}
		
		public override void OnUpdate()
		{
			delayedEvent.Update();
			
			if (delayedEvent.Finished)
				Finish();
		}
	}
}