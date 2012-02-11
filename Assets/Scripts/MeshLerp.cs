using UnityEngine;
using System.Collections;

public class MeshLerp  {
    public MeshLerp(int width, int height) 
    {
	this.width = width;
	this.height = height;
	this.lastVerts = new Vector3[width * height];
    }

    public float speed { get; set; }

    int width;
    int height;
    Vector3[] lastVerts;

    public void applyLerp(Vector3[] verts) 
    {
	for (int x = 0; x < this.width; ++x) {
	    for (int y = 0; y  < this.height; ++y) {
		Vector3 thisV = verts[y * this.width + x];
		Vector3 lastV = lastVerts[y * this.width + x];
		Vector3 newV = Vector3.Lerp(lastV, thisV, this.speed);
		verts[y * this.width + x] = newV;
		lastVerts[y * this.width + x] = newV;
	    }
	}
    }
}
