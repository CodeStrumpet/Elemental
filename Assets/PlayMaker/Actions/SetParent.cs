// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the Parent of a Game Object.")]
	public class SetParent : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		public FsmGameObject parent;

		public override void Reset()
		{
			gameObject = null;
			parent = null;
		}

		public override void OnEnter()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);

			if (go != null)
				go.transform.parent = parent.Value == null ? null : parent.Value.transform;
			
			Finish();
		}
	}
}