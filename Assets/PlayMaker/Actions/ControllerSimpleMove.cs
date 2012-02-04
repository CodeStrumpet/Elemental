// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
	public class ControllerSimpleMove : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		public FsmOwnerDefault gameObject;
		[RequiredField]
		public FsmVector3 moveVector;
		public FsmFloat speed;
		public Space space;
		
		GameObject previousGo; // remember so we can get new controller only when it changes.
		CharacterController controller;
		
		public override void Reset()
		{
			gameObject = null;
			moveVector = new FsmVector3 {UseVariable = true};
			speed = 1;
			space = Space.World;
		}

		public override void OnUpdate()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) return;
		
			if (go != previousGo)
			{
				controller = go.GetComponent<CharacterController>();
				previousGo = go;
			}
			
			if (controller != null)
			{
				Vector3 move;
				
				if (space == Space.World)
					move = moveVector.Value;
				else
					move = go.transform.TransformDirection(moveVector.Value);
				
				controller.SimpleMove(move * speed.Value);
			}
			
		}
	}
}
