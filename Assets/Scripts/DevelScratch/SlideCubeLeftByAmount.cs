using UnityEngine;
using System.Collections;

public class SlideCubeLeftByAmount : MonoBehaviour {
	
	public float ratioToSlide;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseUp() {
		transform.renderer.material.color = Color.red;
		Vector3 translationVec = Vector3.right;
		translationVec *= -1.0f; //because left and right are reversed
		translationVec *= transform.localScale.x;
		transform.Translate(translationVec);
	}
}
