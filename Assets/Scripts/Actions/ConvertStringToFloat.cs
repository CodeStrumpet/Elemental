using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Convert)]
	[Tooltip("Convert a String to a Float value")]
	public class ConvertStringToFloat : FsmStateAction
	{
	    [RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString sourceString;

	    [RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeFloat;

	    public override void Reset()
	    {
		sourceString = "";
		storeFloat = 0.0f;
	    }

	    public override void OnEnter()
	    {
		Finish();

		string source = sourceString.Value;
		if (source.Substring(source.Length - 1) == ".") {
		    source = source + "0";
		}
		float val = System.Convert.ToSingle(sourceString.Value);
		storeFloat.Value = val;
	    }
	}
}
