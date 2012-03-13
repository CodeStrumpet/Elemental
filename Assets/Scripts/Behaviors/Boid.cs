using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour {
	
	public Vector3 velocity = Vector3.forward;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!animation.isPlaying) {
			if (Random.value > 0.9f) {
				animation.Play();
			}
		}
	}
}
