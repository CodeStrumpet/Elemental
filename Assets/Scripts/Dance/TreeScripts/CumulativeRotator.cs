using UnityEngine;
using System.Collections;

public class CumulativeRotator : MonoBehaviour {
	
	public float rotationSpeed = 2f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAroundLocal(new Vector3(1.1f, .7f, .3f), rotationSpeed * Time.deltaTime);
	}
}
