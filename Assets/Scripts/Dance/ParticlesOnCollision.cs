using UnityEngine;
using System.Collections;

public class ParticlesOnCollision : MonoBehaviour {
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
		//not able to get this to file w/ Kinect mesh. Would prefer vs OnCollisionEnter;
    }

    void OnTriggerExit(Collider collider) {

    }
	
	void OnMouseOver() {
		OnCollisionEnter(new Collision());	
	}
	
	void OnMouseExit() {
		OnCollisionExit(new Collision());
	}
}
