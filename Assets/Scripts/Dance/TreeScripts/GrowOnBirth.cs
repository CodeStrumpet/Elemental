using UnityEngine;
using System.Collections;

public class GrowOnBirth : MonoBehaviour {
	
	private float appearSpeed = 5.0f;
	private float lerpAmt = 0.0f;
	private Vector3 origScale;
	
	// Use this for initialization
	void Start () {
		origScale = transform.localScale;
		transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (lerpAmt < 1.0f) {
			lerpAmt = Mathf.Min(lerpAmt + appearSpeed * Time.deltaTime, 1.0f);
			transform.localScale = Vector3.Lerp(Vector3.zero, origScale, lerpAmt);
		}
	}
}
