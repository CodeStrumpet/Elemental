using UnityEngine;
using System.Collections;

public class LeafNodeTatPiece : MonoBehaviour {
	
	//public Material material;
	public Texture[] tatTextures;
	
	private int lastSelectedPiece = -1;
	
	public int tatTreeNum = -1; //DEV: this should be made private after development
	private Vector3 scaleFactors; //to account 
	//private int xOffset; //because centers of tree pieces aren't always in the center of the image
	
	// Use this for initialization
	private void SetTreeSegTexture() {		
		
		tatTreeNum = Random.Range(0, tatTextures.Length);
		while (tatTreeNum == lastSelectedPiece) {
			tatTreeNum = Random.Range(0, tatTextures.Length);
		}
		lastSelectedPiece = tatTreeNum;	
		
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
	
	private void setNewYScale(Vector2 origImageDimensions) {
		
		float newYLocalScale = transform.localScale.x * origImageDimensions.y / origImageDimensions.x;
		transform.localScale = new Vector3(transform.localScale.x, newYLocalScale, transform.localScale.z);
	}
	
	//The midpoint of the trunk isn't necessarily the midpoint of the segment.
	//This function moves the midpoint of the trunk to where the midpoint of the segment was.
	private void offsetSegment(Vector2 origImageSize, int origLeftTrunkX, int origRightTrunkX) {
		float origMidpoint = origImageSize.x / 2.0f;
		float treeTrunkMidpoint = (origRightTrunkX + origLeftTrunkX) / 2.0f;
		float offSetInPixels = origMidpoint - treeTrunkMidpoint;
		offSetInPixels *= -1.0f; //for some reason, x directions are reversed
		float ratioToOffset = offSetInPixels / origImageSize.x;
		Debug.Log("ratioToOffset is " + ratioToOffset);
		//transform.Translate(Vector3.right * transform.localScale.x);
		Vector3 vecToTranslateBy = Vector3.right * transform.localScale.x * ratioToOffset;
		Debug.Log("vecToTranslateBy.x is " + vecToTranslateBy.x);
		transform.Translate(vecToTranslateBy);
	}
	
	private void SetScale(int textureNum) {
		//tree piece 1 original dimensions: 1814 x 4171
		//tree piece 1 span of trunk: 520 through 959
		
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
			case 1:
				origDimensions = new Vector2(2474, 4156);
				origLeftTrunkLoc = 1033;
				origRightTrunkLoc = 1419;
				break;			
			case 2:
				origDimensions = new Vector2(2064, 4152);
				origLeftTrunkLoc = 856;
				origRightTrunkLoc = 1405;
				break;			
			default:
				throw new System.Exception("Error in retrieving dimensions of provided texture");			
				break;
		}
		
		//what should the scaling be?
		//scale up by the percentage that the trunk occupies
		float percentOccupiedByTrunk = ((float) origRightTrunkLoc - origLeftTrunkLoc) / origDimensions.x;
		float recip = 1.0f / percentOccupiedByTrunk;
		transform.localScale *= recip;
		
		setNewYScale(origDimensions);
		offsetSegment(origDimensions, origLeftTrunkLoc, origRightTrunkLoc);
		
		//how much to offset on the x axis
		//int trunkMidpoint = (origRightTrunkLoc - origLeftTrunkLoc) / 2;
		//int imageMidpoint = origDimensions.x / 2;
		
		

	}
	
//	//DEBUG
//	private void Update() {
//		if (tatTreeNum == 0) {
//			Debug.Log("localScale in Update() is " + transform.localScale);
//		}
//	}
}
