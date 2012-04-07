using UnityEngine;
using System.Collections;

public class BranchOnMouseOver : MonoBehaviour {
    public Color onColor = Color.green;
    public Color offColor = Color.red;

	private TreeSegment ts;
	
    void Start () {
		ts = gameObject.GetComponent<TreeSegment>();
    }
	
    void Update () {
	
    }

	void OnMouseOver() {
		ts.Branch();
	}
	
	void OnMouseExit() {
	}
}
