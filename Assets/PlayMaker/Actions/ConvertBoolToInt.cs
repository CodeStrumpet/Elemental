// (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to an Integer value.")]
	public class ConvertBoolToInt : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable;
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;
		public FsmInt falseValue;
		public FsmInt trueValue;
		public bool everyFrame;

		public override void Reset()
		{
			boolVariable = null;
			intVariable = null;
			falseValue = 0;
			trueValue = 1;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoConvertBoolToInt();
			
			if (!everyFrame)
				Finish();
		}
		
		public override void OnUpdate()
		{
			DoConvertBoolToInt();
		}
		
		void DoConvertBoolToInt()
		{
			if (boolVariable.Value)
				intVariable.Value = trueValue.Value;
			else
				intVariable.Value = falseValue.Value;
		}
	}
}