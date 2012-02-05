// DEPRECATED: Use GetEnemyInRange

using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a game object within a given range of another")]
	public class GetGameObjectWithinRange : FsmStateAction
	{
	    [RequiredField]
		public FsmOwnerDefault gameObject;

	    [RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject store;

	    [RequiredField]
		[UIHint(UIHint.Tag)]
		public FsmString withTag;

		public FsmFloat range;
	    [Tooltip("Find Closest? (Uncheck for farthest)")]
		public FsmBool findClosest = true;

	    public bool everyFrame = false;

	    public override void Reset()
	    {
		store = null;
		withTag = null;
		range = 1.0f;
		findClosest = true;
		everyFrame = false;
	    }

	    public override void OnEnter()
	    {
		GetObject();
		if (!everyFrame) {
		    Finish();
		}
	    }

	    public override void OnUpdate()
	    {
		GetObject();
	    }

	    public override string ErrorCheck()
	    {
		if (withTag.Value == "Untagged" || string.IsNullOrEmpty(withTag.Value)) {
		    return "Specify a Tag.";
		}
		return null;
	    }

	    void GetObject()
	    {
		GameObject[] objects = GameObject.FindGameObjectsWithTag(withTag.Value);
		float distance;
		if (findClosest.Value) {
		    distance = Mathf.Infinity;
		} else {
		    distance = 0.0f;
		}
		GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
		Vector3 position = go.transform.position;
		foreach (GameObject obj in objects) {
		    Vector3 diff = obj.transform.position - position;
		    float curDistance = diff.sqrMagnitude;
		    if (curDistance > range.Value*range.Value) {
			continue;
		    }
		    if (findClosest.Value) {
			if (curDistance < distance) {
			    store.Value = obj;
			    distance = curDistance;
			}
		    } else {
			if (curDistance > distance) {
			    store.Value = obj;
			    distance = curDistance;
			}
		    }
		}
	    }
	}
}
