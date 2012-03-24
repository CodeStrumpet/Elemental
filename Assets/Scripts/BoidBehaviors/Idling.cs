using UnityEngine;
using System.Collections;

public class Idling : MonoBehaviour {
	
	public bool landed = true;
	public Transform flyingPrefab;
	public float secsUntilTakeoff = 4.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		secsUntilTakeoff -= Time.deltaTime;
		if(secsUntilTakeoff < 0.0f) {
			landed = false;
		}
		
		if (!landed) {
			Debug.Log("FLYING LEADER TIME!!!!!");
			//Destroy(gameObject);
			//Instantiate(flyingPrefab, transform.position, transform.rotation);
		}
	}
}
