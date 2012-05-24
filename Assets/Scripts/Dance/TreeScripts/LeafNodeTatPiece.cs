using UnityEngine;
using System.Collections;

public class LeafNodeTatPiece : MonoBehaviour {
	
	//public Material material;
	public Texture[] tatTextures;
	
	public int tatTreeNum = -1; //DEV: this should be made private after development
	private Vector3 scaleFactors; //to account 
	private int xOffset; //because centers of tree pieces aren't always in the center of the image
	
	// Use this for initialization
	private void SetTreeSegTexture() {		
		tatTreeNum = Random.Range(0, tatTextures.Length);
		renderer.material.mainTexture = tatTextures[tatTreeNum];
		renderer.material.shader = Shader.Find("Transparent/Diffuse");
		SetScale(tatTreeNum);
	}
	
	private void Initted() {
		ProceduralTreeSegment pts = GetComponent<ProceduralTreeSegment>();
		if (pts == null) {
			throw new System.Exception("A ProceduralTreeSegment script must be attached for this script to work");
		}

		if (pts.DepthLevel == pts.MaxDepth) {
			SetTreeSegTexture();
		}
	}
	
	private void SetScale(int textureNum) {
		Debug.Log("SetScale called");
		//tree piece 1 original dimensions: 1814 x 4171
		//tree piece 1 span of trunk: 520 through 959
		
		Debug.Log("textureNum is " + textureNum);
		
		Vector2 origDimensions;
		int origLeftTrunkLoc;
		int origRightTrunkLoc;
		
		switch(textureNum) 
		{
			case 0:
				origDimensions = new Vector2(1814, 4171);
				origLeftTrunkLoc = 520;
				origRightTrunkLoc = 959;
				break;
			default:
				throw new System.Exception("Error in retrieving dimensions of provided texture");			
				break;
		}
		
		//what should the scaling be?
		//scale up by the percentage that the trunk occupies
		float percentOccupiedByTrunk = ((float) origRightTrunkLoc - origLeftTrunkLoc) / origDimensions.x;
		Debug.Log("percentOccupiedByTrunk: " + percentOccupiedByTrunk);
		float recip = 1.0f / percentOccupiedByTrunk;
		Debug.Log("recip is " + recip);
		Debug.Log("localScale is " + transform.localScale);
		transform.localScale *= recip;
		//Debug.Log("localScale is now " + transform.localScale);
		
		//how much to offset on the x axis
		

	}
	
//	//DEBUG
//	private void Update() {
//		if (tatTreeNum == 0) {
//			Debug.Log("localScale in Update() is " + transform.localScale);
//		}
//	}
}
