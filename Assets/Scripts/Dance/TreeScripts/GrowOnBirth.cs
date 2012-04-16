using UnityEngine;
using System.Collections;

public class GrowOnBirth : MonoBehaviour {
	
	public float appearSpeed = 2.0f;
	private float lerpAmt = 0.0f;
	private Vector3 origScale;
	TreeSegment ts;
	
	// Use this for initialization
	void Start () {
		origScale = transform.localScale;
		transform.localScale = Vector3.zero;
		ts = gameObject.GetComponent<TreeSegment>();
	}
	
	// Update is called once per frame
	void Update () {
		if (lerpAmt < 1.0f) {
			lerpAmt = Mathf.Min(lerpAmt + appearSpeed * Time.deltaTime, 1.0f);
			transform.localScale = Vector3.Lerp(Vector3.zero, origScale, lerpAmt);
		}
		else {
			if (! ts.readyToBranch) {
				ts.readyToBranch = true;
			}
		}
	}
}
