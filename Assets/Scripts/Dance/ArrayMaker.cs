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
	public int numXCubes = 1;
	public int numYCubes = 1;
	public int numZCubes = 1;
	public float padding = 0.0f;
	
	//if true, then instantiated objects are reduced by the amount 
	//of padding. This is useful for keeping centers of instantiated objects
	//at the positions they would be with no padding.
	public bool paddingFromScale = false;

	public Transform[,,] Colliders {
		get { return colliders; }
	}
	private Transform[,,] colliders;

	private float cubeXLen = 0;
	private float cubeYLen = 0;
	private float cubeZLen = 0;

	// Use this for initialization
	public void Start () {
		if (renderer != null) renderer.enabled = false;

		if (numXCubes < 1 || numYCubes < 1 || numZCubes < 1) {
			throw new System.Exception("CubeGroups must be a minimum of 1 across every dimension");
		}

		cubeXLen = transform.localScale.x;
		cubeYLen = transform.localScale.y;
		cubeZLen = transform.localScale.z;
		if (paddingFromScale) {
			cubeXLen -= padding;
			cubeYLen -= padding;
			cubeZLen -= padding;
		}
			
		prefabObject.transform.localScale = new Vector3(cubeXLen, cubeYLen, cubeZLen);		
		
		colliders = new Transform[numXCubes, numYCubes, numZCubes];

		for (int xBoxNum=0; xBoxNum < numXCubes; xBoxNum++) {
			for (int yBoxNum=0; yBoxNum < numYCubes; yBoxNum++) {
				for (int zBoxNum=0; zBoxNum < numZCubes; zBoxNum++) {
					
					GameObject newCube = (GameObject) Instantiate(prefabObject, 
					                                    new Vector3(transform.position.x + (float) xBoxNum * (cubeXLen + padding), 
					                                                transform.position.y + (float) yBoxNum * (cubeYLen + padding), 
					                                                transform.position.z + (float) zBoxNum * (cubeZLen + padding)),
					                                    Quaternion.identity);
					newCube.transform.parent = transform;
					colliders[xBoxNum, yBoxNum, zBoxNum] = newCube.transform;
				}
			}
		}

		SendMessage ("OnAllCollidersCreated", SendMessageOptions.DontRequireReceiver);
	}
}