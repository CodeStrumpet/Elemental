// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a Float value.")]
	public class ConvertBoolToFloat : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable;
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;
		public FsmFloat falseValue;
		public FsmFloat trueValue;
		public bool everyFrame;

		public override void Reset()
		{
			boolVariable = null;
			floatVariable = null;
			falseValue = 0;
			trueValue = 1;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoConvertBoolToFloat();
			
			if (!everyFrame)
				Finish();
		}
		
		public override void OnUpdate()
		{
			DoConvertBoolToFloat();
		}
		
		void DoConvertBoolToFloat()
		{
			if (boolVariable.Value)
				floatVariable.Value = trueValue.Value;
			else
				floatVariable.Value = falseValue.Value;
		}
	}
}