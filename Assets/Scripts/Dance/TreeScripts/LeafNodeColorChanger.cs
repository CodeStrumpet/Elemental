using UnityEngine;
using System.Collections;

public class LeafNodeColorChanger : MonoBehaviour {
	
	private ProceduralTreeSegment pts;
	public Color leafColor = Color.green;
	
	// Use this for initialization
	void Start () {
		pts = GetComponent<ProceduralTreeSegment>();
		if (pts == null) {
			throw new System.Exception("A ProceduralTreeSegment script must be attached for this script to work");
		}
		
		if (pts.DepthLevel == pts.MaxDepth) {
			transform.renderer.material.color = leafColor;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
