using UnityEngine;
using System.Collections;

public class FollowLand : MonoBehaviour {
    public Transform landMesh; 
    public float rayDistance = 20.0f;
    public float rayOffset = 0.1f; // offset from zero
    public int frameSkip = 5;
    public bool debugRaycast = false;

    int frameCount = 0;

    void Update() {
	if (DoRaycast()) {
	    RaycastToLand();
	}
    }

    bool RaycastToLand() {
	int layer = landMesh.gameObject.layer;
	int layerMask = 1 << layer;
	RaycastHit hit;

	Vector3 origin = new Vector3(transform.position.x,
				     rayDistance + rayOffset,
				     transform.position.z);
	bool rayHit = Physics.Raycast(origin,
				      new Vector3(0, -1, 0),
				      out hit,
				      rayDistance, layerMask);
	if (rayHit) {
	    Vector3 newPos = new Vector3(transform.position.x,
					 hit.point.y,
					 transform.position.z);
	    transform.position = newPos;
	} else {
	}
	if (debugRaycast) {
	    if (rayHit) {
		DrawDebugRaycast(origin, Color.green);
	    } else {
		DrawDebugRaycast(origin, Color.white);
	    }
	}
	return rayHit;
    }

    void DrawDebugRaycast(Vector3 origin, Color c) {
	Debug.DrawRay(origin,
		      new Vector3(0, -rayDistance, 0),
		      c);
    }

    bool DoRaycast() {
	frameCount++;
	if (frameCount >= frameSkip) {
	    frameCount = 0;
	    return true;
	}
	return false;
    }
}
