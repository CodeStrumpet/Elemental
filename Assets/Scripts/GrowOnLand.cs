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
    public float rayOffset = 0.1f; // offset from zero
    public float growSpeed = 1.0f;
    public float shrinkSpeed = 4.0f;
    public int frameSkip = 5;
    public bool debugRaycast = false;

    State state = State.Setup;
    int frameCount = 0;

    Vector3 origScale;

    void Start() {
	origScale = transform.localScale;
    }

    void Transition(State newState) {
	Debug.Log("Transitioning to " + newState);
	state = newState;
	switch (state) {
	case State.OffLand:
	    transform.localScale = Vector3.zero;
	    break;
	}
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
	    if (DoRaycast() && RaycastToLand()) {
		Transition(State.Growing);
	    }
	    break;
	case State.Growing:
	    Grow();
	    if (Vector3.Distance(transform.localScale,
				 origScale) < 0.001f) {
		Transition(State.OnLand);
	    }
	    RaycastToLand();
	    break;
	case State.OnLand:
	    if (DoRaycast() && !RaycastToLand()) {
		Transition(State.Shrinking);
	    }
	    break;
	case State.Shrinking:
	    Shrink();
	    if (Vector3.Distance(transform.localScale,
				 Vector3.zero) < 0.001f) {
		Transition(State.OffLand);
	    }
	    RaycastToLand();
	    break;
	}
    }

    void Grow() {
	transform.localScale = Vector3.Slerp(transform.localScale,
					    origScale,
					    Time.deltaTime * growSpeed);
    }

    void Shrink() {
	transform.localScale = Vector3.Slerp(transform.localScale,
					    Vector3.zero,
					    Time.deltaTime * shrinkSpeed);
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
