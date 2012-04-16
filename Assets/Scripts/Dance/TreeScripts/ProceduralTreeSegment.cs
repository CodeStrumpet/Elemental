using UnityEngine;
using System.Collections;

public class ProceduralTreeSegment : MonoBehaviour
{
	public int maxDepth = 7;
	public float minBranchAngle = 3f;
	public float maxBranchAngle = 35f;
	public float sizeScalar = 0.925f;
	public bool enableWidthDecay = false;
	public bool branched = false;
	public bool readyToBranch = false;

	private ProceduralTreeSegment branch1;
	private ProceduralTreeSegment branch2;
	private int depthLevel;
	public Vector3 branchLoc; //DEVEL: change back to private
	private bool isRoot = true; //used to enforce calling Init() on children
		
	private TreePieceMeshMaker treePiece;
	
	private Vector3 GetBranchLoc() {
		return transform.localPosition + 
			transform.up.normalized * transform.localScale.y * treePiece.height;
	}
	
	void Awake ()
	{
		treePiece = GetComponent<TreePieceMeshMaker>();

		//Covers the root node. For all children, these should be overwritten via Init
		branchLoc = GetBranchLoc();

		if (true) { //DEVEL
			GameObject branchSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);		//DEVEL
			branchSphere.transform.localScale = Vector3.one * 0.25f;
			branchSphere.transform.position = branchLoc;
		}
		
		depthLevel = 0;
	}

	public void Init (int _depthLevel, float _rotateAmount, float _sizeScalar)
	{		
		
		transform.Rotate(Vector3.forward, _rotateAmount);
		//transform.localScale = transform.localScale * _sizeScalar * Random.Range (80, 100) / 100f;
		
		// randomlength	of new branches	
		//transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * Random.Range (75, 115) / 100f, transform.localScale.z);
		// attenuation factor	
		
		//reset branchLoc, since it's been rotated
		branchLoc = GetBranchLoc();
		
		depthLevel = _depthLevel;
		isRoot = false;
		readyToBranch = false;
	}


	public bool Branch ()
	{
		
		if (branched || !readyToBranch) {
			return false;
		}
		
		Debug.Log("getting ready to branch"); //DEBUG
		
		if (depthLevel != 0 && isRoot) {
			throw new System.Exception ("Cannot branch before calling Init() on TreeSegment");
		}
		
		if (depthLevel >= maxDepth) {
			//	    renderer.material.color = new Color(0, 1f, 0, renderer.material.color.a); //DEVEL
			return false;
		}
		int childDepth = depthLevel + 1;
		if (branch1 == null) {
			// if no first branch
			branch1 = Instantiate (this, branchLoc, transform.rotation) as ProceduralTreeSegment;
			branch1.Init (childDepth, Random.Range (minBranchAngle, maxBranchAngle), sizeScalar);
		}
		
		if (branch2 == null) {
			branch2 = Instantiate (this, branchLoc, transform.rotation) as ProceduralTreeSegment;
			branch2.Init (childDepth, Random.Range (minBranchAngle, maxBranchAngle) * -1f, sizeScalar);
		}
		
		branched = true;
		//	renderer.material.color = new Color(139 / 255f, 100 / 255f, 19 / 255f, renderer.material.color.a); //DEVEL
		
		Debug.Log("finished branching");//DEBUG
		
		
		return true;
	}

	void OnMouseUp ()
	{
		Debug.Log("mouse pressed"); //DEBUG
		Branch ();
	}
}
