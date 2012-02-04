// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
	public class ScaleTime : FsmStateAction
	{
		[RequiredField]
		[HasFloatSlider(0,4)]
		public FsmFloat timeScale;
		public bool everyFrame;

		public override void Reset()
		{
			timeScale = 1.0f;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			DoTimeScale();
			
			if (!everyFrame)
				Finish();
		}
		public override void OnUpdate()
		{
			DoTimeScale();
		}
		
		void DoTimeScale()
		{
			Time.timeScale = timeScale.Value;
			
			//TODO: found 0.02 in the docs... can this be set anywhere else?
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
		}
	}
}