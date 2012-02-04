// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Smoothly Rotates a Game Object so its forward vector points at a Target. The target can be defined as a Game Object or a world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
	public class SmoothLookAt : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		public FsmGameObject targetObject;
		public FsmVector3 targetPosition;
		public FsmVector3 upVector;
		public FsmBool keepVertical;
		[HasFloatSlider(0.5f,15)]
		public FsmFloat speed;
		//[Tooltip("Draw a line in the Scene View to the look at position.")]
		public FsmBool debug;
		
		GameObject previousGo; // track game object so we can re-initialize when it changes.
		Quaternion lastRotation;
		Quaternion desiredRotation;
		
		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			targetPosition = new FsmVector3 { UseVariable = true};
			upVector = new FsmVector3 { UseVariable = true};
			keepVertical = true;
			debug = false;
			speed = 5;
		}

		public override void OnEnter()
		{
			previousGo = null;
		}

		public override void OnLateUpdate()
		{
			DoSmoothLookAt();
		}

		void DoSmoothLookAt()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) return;

			GameObject goTarget = targetObject.Value;
			if (goTarget == null && targetPosition.IsNone) return;

			// re-initialize if game object has changed
			
			if (previousGo != go)
			{
				lastRotation = go.transform.rotation;
				desiredRotation = lastRotation;
				previousGo = go;
			}
			
			// desired look at position

			Vector3 lookAtPos;
			if (goTarget != null)
			{
				if (!targetPosition.IsNone)
					lookAtPos = goTarget.transform.TransformPoint(targetPosition.Value);
				else
					lookAtPos = goTarget.transform.position;
			}
			else
			{
				lookAtPos = targetPosition.Value;
			}
			
			if (keepVertical.Value)
			{
				lookAtPos.y = go.transform.position.y;
			}
			
			// smooth look at
			
			Vector3 diff = lookAtPos - go.transform.position;
			if (diff.sqrMagnitude > 0)
			{
				desiredRotation = Quaternion.LookRotation(diff, upVector.IsNone ? Vector3.up : upVector.Value);
			}

			lastRotation = Quaternion.Slerp(lastRotation, desiredRotation, speed.Value * Time.deltaTime);	
			go.transform.rotation = lastRotation;
			
			// debug line to target
			
			if (debug.Value)
			{
				Debug.DrawLine(go.transform.position, lookAtPos, Color.grey);
			}
		}

	}
}