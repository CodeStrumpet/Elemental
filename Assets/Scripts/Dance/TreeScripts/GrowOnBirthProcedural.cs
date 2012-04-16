using UnityEngine;
using System.Collections;

public class GrowOnBirthProcedural : MonoBehaviour {
	
	public float appearSpeed = 2.0f;
	private float lerpAmt = 0.0f;
	private Vector3 origScale;
	ProceduralTreeSegment pts;
	
	// Use this for initialization
	void Start () {
		origScale = transform.localScale;
		transform.localScale = Vector3.zero;
		pts = gameObject.GetComponent<ProceduralTreeSegment>();
	}
	
	// Update is called once per frame
	void Update () {
		if (lerpAmt < 1.0f) {
			lerpAmt = Mathf.Min(lerpAmt + appearSpeed * Time.deltaTime, 1.0f);
			transform.localScale = Vector3.Lerp(Vector3.zero, origScale, lerpAmt);
		}
		else {
			if (! pts.readyToBranch) {
				pts.readyToBranch = true;
			}
		}
	}
}
