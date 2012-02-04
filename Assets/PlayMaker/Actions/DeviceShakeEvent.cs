// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends an Event when the mobile device is shaken.")]
	public class DeviceShakeEvent : FsmStateAction
	{
		[RequiredField]
		public FsmFloat shakeThreshold;
		[RequiredField]
		public FsmEvent sendEvent;

		public override void Reset()
		{
			shakeThreshold = 3f;
			sendEvent = null;
		}

		public override void OnUpdate()
		{
			var acceleration = Input.acceleration;
			
			if (acceleration.sqrMagnitude > (shakeThreshold.Value * shakeThreshold.Value))
				Fsm.Event(sendEvent);
		}
	}
}