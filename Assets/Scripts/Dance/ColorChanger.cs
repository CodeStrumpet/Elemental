using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {
    public Color onColor = Color.blue;
    public Color offColor = Color.yellow;

    void Start () {
	
    }
	
    void Update () {
	
    }

    void OnCollisionEnter(Collision collision) {
	Debug.Log("Collision detected");
	renderer.material.color = onColor;
    }

    void OnCollisionExit(Collision collision) {
	Debug.Log("Collision exited");
	renderer.material.color = offColor;
    }

    void OnTriggerEnter(Collider collider) {
	Debug.Log("Trigger entered");
	renderer.material.color = onColor;
    }

    void OnTriggerExit(Collider collider) {
	Debug.Log("Trigger exited");
	renderer.material.color = offColor;
    }
}
