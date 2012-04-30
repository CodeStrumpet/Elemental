using UnityEngine;
using System.Collections;

public class RandomizeTreePieceLen : MonoBehaviour {
	
	public float maxLenAttenutionRatio = 0.25f;
	
	private ProceduralTreeSegment pts;
	
	// Use this for initialization
	void Start () {
		pts = GetComponent<ProceduralTreeSegment>();
		if (pts == null) {
			throw new System.Exception("The GameObject needs a ProceduralTreeSeqment component for this script to work");
		}
	}
	
	public void Initted() {
		transform.localScale = new Vector3(transform.localScale.x, 
		                                   transform.localScale.y * Random.Range(1.0f - maxLenAttenutionRatio, 1.0f),
		                                   transform.localScale.z);
	}
}
