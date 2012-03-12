using UnityEngine;
using System.Collections;

public class Boids : MonoBehaviour {
	
	public Transform boid;
	public int number_of_boids = 10;  	
	public float cohesionFactor = 2.0f;
	public float repulsionFactor = 0.1f;
	public float velocitySimilarityFactor = 0.01f;
	public float scatterFactor = 0.0f;
	public Vector3 flockInitialVelocity = Vector3.forward;
	public float speed = 0.05f;
	public bool	maintainConstantHeight = false;
	public bool flockHasLeader = false;
	public bool highlightLeader = true;
	public bool stationaryCenter = true;
	private Vector3 center = Vector3.zero;
	
	private Transform[] boidsarray;
	private Vector3[] boidsvelocity;
	private Vector3 flockLeaderVelocity;
	
	// Use this for initialization
	void Start () {

		boidsarray = new Transform[number_of_boids];
		boidsvelocity = new Vector3[number_of_boids];
		if (!stationaryCenter) {
			center = Vector3.zero;	
		}
		else {
			center = transform.position;
		}
				
		for (int i=0; i < number_of_boids; i++)
		{
			Transform b = Instantiate(boid, new Vector3( Random.value, Random.value, Random.value), Quaternion.identity) as Transform; 
			b.parent = transform;
			b.transform.localScale = Vector3.one;
			boidsarray[i] = b; 
			boidsvelocity[i] = flockInitialVelocity;
		}	
		
		if (highlightLeader) {
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.parent = boidsarray[0];
			sphere.transform.localScale = Vector3.one * 5.0f;
			sphere.transform.position = sphere.transform.parent.position;
			sphere.renderer.material.color = Color.red;
		}
		
		flockInitialVelocity = Vector3.Normalize(flockInitialVelocity);
		
		if (flockHasLeader) {
			flockLeaderVelocity = flockInitialVelocity;
		}
	}
	
	// Update is called once per frame
	void Update () {
				
		if (flockHasLeader) {
			//update flockLeaderVelocity here
			boidsarray[0].rotation = Quaternion.LookRotation(boidsvelocity[0]);
			boidsarray[0].Translate(Vector3.forward * Time.deltaTime * speed);
		}
		
		int startingIndex = flockHasLeader ? 1 : 0;
		if (!stationaryCenter) {
			center = Vector3.zero;
			for (int i = startingIndex; i < number_of_boids; i++)
			{		
				center += boidsarray[i].position / number_of_boids;
			}	
		}
		
		Debug.Log(center.x + "," + center.y + "," + center.z);

		for (int i = startingIndex; i < number_of_boids; i++) {
			// print(boidsvelocity[i]);
			if (flockHasLeader) {
				boidsvelocity[i] = getLeaderAttractor(i);
			} else {
				boidsvelocity[i] += getCenterAttractor(i); // boids will all fly towards center
			}
			boidsvelocity[i] += getRepulsion(i); // repulse from nearby boids
			boidsvelocity[i] += matchVelocity(i); // match velocity
			boidsvelocity[i] += addScatter(i); // scatter

			if (maintainConstantHeight) {
				boidsvelocity[i].y = 0.0f;
			}

			//boidsvelocity[i].Normalize();
			boidsarray[i].rotation = Quaternion.LookRotation(boidsvelocity[i]);
			
			boidsarray[i].Translate(Vector3.forward * speed * Time.deltaTime);
		}
	}
		
	Vector3 matchVelocity(int boidIndex)
	{
		Vector3 perceivedVelocity = Vector3.zero;
		
		// adjust boid velocity to be nearer to neighboring velocities
		for (int i = 0; i < number_of_boids; i++)
		{
			if (i != boidIndex)
			{
				perceivedVelocity += boidsvelocity[i];
			}
		}
		
		perceivedVelocity /= (number_of_boids - 1);
		return perceivedVelocity * Time.deltaTime * velocitySimilarityFactor;
	}
	
	Vector3 addScatter(int boidIndex)
	{
		// adds noise to position
		return (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)) * scatterFactor;
		
	}
	
	Vector3 getRepulsion(int boidIndex)
	{
	// move away from nearby boids	
		Vector3 repulsion = Vector3.zero;
		int somethreshold = 2; // eh local
		
		for (int i=0; i < number_of_boids; i++)
		{
			if (i != boidIndex) 
			{
				// add to repulsion 
				if ((boidsarray[i].position - boidsarray[boidIndex].position).sqrMagnitude < somethreshold)
				{
					repulsion += boidsarray[boidIndex].position - boidsarray[i].position;
				}
			}
		}
		
		return repulsion * Time.deltaTime * repulsionFactor;
	}

	Vector3 getCenterAttractor(int boidIndex)
	{
		// RULE THE FIRST!!
		// for boid at boidIndex in boidsarray, returns vector3 distance to center
			
		return (center - boidsarray[boidIndex].position) * cohesionFactor * Time.deltaTime;
	}
	
	Vector3 getLeaderAttractor(int boidIndex) {
		return (boidsarray[0].position - boidsarray[boidIndex].position) * cohesionFactor * Time.deltaTime;	
	}
}
