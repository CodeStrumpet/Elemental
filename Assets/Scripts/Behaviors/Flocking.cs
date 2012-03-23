using UnityEngine;
using System.Collections;

public class Flocking : MonoBehaviour {

	
	
	public Vector3 velocity = Vector3.forward;
	public bool wantsToLand = false;
	public bool isLanding = false;
	public bool isLanded = false;
	public bool fixYValue = true;
	public int landLayerMask = 0;
	public float rayDistance = 1000.0f;
	public Vector3 landingPoint = Vector3.zero;
	
	
	// Use this for initialization
	void Start () {
		if (Random.value > 0.8f) {
			wantsToLand = true;	
		}
		
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!animation.isPlaying) {
			if (Random.value > 0.2f) {
				animation.Play();
			}
		}
		

	}
	
	public void addAdditionalUpdateFrameBehavior(Boids boidsController) {
		if (wantsToLand) {
			
    	    RaycastHit hit;

			Vector3 origin = new Vector3(transform.position.x,
			  							   rayDistance,
			  							   transform.position.z);
			
			bool rayHit = Physics.Raycast(origin,
			 							     new Vector3(0, -1, 0),
										      out hit,
										      rayDistance);//, landLayerMask);;
			
			bool foundValidLandingPoint = false;
			if (rayHit) {
				if (hit.point.y > boidsController.desiredLandingRange.x && hit.point.y < boidsController.desiredLandingRange.y) {
					foundValidLandingPoint = true;
					print("FOUND VALID LANDING POINT!!!! " + hit.transform);
				}
			}
			
			if (!isLanding && !isLanded && foundValidLandingPoint) {
				print("Landing Point found");
				landingPoint = hit.point;
				isLanding = true;
				fixYValue = false;
				
			}/* else if (isLanding) {
				if (hit.distance < 0.2f) {
					isLanded = true;	
				}
			} // take off...*/
		}
	}
}
