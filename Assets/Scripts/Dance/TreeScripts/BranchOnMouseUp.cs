using UnityEngine;
using System.Collections;

public class BranchOnMouseUp : MonoBehaviour {
	public bool verbose = false;

	private ProceduralTreeSegment pts;
	
    void Start () {
		pts = gameObject.GetComponent<ProceduralTreeSegment>();
    }
	
    void Update () {
	
    }

	void OnMouseUp() {
		if (verbose) Debug.Log("OnMouseUp() entered");
		pts.Branch();
	}
}
