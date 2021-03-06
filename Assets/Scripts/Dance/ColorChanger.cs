using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {
    public Color onColor = Color.blue;
    public Color offColor = Color.yellow;

    void OnCollisionEnter(Collision collision) {
	//Debug.Log("Collision detected");
		renderer.material.color = onColor;		
    }

    void OnCollisionExit(Collision collision) {
	//Debug.Log("Collision exited");
		renderer.material.color = offColor;
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
