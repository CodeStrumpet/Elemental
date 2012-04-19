using UnityEngine;
using System.Collections;

public class ParticlesOffOnMouseOver : MonoBehaviour {
	
	public ParticleEmitter pe;
	public Color onColor = Color.red;
	public Color offColor = Color.green;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseEnter() {
		renderer.material.color = onColor;
		Debug.Log("mouse entered");
		pe.emit = false;
	}

	void OnMouseExit() {
		renderer.material.color = offColor;
		Debug.Log("mouse exited");
		pe.emit = true;
	}
}
