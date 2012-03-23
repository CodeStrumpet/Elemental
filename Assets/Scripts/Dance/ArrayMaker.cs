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
	public float x_offset = 0;
	public float y_offset = 0;
	public float z_offset = 0;
	public int num_x_cubes = 1;
	public int num_y_cubes = 1;
	public int num_z_cubes = 1;
	private float cube_group_x_len = 1;
	private float cube_group_y_len = 1;
	private float cube_group_z_len = 1;

	public Transform[,,] Colliders {
		get { return colliders; }
	}
	private Transform[,,] colliders;

	private float cube_x_len = 0;
	private float cube_y_len = 0;
	private float cube_z_len = 0;

	// Use this for initialization
	public void Start () {
		if (renderer != null) renderer.enabled = false;

		if (num_x_cubes < 1 || num_y_cubes < 1 || num_z_cubes < 1) {
			throw new System.Exception("CubeGroups must be a minimum of 1 across every dimension");
		}

		cube_group_x_len = transform.localScale.x;
		cube_group_y_len = transform.localScale.y;
		cube_group_z_len = transform.localScale.z;

		cube_x_len = cube_group_x_len / (float) num_x_cubes;
		print(cube_x_len);
		cube_y_len = cube_group_y_len / (float) num_y_cubes;		
		cube_z_len = cube_group_z_len / (float) num_z_cubes;		
		prefabObject.transform.localScale = new Vector3(cube_x_len, cube_y_len, cube_z_len);		

		colliders = new Transform[num_x_cubes, num_y_cubes, num_z_cubes];

		for (int x_box_num=0; x_box_num < num_x_cubes; x_box_num++) {
			for (int y_box_num=0; y_box_num < num_y_cubes; y_box_num++) {
				for (int z_box_num=0; z_box_num < num_z_cubes; z_box_num++) {
					GameObject newCube = (GameObject) Instantiate(prefabObject, 
					                                    new Vector3((float) x_box_num * cube_x_len + x_offset, 
					                                                (float) y_box_num * cube_y_len + y_offset, 
					                                                (float) z_box_num * cube_z_len + z_offset),
					                                    Quaternion.identity);
					newCube.transform.parent = transform;
					ColliderCoordinates coordinates = newCube.GetComponent(typeof(ColliderCoordinates)) as ColliderCoordinates;
					coordinates.Coordinates = new Vector3 (x_box_num, y_box_num, z_box_num);
					colliders[x_box_num, y_box_num, z_box_num] = newCube.transform;
				}
			}
		}

		SendMessage ("OnAllCollidersCreated", SendMessageOptions.DontRequireReceiver);
	}

	// Update is called once per frame
	void Update () {

	}

}