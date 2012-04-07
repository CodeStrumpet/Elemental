using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {
    public Color onColor = Color.blue;
    public Color offColor = Color.yellow;

	public ParticleEmitter p;
	
    void Start () {
		//p = transform.GetComponent<ParticleEmitter>();
		p.emit = false;
    }
	
    void Update () {
	
    }

    void OnCollisionEnter(Collision collision) {
	//Debug.Log("Collision detected");
		renderer.material.color = onColor;		
		p.emit = true;
    }

    void OnCollisionExit(Collision collision) {
	//Debug.Log("Collision exited");
		renderer.material.color = offColor;
		p.emit = false;
    }

    void OnTriggerEnter(Collider collider) {
	//Debug.Log("Trigger entered");
	renderer.material.color = onColor;
    }

    void OnTriggerExit(Collider collider) {
	//Debug.Log("Trigger exited");
	renderer.material.color = offColor;
    }
	
	void OnMouseOver() {
		OnCollisionEnter(new Collision());	
	}
	
	void OnMouseExit() {
		OnCollisionExit(new Collision());
	}
}
