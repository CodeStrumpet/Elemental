using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Custom")]
	[Tooltip("Sets a mouse orbit component's distance")]
	public class SetMouseOrbitDistance : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(MouseOrbit))]
		public FsmOwnerDefault gameObject;

	    public FsmFloat distance;

	    public override void Reset()
	    {
		distance = 10;
	    }

	    public override void OnEnter()
	    {
		Finish();
		
		var go = Fsm.GetOwnerDefaultTarget(gameObject);
		if (go == null) {
		    return;
		}

		var mouseOrbit = go.GetComponent(typeof(MouseOrbit)) as MouseOrbit;
		if (mouseOrbit == null) {
		    LogWarning("Missing mouse orbit script: " + go.name);
		    return;
		}

		mouseOrbit.distance = distance.Value;
	    }
	}
}
