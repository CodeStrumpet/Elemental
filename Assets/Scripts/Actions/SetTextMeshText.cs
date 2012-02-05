using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets a TextMesh's text")]
	public class SetTextMeshText : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(TextMesh))]
		public FsmOwnerDefault gameObject;

	    public FsmString text;

	    public override void Reset()
	    {
		text = "Hello World";
	    }

	    public override void OnEnter()
	    {
		Finish();
		
		var go = Fsm.GetOwnerDefaultTarget(gameObject);
		if (go == null) {
		    return;
		}

		var textMesh = go.GetComponent(typeof(TextMesh)) as TextMesh;
		if (textMesh == null) {
		    LogWarning("Missing text mesh: " + go.name);
		    return;
		}

		textMesh.text = text.Value;
	    }
	}
}