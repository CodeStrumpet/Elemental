// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a Game Object around each Axis. Use a Vector3 Variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	public class Rotate : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		[UIHint(UIHint.Variable)]
		public FsmVector3 vector;
		public FsmFloat xAngle;
		public FsmFloat yAngle;
		public FsmFloat zAngle;
		public Space space;
		[Tooltip("Rotate over one second")]
		public bool perSecond;
		public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			vector = null;
			// default axis to variable dropdown with None selected.
			xAngle = new FsmFloat { UseVariable = true };
			yAngle = new FsmFloat { UseVariable = true };
			zAngle = new FsmFloat { UseVariable = true };
			space = Space.Self;
			perSecond = false;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			DoRotate();

			if(!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoRotate();
		}

		void DoRotate()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);

			var rotate = vector.IsNone ? new Vector3(xAngle.Value, yAngle.Value, zAngle.Value) : vector.Value;

			// override any axis

			if (!xAngle.IsNone) rotate.x = xAngle.Value;
			if (!yAngle.IsNone) rotate.y = yAngle.Value;
			if (!zAngle.IsNone) rotate.z = zAngle.Value;
		
			// apply
			
			if (!perSecond)
			{
				go.transform.Rotate(xAngle.Value, yAngle.Value, zAngle.Value, space);
			}
			else
			{
				go.transform.Rotate(xAngle.Value * Time.deltaTime, 
					yAngle.Value * Time.deltaTime, 
					zAngle.Value * Time.deltaTime, 
					space);
			}
		}

	}
}