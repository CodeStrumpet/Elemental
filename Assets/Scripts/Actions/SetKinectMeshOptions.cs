using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Custom")]
	[Tooltip("Sets options for KinectMesh object")]
	public class SetKinectMeshOptions : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(KinectMesh))]
		public FsmOwnerDefault gameObject;

	    public FsmBool doLerp;
	    public FsmBool doBlur;

	    public override void Reset()
	    {
		doLerp = true;
		doBlur = true;
	    }

	    public override void OnEnter()
	    {
		Finish();
		
		var go = Fsm.GetOwnerDefaultTarget(gameObject);
		if (go == null) {
		    return;
		}

		var kinectMesh = go.GetComponent(typeof(KinectMesh)) as KinectMesh;

		if (kinectMesh == null) {
		    LogWarning("Missing KinectMesh script: " + go.name);
		    return;
		}
		
		kinectMesh.applyBlur = doBlur.Value;
		kinectMesh.applyLerp = doLerp.Value;
	    }
	}
}
