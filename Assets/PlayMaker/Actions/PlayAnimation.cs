// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Plays an Animation on a Game Object. You can add named animation clips to the object in the Unity editor, or with the Add Animation Clip action.")]
	public class PlayAnimation : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		[UIHint(UIHint.Animation)]
		public FsmString animName;
		public PlayMode playMode;
		[HasFloatSlider(0f, 5f)]
		public FsmFloat blendTime;
		public FsmEvent finishEvent;
		public bool stopOnExit;

		AnimationState anim;
		float prevAnimTime;

		public override void Reset()
		{
			gameObject = null;
			animName = null;
			playMode = PlayMode.StopAll;
			blendTime = 0.3f;
			finishEvent = null;
			stopOnExit = false;
		}

		public override void OnEnter()
		{
			DoPlayAnimation();
		}

		void DoPlayAnimation()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null || string.IsNullOrEmpty(animName.Value))
			{
				Finish();
				return;
			}
			
			if (string.IsNullOrEmpty(animName.Value))
			{
				LogWarning("Missing animName!");
				Finish();
				return;
			}

			if (go.animation == null)
			{
				LogWarning("Missing animation component!");
				Finish();
				return;
			}

			anim = go.animation[animName.Value];

			if (anim == null)
			{
				LogWarning("Missing animation: " + animName.Value);
				Finish();
				return;
			}

			float time = blendTime.Value;
			if (time == 0)
				go.animation.Play(animName.Value, playMode);
			else
				go.animation.CrossFade(animName.Value, time, playMode);
		}

		public override void OnUpdate()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null || anim == null) return;

			// Use helper since different wrap modes make it harder to tell if anim has finished
			if (ActionHelpers.HasAnimationFinished(anim, prevAnimTime, anim.time))
			{
				Fsm.Event(finishEvent);
				Finish();
			}

			prevAnimTime = anim.time;
		}

		public override void OnExit()
		{
			if (stopOnExit)
				StopAnimation();
		}

		void StopAnimation()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go != null && go.animation != null)
				go.animation.Stop(animName.Value);
		}
	}
}