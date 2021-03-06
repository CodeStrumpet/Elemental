using UnityEngine;
using System.Collections;

public class TreeSegment : MonoBehaviour {
    public int maxDepth = 7;
    public float minBranchAngle = 3f;
    public float maxBranchAngle = 35f;
    public float sizeScalar = 0.925f;
    public bool enableAlphaDecay = false;
    public bool enableWidthDecay = true;
    public float alphaDecay = 0.9f;
    public bool branched = false;
    public bool readyToBranch = false;

    private TreeSegment branch1;
    private TreeSegment branch2;
    private int depthLevel;
    private Vector3 branchLoc;
    private bool isRoot = true; //used to enforce calling Init() on children
    private Vector3 basePoint; // origin point for branches

    void Awake() {
	//Covers the root node. For all children, these should be overwritten via Init
		branchLoc = transform.localPosition + 
	    transform.up.normalized * transform.localScale.y;
		depthLevel = 0;
    }

    public void Init(int _depthLevel, float _rotateAmount, float _sizeScalar) {
	if (enableAlphaDecay) {
	    Color c = renderer.material.color;
	    float newAlpha = c.a * alphaDecay;
	    //	    renderer.material.color = new Color(c.r, c.g, c.b, newAlpha);
	}
		
	basePoint = transform.localPosition  - transform.up.normalized * transform.localScale.y / 2f;
		
	// transform.Rotate(Vector3.forward, _rotateAmount);
	transform.RotateAround(basePoint, Vector3.forward, _rotateAmount); // Vector3.forward
	transform.localScale = transform.localScale * _sizeScalar * Random.Range(80,100)/100f;
		
	// randomlength	of new branches	
	transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y*Random.Range(75,115)/100f, transform.localScale.z); // attenuation factor	
		
	if (enableWidthDecay) {// make widths smaller
	    if (depthLevel > 3) { // afterdepth level3
			transform.localScale = new Vector3(transform.localScale.x* .6f, transform.localScale.y, transform.localScale.z); // attenuation factor	
	    }
	    if (depthLevel > 7) { // afterdepth level3
			transform.localScale = new Vector3(transform.localScale.x* .6f, transform.localScale.y*1.2f, transform.localScale.z); // attenuation factor	
	    }
	    //float newAlpha = c.a * alphaDecay;
	    //renderer.material.color = new Color(c.r, c.g, c.b, newAlpha);
	}
		
	branchLoc = transform.localPosition + 
	    transform.up.normalized*1.5f * transform.localScale.y;		
		
	//			Vector3 translateAmount = branchLoc.localEulerAngles;
	//		branchLoc.Translate(translateAmount.normalized * branchLoc.localScale.y / 2f);
	
		
	depthLevel = _depthLevel;
	isRoot = false;
	readyToBranch = false;
    }
	
    public bool Branch() {
		
	if (branched || !readyToBranch) {
	    return false;
	}
			
	if (depthLevel != 0 && isRoot) {
	    throw new System.Exception("Cannot branch before calling Init() on TreeSegment");
	}

	if (depthLevel >= maxDepth) {
	    //	    renderer.material.color = new Color(0, 1f, 0, renderer.material.color.a); //DEVEL
	    return false;
	}
	int childDepth = depthLevel + 1;
	if (branch1 == null) { // if no first branch
	    branch1 = Instantiate(this, branchLoc, transform.rotation)
		as TreeSegment;
	    branch1.Init(childDepth, 
			 Random.Range(minBranchAngle, maxBranchAngle),
			 sizeScalar);
	}
	if (branch2 == null) {
	    branch2 = Instantiate(this, branchLoc, transform.rotation)
		as TreeSegment;
	    branch2.Init(childDepth, 
			 Random.Range(minBranchAngle, maxBranchAngle) * -1f,
			 sizeScalar);
	}
		
	branched = true;
	//	renderer.material.color = new Color(139 / 255f, 100 / 255f, 19 / 255f, renderer.material.color.a); //DEVEL
		
	return true;
    }
			
    void OnMouseUp() {
	Branch();
    }
}
