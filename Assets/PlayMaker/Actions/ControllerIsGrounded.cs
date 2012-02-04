// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Tests if a Character Controller on a Game Object was touching the ground during the last move.")]
	public class ControllerIsGrounded : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		public FsmOwnerDefault gameObject;
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;
		public bool everyFrame;
		
		GameObject previousGo; // remember so we can get new controller only when it changes.
		CharacterController controller;
		
		public override void Reset()
		{
			gameObject = null;
			trueEvent = null;
			falseEvent = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoControllerIsGrounded();
			
			if (!everyFrame)
				Finish();
		}

		public override void OnUpdate()
		{
			DoControllerIsGrounded();
		}
		
		void DoControllerIsGrounded()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) return;
		
			if (go != previousGo)
			{
				controller = go.GetComponent<CharacterController>();
				previousGo = go;
			}
			
			if (controller == null)	return;
	
			bool isGrounded = controller.isGrounded;

			storeResult.Value = isGrounded;

			if (isGrounded)
				Fsm.Event(trueEvent);
			else
				Fsm.Event(falseEvent);
		}
	}
}
