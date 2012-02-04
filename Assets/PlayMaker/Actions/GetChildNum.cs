// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Child of a Game Object by Index.\nE.g., O to get the first child. HINT: Use this with an integer variable to iterate through children.")]
	public class GetChildNum : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		[RequiredField]
		public FsmInt childIndex;
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject store;

		public override void Reset()
		{
			gameObject = null;
			childIndex = 0;
			store = null;
		}

		public override void OnEnter()
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
				store.Value = DoGetChildNum(Owner);
			else
				store.Value = DoGetChildNum(gameObject.GameObject.Value);
			
			Finish();
		}

		GameObject DoGetChildNum(GameObject go)
		{
			if (go == null) return null;

			return go.transform.GetChild(childIndex.Value % go.transform.childCount).gameObject;
		}
	}
}