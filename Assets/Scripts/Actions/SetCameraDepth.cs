using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets a camera's depth")]
	public class SetCameraDepth : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(Camera))]
		public FsmOwnerDefault gameObject;

	    public FsmInt depth;

	    public override void Reset()
	    {
		depth = 0;
	    }

	    public override void OnEnter()
	    {
		Finish();
		
		var go = Fsm.GetOwnerDefaultTarget(gameObject);
		if (go == null) {
		    return;
		}

		var camera = go.GetComponent(typeof(Camera)) as Camera;
		if (camera == null) {
		    LogWarning("Missing camera: " + go.name);
		    return;
		}

		camera.depth = depth.Value;
	    }
	}
}
