using UnityEngine;
using System.Collections;

public class ColorChangeOnCollision : MonoBehaviour {
	
	public Color collisionColor = Color.red;
	
	void OnCollisionEnter(Collision c) {
		transform.renderer.material.color = collisionColor;
						
	}
}
