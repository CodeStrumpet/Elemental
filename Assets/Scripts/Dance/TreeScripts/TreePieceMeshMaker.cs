using UnityEngine;
using System.Collections;

public class TreePieceMeshMaker : MonoBehaviour
{
	public float startWidth = 1;
	public float endWidth = 1;
	public float height = 1;
	
	private MeshCollider meshCollider;
	
	void Start ()
	{
		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter>().mesh = mesh;
		meshCollider = GetComponent<MeshCollider>();
		
		//pick a random endWidth
		endWidth = 0.8f;
	}

	void Update ()
	{// will define shape of all new segments
		Vector3[] newVertices = new Vector3[5];
		Vector2[] newUV = new Vector2[5];
		int[] newTriangles = new int[6];
		
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear ();
		
		float startMidPoint = startWidth / 2f;
		float endMidPoint = endWidth / 2f; // 70% smaller
		
		// bottom left, bottom right, top left, top right
		newVertices[0] = new Vector3 (-startMidPoint, 0, 0);
		newVertices[1] = new Vector3 (startMidPoint, 0, 0);
		newVertices[2] = new Vector3 (endMidPoint, height, 0);
		newVertices[3] = new Vector3 (-endMidPoint, height, 0);

		newTriangles[0] = 0;
		newTriangles[1] = 1;
		newTriangles[2] = 3;
		
		newTriangles[3] = 1;
		newTriangles[4] = 2;
		newTriangles[5] = 3;
		
		newUV[0] = new Vector2 (0, 0);
		newUV[1] = new Vector2 (0, 1);
		newUV[2] = new Vector2 (1, 1);
		newUV[3] = new Vector2 (1, 0);
		
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
		mesh.RecalculateNormals();
		
		meshCollider.sharedMesh = mesh;
	}
	
	public void FuckShitUp() {
	// hi --RG
		
		
	}
	
}
