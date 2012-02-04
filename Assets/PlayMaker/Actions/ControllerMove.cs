// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Moves a Game Object with a Character Controller. See also CharacterSimpleMove.")]
	public class ControllerMove : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		public FsmOwnerDefault gameObject;
		[RequiredField]
		public FsmVector3 moveVector;
		public Space space;
		public FsmBool perSecond;
		
		GameObject previousGo; // remember so we can get new controller only when it changes.
		CharacterController controller;
		
		public override void Reset()
		{
			gameObject = null;
			moveVector = new FsmVector3 {UseVariable = true};
			space = Space.World;
			perSecond = true;
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
				
				if (perSecond.Value)
					controller.Move(move * Time.deltaTime);
				else
					controller.Move(move);
			}
			
		}
	}
}
