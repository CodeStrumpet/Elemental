// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Smoothly Rotates a Game Object so its forward vector points in the specified Direction.")]
	public class SmoothLookAtDirection : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		[RequiredField]
		public FsmVector3 targetDirection;
		public FsmFloat minMagnitude;
		public FsmVector3 upVector;
		[RequiredField]
		public FsmBool keepVertical;
		[RequiredField]
		[HasFloatSlider(0.5f,15)]
		public FsmFloat speed;
		
		GameObject previousGo; // track game object so we can re-initialize when it changes.
		Quaternion lastRotation;
		Quaternion desiredRotation;
		
		public override void Reset()
		{
			gameObject = null;
			targetDirection = new FsmVector3 { UseVariable = true};
			minMagnitude = 0.1f;
			upVector = new FsmVector3 { UseVariable = true};
			keepVertical = true;
			speed = 5;
		}

		public override void OnEnter()
		{
			previousGo = null;
		}
		
		public override void OnLateUpdate()
		{
			DoSmoothLookAtDirection();
		}

		void DoSmoothLookAtDirection()
		{
			if (targetDirection.IsNone) return;
			
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) return;

			// re-initialize if game object has changed
			
			if (previousGo != go)
			{
				lastRotation = go.transform.rotation;
				desiredRotation = lastRotation;
				previousGo = go;
			}
			
			// desired direction

			Vector3 diff = targetDirection.Value;
			
			if (keepVertical.Value)
			{
				diff.y = 0;
			}
			
			// smooth look at

			if (diff.sqrMagnitude > minMagnitude.Value)
			{
				desiredRotation = Quaternion.LookRotation(diff, upVector.IsNone ? Vector3.up : upVector.Value);			
			}
			
			lastRotation = Quaternion.Slerp(lastRotation, desiredRotation, speed.Value * Time.deltaTime);	
			go.transform.rotation = lastRotation;			
		}
	}
}