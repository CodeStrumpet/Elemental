using UnityEngine;
using System.Collections;

public class GrowOnBirthProcedural : MonoBehaviour {
	
	public float appearSpeed = 1.5f;
	public float maxRandGrowthRatio = 0.25f;
	
	private float newAppearSpeed = 1.5f; //don't modify appearSpeed since children instantiate from it 
	private float lerpAmt = 0.0f;
	private Vector3 origScale;
	private ProceduralTreeSegment pts;
	
	// Use this for initialization
	void Start () {
		origScale = transform.localScale;
		transform.localScale = Vector3.zero;
		pts = gameObject.GetComponent<ProceduralTreeSegment>();
		if (pts == null) {
			throw new System.Exception("A ProceduralTreeSegment script must be attached to the GameObject for this script to work");
		}
		
		newAppearSpeed = appearSpeed * Random.Range(1.0f, 1.0f + maxRandGrowthRatio);
	}
	
	// Update is called once per frame
	void Update () {
		if (lerpAmt < 1.0f) {
			lerpAmt = Mathf.Min(lerpAmt + newAppearSpeed * Time.deltaTime, 1.0f); // RG random.range randomgrowthspeed
			transform.localScale = Vector3.Lerp(Vector3.zero, origScale, lerpAmt);
		}
		else {
			if (! pts.readyToBranch) {
				pts.readyToBranch = true;
			}
		}
	}
}
