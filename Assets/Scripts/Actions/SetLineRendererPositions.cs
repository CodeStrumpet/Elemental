using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Effects)]
	[Tooltip("Sets a line renderer's start and end positions")]
	public class SetLineRendererPositions : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(LineRenderer))]
		public FsmOwnerDefault gameObject;

	    [UIHint(UIHint.Variable)]
		[Tooltip("Start position.")]
		public FsmVector3 startPos;

	    [UIHint(UIHint.Variable)]
		[Tooltip("End position.")]
		public FsmVector3 endPos;

	    public bool everyFrame;
		
	    public override void Reset()
	    {
		startPos = Vector3.zero;
		endPos = new Vector3(0, 0, 1);
		everyFrame = false;
	    }

	    public override void OnEnter()
	    {
		SetPositions();
		if (!everyFrame) {
		    Finish();
		}
	    }

	    public override void OnUpdate()
	    {
		SetPositions();
	    }

	    void SetPositions()
	    {
		var go = Fsm.GetOwnerDefaultTarget(gameObject);
		if (go == null) {
		    return;
		}

		var lineRenderer = go.GetComponent(typeof(LineRenderer)) as LineRenderer;
		if (lineRenderer == null) {
		    LogWarning("Missing line renderer: " + go.name);
		    return;
		}

		lineRenderer.SetPosition(0, startPos.Value);
		lineRenderer.SetPosition(1, endPos.Value);
	    }
	}
}

