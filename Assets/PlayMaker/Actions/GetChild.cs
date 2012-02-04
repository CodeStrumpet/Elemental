// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the Child of a Game Object by Name and/or Tag. Use this to find attach points etc.")]
	public class GetChild : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		public FsmString childName;
		[UIHint(UIHint.Tag)]
		public FsmString withTag;
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			gameObject = null;
			childName = "";
			withTag = "Untagged";
			storeResult = null;
		}

		public override void OnEnter()
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
				storeResult.Value = DoGetChildByName(Owner, childName.Value, withTag.Value);
			else
				storeResult.Value = DoGetChildByName(gameObject.GameObject.Value, childName.Value, withTag.Value);
			
			Finish();
		}

		static GameObject DoGetChildByName(GameObject root, string name, string tag)
		{
			if (root == null) return null;

			foreach (Transform child in root.transform)
			{
				if (!string.IsNullOrEmpty(name))
				{
					if (child.name == name)
					{
						if (!string.IsNullOrEmpty(tag))
						{
							if (child.tag.Equals(tag))
								return child.gameObject;
						}
						else return child.gameObject;
					}
				}
				if (!string.IsNullOrEmpty((tag)))
					if (child.tag == tag)
						return child.gameObject;

				GameObject returnObject = DoGetChildByName(child.gameObject, name, tag);
				if(returnObject != null)
					return returnObject;
			}

			return null;
		}

		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(childName.Value) && string.IsNullOrEmpty(withTag.Value))
				return "Specify Child Name, Tag, or both.";
			return null;
		}

	}
}