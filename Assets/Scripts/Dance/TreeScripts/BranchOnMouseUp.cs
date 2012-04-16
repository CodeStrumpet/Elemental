using UnityEngine;
using System.Collections;

public class BranchOnMouseUp : MonoBehaviour {
	public bool verbose = false;

	//private TreeSegment ts;
	
    void Start () {
		//ts = gameObject.GetComponent<TreeSegment>();
    }
	
    void Update () {
	
    }

	void OnMouseUp() {
		if (verbose) Debug.Log("OnMouseUp() entered");
//		if (ts != null) {
//			ts.Branch();
//		}
	}
}
