using UnityEngine;
using System.Collections;

public class MoveOnPlayerEnter : MonoBehaviour {
    public Vector2 bounds = new Vector2(10, 10);

    void Start () {
	transform.position = GetNewPosition();
    }

    Vector3 GetNewPosition() {
	Vector2 halfBounds = new Vector2(bounds.x * 0.5f,
					 bounds.y * 0.5f);
	return new Vector3(Random.Range(-halfBounds.x, halfBounds.x),
			   0,
			   Random.Range(-halfBounds.y, halfBounds.y));
    }

    void OnTriggerEnter(Collider other) {
	if (other.gameObject.tag == "Player") {
	    transform.position = GetNewPosition();
	}
    }

    void OnDrawGizmos() {
	Gizmos.color = Color.yellow;
	Gizmos.DrawWireCube(Vector3.zero, new Vector3(bounds.x,
						      1.0f,
						      bounds.y));
    }
}
