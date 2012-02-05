using UnityEngine;
using System.Collections;

public class UpdateMeshCollider : MonoBehaviour {

    public float interval = 0.05f; // seconds
    float nextUpdateTime = 0;

    MeshCollider meshCollider;

    // Use this for initialization
    void Start() {
	nextUpdateTime = Time.time + interval;
	meshCollider = gameObject.AddComponent("MeshCollider") as MeshCollider;
    }
	
    // Update is called once per frame
    void Update () {
	if (Time.time > nextUpdateTime) {
	    nextUpdateTime = Time.time + interval;
	    Destroy(GetComponent<MeshCollider>());
	    if (GetComponent<MeshCollider>() == null) {
		meshCollider = gameObject.AddComponent("MeshCollider") as MeshCollider;
	    }

	    /*
	    meshCollider.sharedMesh = null;
	    meshCollider.sharedMesh = GetComponent<MeshFilter>().mesh;
	    */
	}
    }
}
