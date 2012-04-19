using UnityEngine;
using System.Collections;

public class ParticlesOffOnCollision : MonoBehaviour {
	
	public ParticleEmitter p;
	public Color onColor = Color.green;
	public Color offColor = Color.red;
	
	public float minSecsUntilReemit = 2.0f;
	private float secsUntilReemit;
	
	// Use this for initialization
	void Start () {
		secsUntilReemit = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		secsUntilReemit -= Time.deltaTime;
		
		secsUntilReemit = Mathf.Max(secsUntilReemit, -1f);
		
		if(secsUntilReemit < 0.0f && p.emit == false) {
			renderer.material.color = onColor;
			p.emit = true;
		}
	}
	
	void OnCollisionEnter() {
		//Debug.Log("HEY I COLLIDED!");

		if (secsUntilReemit <= 0.0f) {
			renderer.material.color = offColor;
			p.emit = false;
		}
		
		secsUntilReemit = minSecsUntilReemit;
	}
}
