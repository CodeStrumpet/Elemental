using UnityEngine;
using System.Collections;

public class MeshBlur {
    public MeshBlur()
    {
	myFilter = new int[3, 3];
	width = 3;
	height = 3;
	offset = 0;
	absolute = false;
    }

    public MeshBlur(int width, int height) 
    {
	myFilter = new int[width, height];
	this.width = width;
	this.height = height;
	offset = 0;
	absolute = false;
    }

    // The actual filter array
    public int[,] myFilter { get; set; }
    // Width of the filter box
    public int width { get; set; }
    // Height of the filter box
    public int height { get; set; }
    // Amount to add to the z values
    public int offset { get; set; }
    // Determines if we should take the absolute value prior to clamping
    public bool absolute { get; set; }

    // Applies the filter to the input vertices
    public void applyFilter(Vector3[] verts, int width, int height)
    {
	for (int x = 0; x < width; ++x) {
	    for (int y = 0; y < height; ++y) {
		float zValue = 0;
		float weight = 0;
		int xCurrent = -this.width / 2;

		for (int x2 = 0; x2 < this.width; ++x2) {
		    if (xCurrent + x < width && xCurrent + x >= 0) {
			int yCurrent = -this.height / 2;
			for (int y2 = 0; y2 < this.height; ++y2) {
			    if (yCurrent + y < height && yCurrent + y >= 0) {
				Vector3 v = verts[(yCurrent + y) * width + (xCurrent + x)];
				zValue += myFilter[x2, y2] * v.z;
				weight += myFilter[x2, y2];
			    }
			    ++yCurrent;
			}
		    }
		    ++xCurrent;
		}
		Vector3 vert = verts[y * width + x];
		float meanZ = vert.z;
		if (weight == 0) {
		    weight = 1;
		}
		if (weight > 0) {
		    if (this.absolute) {
			zValue = System.Math.Abs(zValue);
		    }
		    zValue = (zValue / weight) + this.offset;
		    meanZ = zValue;
		}
		vert.z = meanZ;
		verts[y * width + x] = vert;
	    }
	}
    }
}


