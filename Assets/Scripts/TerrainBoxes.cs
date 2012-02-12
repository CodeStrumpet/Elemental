using UnityEngine;
using System.Collections;

public class TerrainBoxes : MonoBehaviour {
    public Transform colliderMesh;
    public Vector2 bounds = new Vector2(10, 10);
    public Vector2 res = new Vector2(16, 8);
    public float boxHeight = 1.0f;

    Transform[] colliders;

    void Start () {
	GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);

	Vector2 spacing = new Vector2(bounds.x / res.x,
				      bounds.y / res.y);
	Vector2 boxOffset = new Vector2(-spacing.x * 0.5f,
					-spacing.y * 0.5f);
	Vector2 objOffset = new Vector2(-bounds.x * 0.5f + spacing.x,
					-bounds.y * 0.5f + spacing.y);

	colliders = new Transform[(int)res.x * (int)res.y];

	// Create Boxes
	box.renderer.enabled = false;
	box.transform.localScale = new Vector3(spacing.x, boxHeight, spacing.y);
	FlowObstacle fo = box.AddComponent(typeof(FlowObstacle)) as FlowObstacle;
	GameObject fv = GameObject.Find("FlowVolumes");
	fo.FlowVolumes = fv.transform;
	for (int y = 0; y < res.y; y++) {
	    for (int x = 0; x < res.x; x++) {
		Transform newBox = Instantiate(box.transform,
					       new Vector3(x * spacing.x, 0,
							   y * spacing.y),
					       Quaternion.identity) as Transform;
		newBox.Translate(boxOffset.x, 0, boxOffset.y);
		newBox.parent = transform;
		colliders[y * (int)res.x + x] = newBox;
	    }
	}
	transform.Translate(objOffset.x, 0, objOffset.y);
	GameObject.Destroy(box);
    }
	
    void Update () {
	
    }

    void OnDrawGizmos() {
	Gizmos.color = Color.blue;
	Gizmos.DrawWireCube(Vector3.zero, new Vector3(bounds.x,
						      boxHeight,
						      bounds.y));
    }
}
