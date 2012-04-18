using UnityEngine;
using System.Collections;

public class BranchOnCollision : MonoBehaviour {
	
	private TreeSegment ts;
	
	// Use this for initialization
	void Start () {
		ts = gameObject.GetComponent<TreeSegment>();
		if (gameObject.GetComponent<Rigidbody>() == null) {
			throw new System.Exception("This GameObject needs a rigid body to trigger branching on collision");
		}
	}
	
	void OnCollisionEnter(Collision c) {
				
		//ignore collisions from another tree segment
		if (c.gameObject.GetComponent<TreeSegment>() != null) {
			return;
		}

		c.transform.renderer.material.color = Color.red;

		Debug.Log("Collided with type " + c.GetType());
		
		if(ts.Branch()) {
			Destroy(transform.rigidbody);
		}
	}
}
