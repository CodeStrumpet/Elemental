using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class ZigfuDepthmapToMeshWithCopyAndBounds : MonoBehaviour
{
    public bool pushX = false;
    public bool pushY = false;
    public bool pushZ = true;
    public float pushBack = 1.0f;

    public float minX = -10000f;
    public float maxX = 10000f;
    public float minY = -10000f;
    public float maxY = 10000f;
    public float minZ = -10000f;
    public float maxZ = 100000f;

    public float interval = 2f; // seconds
    float nextUpdateTime = 0;
    public Transform meshCopyPrefab; // we'll be instantiating these
    Transform[] meshCopies;
    int meshIndex = 0;
    public int totalCopies = 1;

    public Vector3 gridScale = new Vector3(0.01f, 0.01f, 0.01f);
    public bool GenerateNormals = false;
    public bool GenerateUVs = true;

    public Vector2 DesiredResolution = new Vector2(160, 120); // should be a divisor of 640x480
                                                              // and 320x240 is too high (too many vertices)
    int factorX;
    int factorY;

    short[] rawDepthMap;
    float[] depthHistogramMap;
    int XRes;
    int YRes;
    Mesh curMesh;
    MeshFilter meshFilter;

    Vector2[] uvs;
    Vector3[] verts;
    int[] tris;
    Vector3[] pts;

    // Use this for initialization
    void Start()
    {
		// Allocate memory for each mesh copy
		meshCopies = new Transform[totalCopies];
		for (int i = 0; i < totalCopies; i++) {
		    Vector3 pos = new Vector3(0, 0, -1);
		    Transform newMeshObj = Instantiate(meshCopyPrefab, pos, Quaternion.identity) as Transform;
		    meshCopies[i] = newMeshObj;
		}
	
		nextUpdateTime = Time.time + interval;	

        // init stuff
        YRes = ZigInput.Depth.yres;
        XRes = ZigInput.Depth.xres;
        factorX = (int)(XRes / DesiredResolution.x);
        factorY = (int)(YRes / DesiredResolution.y);
        // depthmap data
        rawDepthMap = new short[(int)(XRes * YRes)];

        // the actual mesh we'll use
        
        curMesh = new Mesh();

        meshFilter = (MeshFilter)GetComponent(typeof(MeshFilter));
        meshFilter.mesh = curMesh;

        int YScaled = YRes / factorY;
        int XScaled = XRes / factorX;

        verts = new Vector3[XScaled * YScaled];
        uvs = new Vector2[verts.Length];
        tris = new int[(XScaled - 1) * (YScaled - 1) * 2 * 3];
        pts = new Vector3[XScaled * YScaled];

        CalculateTriangleIndices(YScaled, XScaled);
        CalculateUVs(YScaled, XScaled);
        ZigInput.Instance.AddListener(gameObject);
    }

    void UpdateDepthmapMesh(Mesh mesh)
    {
        if (meshFilter == null)
            return;
        Profiler.BeginSample("UpdateDepthmapMesh");
        mesh.Clear();
        
        // flip the depthmap as we create the texture
        int YScaled = YRes / factorY;
        int XScaled = XRes / factorX;
        // first stab, generate all vertices (next time, only vertices for 'valid' depths)
        // first stab, decimate rather than average depth pixels
	//Debug.Log("Before UpdateVertices");
        UpdateVertices(mesh, YScaled, XScaled);
        if (GenerateUVs) {
            UpdateUVs(mesh, YScaled, XScaled);
        }
        UpdateTriangleIndices(mesh);
        // normals - if we generate we need to update them according to the new mesh
        if (GenerateNormals) {
            mesh.RecalculateNormals();
        }

        Profiler.EndSample();
    }

    private void UpdateUVs(Mesh mesh, int YScaled, int XScaled)
    {
        Profiler.BeginSample("UpdateUVs");
        mesh.uv = uvs;
        Profiler.EndSample();
    }

    private void CalculateUVs(int YScaled, int XScaled)
    {
        for (int y = 0; y < YScaled; y++) {
            for (int x = 0; x < XScaled; x++) {
                //uvs[y * XScaled + x] = new Vector2((float)x / (float)XScaled,
                //                       (float)y / (float)YScaled);
                uvs[y * XScaled + x].x = (float)x / (float)XScaled;
                uvs[y * XScaled + x].y = ((float)(YScaled - 1 - y) / (float)YScaled);
            }
        }
    }
    
    private void UpdateVertices(Mesh mesh, int YScaled, int XScaled)
    {
        int depthIndex = 0;
        Profiler.BeginSample("UpdateVertices");

        Profiler.BeginSample("FillPoint3Ds");
		short maxDepth = 100;
        Vector3 vec = new Vector3();
        Vector3 pt = new Vector3();
	//Debug.Log("YScaled: " + YScaled + " XScaled: " + XScaled);
        for (int y = 0; y < YScaled; y++) {
            for (int x = 0; x < XScaled; x++, depthIndex += factorX) {
                short pixel = rawDepthMap[depthIndex];
                if (pixel == 0) pixel = maxDepth; // if there's no depth,  default to max depth

                // RW coordinates
                pt.x = x * factorX;
                pt.y = y * factorY;
                pt.z = pixel;
                pts[x + y * XScaled] = pt; // in structs, assignment is a copy, so modifying the same variable
                                           // every iteration is okay
            }
            // Skip lines
            depthIndex += (factorY - 1) * XRes;
        }
        Profiler.EndSample();
        Profiler.BeginSample("ProjectiveToRW");
		for (int i = 0; i < pts.Length; i++) {
	    	pts[i].x -= XRes / 2;
	    	pts[i].y = (YRes / 2) - pts[i].y; // flip Y axis in projective
		}
        Profiler.EndSample();
        Profiler.BeginSample("PointsToVertices");
        for (int y = 0; y < YScaled; y++) {
            for (int x = 0; x < XScaled; x++) {
                pt = pts[x + y * XScaled];
                vec.x = pt.x * gridScale.x;
                vec.y = pt.y * gridScale.y;
                vec.z = -pt.z * gridScale.z;

		int index = y * XScaled + x;
		if (vec.x > minX && vec.x < maxX &&
		    vec.y > minY && vec.y < maxY &&
		    vec.z > minZ && vec.z < maxZ) {
		    verts[index] = vec;
		} else {
		    float vel = pushBack * Time.deltaTime;
		    if (pushX) {
			verts[index].x -= vel;
		    } 
		    if (pushY) {
			verts[index].y -= vel;
		    }
		    if (pushZ) {
			verts[index].z -= vel;
		    }
		}
            }
        }
        Profiler.EndSample();
        Profiler.BeginSample("AssignVerticesToMesh");
        mesh.vertices = verts;
        Profiler.EndSample();

        Profiler.EndSample();
    }

    private void UpdateTriangleIndices(Mesh mesh)
    {
        Profiler.BeginSample("UpdateTriangleIndices");

        mesh.triangles = tris;
        Profiler.EndSample();
    }

    private void CalculateTriangleIndices(int YScaled, int XScaled)
    {
        int triIndex = 0;
        int posIndex = 0;
        for (int y = 0; y < (YScaled - 1); y++) {
            for (int x = 0; x < (XScaled - 1); x++, posIndex++) {
                // Counter-clockwise triangles

                tris[triIndex++] = posIndex + 1; // bottom right
                tris[triIndex++] = posIndex; // bottom left
                tris[triIndex++] = posIndex + XScaled; // top left

                tris[triIndex++] = posIndex + 1; // bottom right
                tris[triIndex++] = posIndex + XScaled; // top left
                tris[triIndex++] = posIndex + XScaled + 1; // top right
            }
            posIndex++; // finish row
        }
    }

    void LateUpdate() {

		rawDepthMap = ZigInput.Depth.data;
		UpdateDepthmapMesh(curMesh);

		if (Time.time > nextUpdateTime && totalCopies > 0) {
		    nextUpdateTime = Time.time + interval;

		    meshIndex = (meshIndex + 1) % totalCopies;
		    Mesh newMesh = meshCopies[meshIndex].GetComponent<MeshFilter>().mesh;
	    	UpdateDepthmapMesh(newMesh);
		}
    }
}
