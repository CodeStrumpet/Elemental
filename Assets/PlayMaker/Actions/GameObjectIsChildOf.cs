// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if a Game Object is a Child of another Game Object.")]
	public class GameObjectIsChildOf : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		[RequiredField]
		public FsmGameObject isChildOf;
		public FsmEvent trueEvent;
		public FsmEvent falseEvent;
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public override void Reset()
		{
			gameObject = null;
			isChildOf = null;
			trueEvent = null;
			falseEvent = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
				DoIsChildOf(Owner);
			else
				DoIsChildOf(gameObject.GameObject.Value);
			
			Finish();
		}

		void DoIsChildOf(GameObject go)
		{
			if (go == null || isChildOf == null) return;
			
			bool isChild = go.transform.IsChildOf(isChildOf.Value.transform);
			
			if (storeResult != null)
				storeResult.Value = isChild;
			
			Fsm.Event(isChild ? trueEvent : falseEvent);
		}
	}
}