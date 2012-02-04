// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Blends an Animation towards a Target Weight over a specified Time.\nOptionally sends an Event when finished.")]
	public class BlendAnimation : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		public FsmOwnerDefault gameObject;
		[RequiredField]
		[UIHint(UIHint.Animation)]
		public FsmString animName;
		[RequiredField]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat targetWeight;
		[RequiredField]
		[HasFloatSlider(0f, 5f)]
		public FsmFloat time;
		public FsmEvent finishEvent;

		DelayedEvent delayedFinishEvent;

		public override void Reset()
		{
			gameObject = null;
			animName = null;
			targetWeight = 1f;
			time = 0.3f;
			finishEvent = null;
		}

		public override void OnEnter()
		{
			DoBlendAnimation(gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value);
		}

		public override void OnUpdate()
		{
			delayedFinishEvent.Update();
			
			if (delayedFinishEvent.Finished)
				Finish();
		}

		void DoBlendAnimation(GameObject go)
		{
			if (go == null) return;

			if (go.animation == null)
			{
				LogWarning("Missing Animation component on GameObject: " + go.name);
				return;
			}

			AnimationState anim = go.animation[animName.Value];

			if (anim == null)
			{
				LogWarning("Missing animation: " + animName.Value);
				return;
			}

			float timeValue = time.Value;
			go.animation.Blend(animName.Value, targetWeight.Value, timeValue);
			
			
			// TODO: doesn't work well with scaled time
			if (finishEvent != null)
				delayedFinishEvent = new DelayedEvent(Fsm, finishEvent, anim.length);
			else
				Finish();
		}
	}
}