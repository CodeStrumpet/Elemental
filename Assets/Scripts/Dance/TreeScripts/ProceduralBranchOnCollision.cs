using UnityEngine;
using System.Collections;

public class ProceduralBranchOnCollision : MonoBehaviour {
	
	private ProceduralTreeSegment ts;
	public bool verbose = false;
	
	// Use this for initialization
	void Start () {
		ts = gameObject.GetComponent<ProceduralTreeSegment>();

		if (ts == null) {
			throw new System.Exception("This GameObject needs a ProceduralTreeSegment");
		}
		
		if (gameObject.GetComponent<Rigidbody>() == null) {
			throw new System.Exception("This GameObject needs a rigid body to trigger branching on collision");
		}
	}
	
	void OnCollisionEnter(Collision c) {
						
		//ignore collisions from another tree segment
		if (c.gameObject.GetComponent<ProceduralTreeSegment>() != null) {
			return;
		}

		if (verbose) {
			Debug.Log("collision hit");	
		}

		//c.transform.renderer.material.color = Color.red;

		//Debug.Log("Collided with type " + c.GetType());
		
		if(ts.Branch()) {
			Destroy(transform.rigidbody);
		}
	}
}
