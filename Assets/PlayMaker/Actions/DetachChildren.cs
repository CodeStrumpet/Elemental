// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Unparents all children from the Game Object.")]
	public class DetachChildren : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
				DoDetachChildren(Owner);
			else
				DoDetachChildren(gameObject.GameObject.Value);
			
			Finish();
		}

		static void DoDetachChildren(GameObject go)
		{
			if (go == null) return;

			go.transform.DetachChildren();
		}
	}
}