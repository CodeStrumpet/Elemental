using UnityEngine;
using System.Collections;

public class ProceduralTree : MonoBehaviour {

    public float startWidth = 3;
    public float endWidth = 1;
    public float height = 3;

    void Start() {
	Mesh mesh = new Mesh();
	//gameObject.AddComponent<MeshFilter>();
	GetComponent<MeshFilter>().mesh = mesh;	
    }

    void Update () {
	Vector3[] newVertices = new Vector3[5];
	Vector2[] newUV = new Vector2[5];
	int[] newTriangles = new int[6];

	Mesh mesh = GetComponent<MeshFilter>().mesh;
	mesh.Clear();
	
	float startMidPoint = startWidth / 2;
	float endMidPoint = endWidth / 2;
	newVertices[0] = new Vector3(0, 0, 0);
	newVertices[1] = new Vector3(startWidth, 0, 0);
	newVertices[2] = new Vector3(startMidPoint + endMidPoint, height, 0);	
	newVertices[3] = new Vector3(startMidPoint - endMidPoint, height, 0);	

	newTriangles[0] = 0;
	newTriangles[1] = 1;
	newTriangles[2] = 3;

	newTriangles[3] = 1;
	newTriangles[4] = 2;
	newTriangles[5] = 3;

	newUV[0] = new Vector2(0, 0);
	newUV[1] = new Vector2(1, 0);
	newUV[2] = new Vector2(1, 1);
	newUV[3] = new Vector2(0, 1);

	mesh.vertices = newVertices;
	mesh.uv = newUV;
	mesh.triangles = newTriangles;
    }    
}
