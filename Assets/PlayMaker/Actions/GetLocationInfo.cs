// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{	
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets Location Info from a mobile device. NOTE: Use StartLocationService before trying to get location info.")]
	public class GetLocationInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmVector3 vectorPosition;		
		[UIHint(UIHint.Variable)]
		public FsmFloat longitude;
		[UIHint(UIHint.Variable)]
		public FsmFloat latitude;
		[UIHint(UIHint.Variable)]
		public FsmFloat altitude;
		[UIHint(UIHint.Variable)]
		public FsmFloat horizontalAccuracy;
		[UIHint(UIHint.Variable)]
		public FsmFloat verticalAccuracy;
		// TODO: figure out useful way to expose timeStamp
		// maybe how old is the location...?
		//[UIHint(UIHint.Variable)]
		//[Tooltip("Timestamp (in seconds since the game started) when location was last updated.")]
		//public FsmFloat timeStamp;
		[Tooltip("Event to send if the location cannot be queried.")]
		public FsmEvent errorEvent;

		public override void Reset()
		{
			longitude = null;
			latitude = null;
			altitude = null;
			horizontalAccuracy = null;
			verticalAccuracy = null;
			//timeStamp = null;
			errorEvent = null;
		}

		public override void OnEnter()
		{
#if UNITY_IPHONE
			DoGetLocationInfo();
#endif
			Finish();
		}

#if UNITY_IPHONE
		void DoGetLocationInfo()
		{
			if (iPhoneSettings.locationServiceStatus != LocationServiceStatus.Running)
			{
				Fsm.Event(errorEvent);
				return;
			}
			
			float x = iPhoneInput.lastLocation.longitude;
			float y = iPhoneInput.lastLocation.latitude;
			float z = iPhoneInput.lastLocation.altitude;
			
			vectorPosition.Value = new Vector3(x,y,z);
			
			longitude.Value = x;
			latitude.Value = y;
			altitude.Value = z;

			horizontalAccuracy.Value = iPhoneInput.lastLocation.horizontalAccuracy;
			verticalAccuracy.Value = iPhoneInput.lastLocation.verticalAccuracy;
			
			//timeStamp.Value = locationInfo.timestamp - 
		}
#endif
	}
}