// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Translates a Game Object. Use a Vector3 variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	public class Translate : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;
		[UIHint(UIHint.Variable)]
		public FsmVector3 vector;
		public FsmFloat x;
		public FsmFloat y;
		public FsmFloat z;
		public Space space;
		[Tooltip("Translate over one second")]
		public bool perSecond;
		
		public override void Reset()
		{
			gameObject = null;
			vector = null;
			// default axis to variable dropdown with None selected.
			x = new FsmFloat { UseVariable = true };
			y = new FsmFloat { UseVariable = true };
			z = new FsmFloat { UseVariable = true };
			space = Space.Self;
			perSecond = true;
		}

		public override void OnUpdate()
		{
			if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
				DoTranslate(Owner);
			else
				DoTranslate(gameObject.GameObject.Value);
		}

		void DoTranslate(GameObject go)
		{
			// init
			
			Vector3 translate;
			
			if (vector.IsNone)
				translate = new Vector3(x.Value, y.Value, z.Value);
			else
				translate = vector.Value;

			// override any axis

			if (!x.IsNone) translate.x = x.Value;
			if (!y.IsNone) translate.y = y.Value;
			if (!z.IsNone) translate.z = z.Value;
		
			// apply
			
			if (!perSecond)
			{
				go.transform.Translate(translate, space);
			}
			else
			{
				go.transform.Translate(translate * Time.deltaTime, space);
			}
		}

	}
}