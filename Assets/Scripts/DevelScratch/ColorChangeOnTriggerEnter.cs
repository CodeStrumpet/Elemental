using UnityEngine;
using System.Collections;

public class ColorChangeOnTriggerEnter : MonoBehaviour {
	
	public Color collisionColor = Color.yellow;
	
	void OnTriggerEnter(Collider c) {
		transform.renderer.material.color = collisionColor;
		c.renderer.material.color = Color.cyan;				
	}
}
