using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets an orthographic camera's size")]
	public class SetOrthographicSize : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(Camera))]
		public FsmOwnerDefault gameObject;

	    [RequiredField]
		public FsmFloat orthographicSize;

	    public override void Reset()
	    {
		orthographicSize = 1.0f;
	    }

	    public override void OnEnter()
	    {
		Finish();

		GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
		if (go == null) {
		    return;
		}

		Camera camera = go.GetComponent(typeof(Camera)) as Camera;
		if (camera == null) {
		    LogWarning("Missing camera: " + go.name);
		    return;
		}
		if (!camera.orthographic) {
		    LogWarning("Camera is not orthographic: " + go.name);
		    return;
		}

		camera.orthographicSize = orthographicSize.Value;
	    }
	}
}