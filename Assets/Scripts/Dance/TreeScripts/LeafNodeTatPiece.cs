using UnityEngine;
using System.Collections;

public class LeafNodeTatPiece : MonoBehaviour {
	
	//public Material material;
	public Texture tatTexture;
	
	// Use this for initialization
	void Start () {
		ProceduralTreeSegment pts = GetComponent<ProceduralTreeSegment>();
		if (pts == null) {
			throw new System.Exception("A ProceduralTreeSegment script must be attached for this script to work");
		}
		
		if (pts.DepthLevel == pts.MaxDepth) {
			//renderer.material = material;	
			renderer.material.mainTexture = tatTexture;
			renderer.material.shader = Shader.Find("Transparent/Diffuse");
		}
	}
}
