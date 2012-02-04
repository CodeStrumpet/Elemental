// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a Color.")]
	public class ConvertBoolToColor : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable;
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmColor colorVariable;
		public FsmColor falseColor;
		public FsmColor trueColor;
		public bool everyFrame;

		public override void Reset()
		{
			boolVariable = null;
			colorVariable = null;
			falseColor = Color.black;
			trueColor = Color.white;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoConvertBoolToColor();
			
			if (!everyFrame)
				Finish();
		}
		
		public override void OnUpdate()
		{
			DoConvertBoolToColor();
		}
		
		void DoConvertBoolToColor()
		{
			if (boolVariable.Value)
				colorVariable.Value = trueColor.Value;
			else
				colorVariable.Value = falseColor.Value;
		}
	}
}