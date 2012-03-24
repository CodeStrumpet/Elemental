/*
 * ArrayMaker is a class designed to make building and 
 * manipulating groups of colliders for the noir Elemental piece 
 * easier.
 * 
 */

using UnityEngine;
using System.Collections;
using System;

public class ArrayMaker : MonoBehaviour {
	public GameObject prefabObject;
	public int numXElements = 1;
	public int numYElements = 1;
	public int numZElements = 1;
	public Vector3 paddingPercent = Vector3.zero;
	
	public Transform[,,] Colliders {
		get { return colliders; }
	}
	private Transform[,,] colliders;

	private float elementXLen = 0;
	private float elementYLen = 0;
	private float elementZLen = 0;

	// Use this for initialization
	public void Start () {
		if (renderer != null) renderer.enabled = false;

		if (numXElements < 1 || numYElements < 1 || numZElements < 1) {
			throw new System.Exception("Arrays must be a minimum of 1 across each x, y, and z dimensions");
		}
		if (paddingPercent.x < 0.0f || paddingPercent.x > 100.0f) {
			throw new System.Exception("paddingPercent must be between 0 and 100");
		}
		if (paddingPercent.y < 0.0f || paddingPercent.y > 100.0f) {
			throw new System.Exception("paddingPercent must be between 0 and 100");
		}
		if (paddingPercent.z < 0.0f || paddingPercent.z > 100.0f) {
			throw new System.Exception("paddingPercent must be between 0 and 100");
		}
		
		elementXLen = prefabObject.transform.localScale.x;
		elementYLen = prefabObject.transform.localScale.y;
		elementZLen = prefabObject.transform.localScale.z;
			
		//prefabObject.transform.localScale = new Vector3(elementXLen, elementYLen, elementZLen);		
		
		colliders = new Transform[numXElements, numYElements, numZElements];

		Vector3 localScale = prefabObject.transform.localScale;
		localScale.x *= 1 - paddingPercent.x / 100.0f;
		localScale.y *= 1 - paddingPercent.y / 100.0f;
		localScale.z *= 1 - paddingPercent.z / 100.0f;
		
		for (int xBoxNum=0; xBoxNum < numXElements; xBoxNum++) {
			for (int yBoxNum=0; yBoxNum < numYElements; yBoxNum++) {
				for (int zBoxNum=0; zBoxNum < numZElements; zBoxNum++) {
					GameObject newElement = (GameObject) Instantiate(prefabObject, 
					                                    new Vector3(transform.position.x + (float) xBoxNum * elementXLen, 
					                                                transform.position.y + (float) yBoxNum * elementYLen, 
					                                                transform.position.z + (float) zBoxNum * elementZLen),
					                                    Quaternion.identity);
					newElement.transform.localScale = localScale;
					newElement.transform.parent = transform;
					colliders[xBoxNum, yBoxNum, zBoxNum] = newElement.transform;
				}
			}
		}

		SendMessage ("OnAllCollidersCreated", SendMessageOptions.DontRequireReceiver);
	}
}