using UnityEngine;
using System.Collections;

class Point {
    public Vector2 v;
    public float width;
};

public class TreeMaker : MonoBehaviour {

    public Transform particlePrefab;

    float dotSize = 9;
    float angleOffsetA;
    float angleOffsetB;

    float prob1 = 0.05f;
    float prob2 = 0.07f;

    ArrayList points;
    LineRenderer lineRenderer;

    void Start() {
	points = new ArrayList();
	
	angleOffsetA = Mathf.Deg2Rad * 1.5f;
	angleOffsetB = Mathf.Deg2Rad * 50f;
	// Draw the tree
	seed(dotSize, Mathf.Deg2Rad * 270.0f, 0, 0, prob1);
	drawTree();
    }
	
    void drawTree() {
	lineRenderer = gameObject.AddComponent<LineRenderer>();
	transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
	transform.Translate(0, 0, -100);
	lineRenderer.useWorldSpace = false;
	lineRenderer.SetVertexCount(points.Count);
	lineRenderer.SetWidth(1.0f, 1.0f);
	for (int i = 0; i < points.Count; i++) {
	    Point p = points[i] as Point;
	    Vector2 v = p.v;
	    Transform t = Instantiate(particlePrefab, new Vector3(v.x, v.y, 0), Quaternion.identity) as Transform;
	    t.localScale = new Vector3(p.width, p.width, p.width);
	    t.parent = transform;
	    //lineRenderer.SetPosition(i, new Vector3(v.x, v.y, 0));
	}
    }

    void seed(float dotSize, float angle, float x, float y, float prob) {
	if (dotSize < 3f) {
	    return;
	}
	float r = Random.value;
	if (r > prob) {
	    Point p = new Point();
	    p.v = new Vector2(x, y);
	    p.width = dotSize;
	    points.Add(p);
	    float newX = x + Mathf.Cos(angle) * dotSize;
	    float newY = y - Mathf.Sin(angle) * dotSize;
	    seed(dotSize * 0.99f, angle - angleOffsetA, newX, newY, prob);
	} else {
	    Point p = new Point();
	    p.v = new Vector2(x, y);
	    p.width = dotSize;
	    points.Add(p);
	    float newX = x + Mathf.Cos(angle);
	    float newY = y - Mathf.Sin(angle);
	    float otherProb = (prob == prob1) ? prob2 : prob1;
	    seed(dotSize * 0.99f, angle + angleOffsetA, newX, newY, otherProb); 
	    seed(dotSize * 0.60f, angle + angleOffsetB, newX, newY, prob);
	    seed(dotSize * 0.50f, angle - angleOffsetB, newX, newY, otherProb);
	}
    }

    void Update() {
	
    }
}
