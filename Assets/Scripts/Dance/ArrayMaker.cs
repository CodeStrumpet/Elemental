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
		
		//area given to each element of the array. Elements will not use all the area if
		//padding is set
		Vector3 elementLocalScale = new Vector3(prefabObject.transform.localScale.x * transform.localScale.x,
		                          prefabObject.transform.localScale.y * transform.localScale.y,
		                          prefabObject.transform.localScale.z * transform.localScale.z);
		Vector3 elementLen = elementLocalScale;
		elementLocalScale.x *= 1 - paddingPercent.x / 100.0f;
		elementLocalScale.y *= 1 - paddingPercent.y / 100.0f;
		elementLocalScale.z *= 1 - paddingPercent.z / 100.0f;
				
		colliders = new Transform[numXElements, numYElements, numZElements];
		
		for (int xBoxNum=0; xBoxNum < numXElements; xBoxNum++) {
			for (int yBoxNum=0; yBoxNum < numYElements; yBoxNum++) {
				for (int zBoxNum=0; zBoxNum < numZElements; zBoxNum++) {
					GameObject newElement = (GameObject) Instantiate(prefabObject, 
					                                    new Vector3(transform.position.x + (float) xBoxNum * elementLen.x, 
					                                                transform.position.y + (float) yBoxNum * elementLen.y, 
					                                                transform.position.z + (float) zBoxNum * elementLen.z),
					                                    Quaternion.identity);
					newElement.transform.localScale = elementLocalScale;
					newElement.transform.parent = transform;
					colliders[xBoxNum, yBoxNum, zBoxNum] = newElement.transform;
				}
			}
		}

		SendMessage ("OnAllCollidersCreated", SendMessageOptions.DontRequireReceiver);
	}
}