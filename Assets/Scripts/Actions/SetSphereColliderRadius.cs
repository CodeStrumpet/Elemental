using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
	[Tooltip("Sets a sphere collider's radius")]
	public class SetSphereColliderRadius : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(SphereCollider))]
		public FsmOwnerDefault gameObject;

	    [UIHint(UIHint.Variable)]
		[Tooltip("Desired Radius.")]
		public FsmFloat radius;

	    public bool everyFrame;
		
	    public override void Reset()
	    {
		radius = 0.5f;
		everyFrame = false;
	    }

	    public override void OnEnter()
	    {
		SetRadius();
		if (!everyFrame) {
		    Finish();
		}
	    }

	    public override void OnUpdate()
	    {
		SetRadius();
	    }

	    void SetRadius()
	    {
		var go = Fsm.GetOwnerDefaultTarget(gameObject);
		if (go == null) {
		    return;
		}

		var collider = go.GetComponent(typeof(SphereCollider)) as SphereCollider;
		if (collider == null) {
		    LogWarning("Missing sphere collider: " + go.name);
		    return;
		}

		collider.radius = radius.Value;
	    }
	}
}

