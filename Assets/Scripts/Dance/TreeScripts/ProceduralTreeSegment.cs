using UnityEngine;
using System.Collections;

public class ProceduralTreeSegment : MonoBehaviour
{
	public float minBranchAngle = 3f;
	public float maxBranchAngle = 35f;
	public float sizeScalar = 1f;
	public bool verbose = false;
	public bool readyToBranch = false;
	
	private int maxDepth = 7;
	private int depthLevel;
	private bool branched = false;
	private ProceduralTreeSegment branch1;
	private ProceduralTreeSegment branch2;
	public Vector3 branchLoc; //DEVEL: delete this. it should only be in the scope of the Branch() method
	private bool isRoot = true; //used to enforce calling Init() on children
	private bool hasGrowthBehavior = false;

	private TreePieceMeshMaker treePiece;
	
	//do very little in Awake(). It should all be done in Init()
	void Awake()
	{
		treePiece = GetComponent<TreePieceMeshMaker>();

		depthLevel = 0; //this must be overwritten in Init() for everyone but the root node
		setHasGrowthBehavior();
		
		if (!hasGrowthBehavior) {
			readyToBranch = true;
		}
	}

	public void Init (int _depthLevel, float _rotateAmount, float _sizeScalar)
	{		
		//_sizeScaler is the piece's starting width
		
		transform.Rotate(Vector3.forward, _rotateAmount);
		
		if (verbose) {
			Debug.Log("_sizeScalar is " + _sizeScalar);
		}
		
		transform.localScale = transform.localScale * _sizeScalar;
			
		//code to alter sizes based on tree depth
		/*
		if (_depthLevel == maxDepth) {
			Vector3 temp = new Vector3(transform.localScale.x, transform.localScale.y * Random.Range (1.5f, 2.5f), transform.localScale.z); // random length
			transform.localScale = temp;
			transform.renderer.material.color = new Color(100 / 255f, 100 / 255f, 100 / 255f, renderer.material.color.a); 
		} else if (_depthLevel > 5) { // more erratic
			Vector3 temp = new Vector3(transform.localScale.x, transform.localScale.y * Random.Range (1.0f, 1.5f), transform.localScale.z); // random length
			transform.localScale = temp;
		} else { // less erratic
			Vector3 temp = new Vector3(transform.localScale.x, transform.localScale.y * Random.Range (.80f, 1.15f), transform.localScale.z); // random length
			transform.localScale = temp;
		} 
		*/
		
		depthLevel = _depthLevel;
		isRoot = false;
	}
	
	private Vector3 GetBranchLoc() {
		return transform.localPosition + 
			transform.up.normalized * transform.localScale.y;
	}

	private void setHasGrowthBehavior() {
		GrowOnBirthProcedural temp = GetComponent<GrowOnBirthProcedural>();
		if (temp != null) {
			hasGrowthBehavior = true; //false because it's waiting for GrowOnBirthProcedural to set it to true
		} else {
			hasGrowthBehavior = false;
		}
	}
	
	void markBranchPoint() {
		GameObject branchSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		branchSphere.transform.localScale = Vector3.one * 0.15f;
		branchSphere.transform.position = branchLoc;
	}

	public bool Branch()
	{

		if (verbose) {
			Debug.Log("Branch() entered");
		}

		if (branched || !readyToBranch) {
			return false;
		}
		
		branchLoc = GetBranchLoc();

		if (verbose) markBranchPoint();
		
		if (depthLevel != 0 && isRoot) {
			throw new System.Exception("Cannot branch before calling Init() on TreeSegment");
		}

		if (depthLevel >= maxDepth) {
			//	    renderer.material.color = new Color(0, 1f, 0, renderer.material.color.a); //DEVEL
			return false;
		}
		
		int childDepth = depthLevel + 1;
		
		if (branch1 == null) {// if no first branch
			branch1 = Instantiate(this, branchLoc, transform.rotation) as ProceduralTreeSegment;
			branch1.Init(childDepth, Random.Range (minBranchAngle, maxBranchAngle), treePiece.endWidthRatio);
		}
		
		if (branch2 == null) {
			branch2 = Instantiate(this, branchLoc, transform.rotation) as ProceduralTreeSegment;
			branch2.Init(childDepth, Random.Range (minBranchAngle, maxBranchAngle) * -1f, treePiece.endWidthRatio);
		}

		branched = true;
		//	renderer.material.color = new Color(139 / 255f, 100 / 255f, 19 / 255f, renderer.material.color.a); //DEVEL

		Debug.Log("finished branching");//DEBUG


		return true;
	}
	
	public int DepthLevel {
		get {
			return depthLevel;
		}
	}

	public int MaxDepth {
		get {
			return maxDepth;
		}
	}

	void OnMouseUp()
	{
		Branch();
	}
}

