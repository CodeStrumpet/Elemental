using UnityEngine;
using System.Collections;

public class GrowOnLand : MonoBehaviour {
    public enum State {
	Setup,
	OffLand,
	Growing,
	OnLand,
	Shrinking
    };
    public Transform landMesh; // probably colliderMesh
    public float rayDistance = 20.0f;
    public int frameSkip = 5;
    public bool debugRaycast = true;

    State state = State.Setup;
    int frameCount = 0;

    void Transition(State newState) {
	Debug.Log("Transitioning to " + newState);
	state = newState;
    }

    void Setup() {
	Transition(State.OffLand);
    }
    
    void Update () {
	switch (state) {
	case State.Setup:
	    Setup();
	    break;
	case State.OffLand:
	    if (RaycastToLand()) {
		Transition(State.Growing);
	    }
	    break;
	case State.Growing:
	    RaycastToLand();
	    break;
	case State.OnLand:
	    break;
	case State.Shrinking:
	    break;
	}
    }

    bool RaycastToLand() {
	if (!DoRaycast()) {
	    return false;
	}
	int layer = landMesh.gameObject.layer;
	int layerMask = 1 << layer;
	RaycastHit hit;

	Vector3 origin = new Vector3(transform.position.x,
				     rayDistance,
				     transform.position.z);
	bool rayHit = Physics.Raycast(origin,
				      new Vector3(0, -1, 0),
				      out hit,
				      rayDistance, layerMask);
	if (rayHit) {
	    if (debugRaycast) {
		Debug.DrawRay(origin, new Vector3(0, -rayDistance, 0), Color.green);
	    }
	    Vector3 newPos = new Vector3(transform.position.x,
					 hit.point.y,
					 transform.position.z);
	    transform.position = newPos;
	} else {
	    if (debugRaycast) {
		Debug.DrawRay(origin, new Vector3(0, -rayDistance, 0));
	    }
	}
	return rayHit;
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
