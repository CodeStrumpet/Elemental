using UnityEngine;
using System.Collections;

public class Landing : MonoBehaviour {
	
	public Vector3 landingPoint;
	public Transform idlePrefab;
	public float speed = 1.0f;
	public bool showLanding = false;
	private GameObject cube;
	
	// Use this for initialization
	void Start () {
		if (showLanding) {
			cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.LookRotation(landingPoint - transform.position);
					
		//transform.rotation = Quaternion.RotateTowards
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
		transform.Rotate(Vector3.up * 90.0f);
				
		if(showLanding) {
			cube.transform.position = landingPoint;
		}
		
		Debug.Log(Vector3.Distance(landingPoint, transform.position));
		if(Vector3.Distance(landingPoint, transform.position) < 0.1f) {
			Vector3 origScale = transform.localScale;
			
			Destroy(gameObject);
			//cube.renderer.material.color = Color.red;
			Destroy(cube);
			transform.rotation = Quaternion.LookRotation(new Vector3(5.0f, 10.0f, 5.0f));
			idlePrefab.transform.localScale = origScale;
			GameObject go = Instantiate(idlePrefab, transform.position, transform.rotation) as GameObject;
		}
	}
}
