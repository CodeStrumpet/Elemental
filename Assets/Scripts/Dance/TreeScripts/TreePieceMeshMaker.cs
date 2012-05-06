using UnityEngine;
using System.Collections;

public class TreePieceMeshMaker : MonoBehaviour
{
	public float endWidthRatio = 1f;
	public float secsBetweenUpdates = .1f;
	
	private MeshCollider meshCollider;
	private float secsUntilUpdate = 0f;
	
	//note: only update the shape from ProceduralTreeSegment
	void Start ()
	{
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		meshCollider = GetComponent<MeshCollider>();
	}

	void Update () {
		
		secsUntilUpdate -= Time.deltaTime;
		
		if (secsUntilUpdate <= 0) {
		
		 // will define shape of all new segments
			Vector3[] newVertices = new Vector3[4];
			Vector2[] newUV = new Vector2[4];
			int[] newTriangles = new int[6];
		
			Mesh mesh = GetComponent<MeshFilter>().mesh;
			mesh.Clear();
		
			float startMidPointXCoord = 0.5f;
			float endMidPointXCoord = 0.5f * endWidthRatio;
		
			// bottom left, bottom right, top left, top right
			newVertices[0] = new Vector3(-startMidPointXCoord, 0, 0);
			newVertices[1] = new Vector3(startMidPointXCoord, 0, 0);
			newVertices[2] = new Vector3(endMidPointXCoord, 1.0f, 0);
			newVertices[3] = new Vector3(-endMidPointXCoord, 1.0f, 0);

			newTriangles[0] = 0;
			newTriangles[1] = 1;
			newTriangles[2] = 3;
		
			newTriangles[3] = 1;
			newTriangles[4] = 2;
			newTriangles[5] = 3;
		
			newUV[0] = new Vector2(1, 0);
			newUV[1] = new Vector2(0, 0);
			newUV[2] = new Vector2(0, 1);
			newUV[3] = new Vector2(1, 1);
		
			mesh.vertices = newVertices;
			mesh.uv = newUV;
			mesh.triangles = newTriangles;
			mesh.RecalculateNormals();
		
			meshCollider.sharedMesh = mesh;
			
			secsUntilUpdate = secsBetweenUpdates;
		}
	}
}
