using UnityEngine;
using System.Collections;

public class Boids : MonoBehaviour {
	
	public Boid boid;
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
	
	private Boid[] boidsarray;
	//private Vector3[] boidsvelocity;
	private Vector3 flockLeaderVelocity;
	
	// Use this for initialization
	void Start () {

		boidsarray = new Boid[number_of_boids];
		//boidsvelocity = new Vector3[number_of_boids];
		if (!stationaryCenter) {
			center = Vector3.zero;	
		}
		else {
			center = transform.position;
		}
				
		for (int i=0; i < number_of_boids; i++)
		{
			Boid b = Instantiate(boid, new Vector3( Random.value, Random.value, Random.value), Quaternion.identity) as Boid; 
			b.transform.parent = transform;
			b.transform.localScale = Vector3.one;
			boidsarray[i] = b; 
			b.velocity = flockInitialVelocity;
		}	
		
		if (highlightLeader) {
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.parent = boidsarray[0].transform;
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
			//boidsarray[0].rotation = Quaternion.LookRotation(boidsvelocity[0]);
			//boidsarray[0].Translate(Vector3.forward * Time.deltaTime * speed);
		}
		
		int startingIndex = flockHasLeader ? 1 : 0;
		if (!stationaryCenter) {
			center = Vector3.zero;
			for (int i = startingIndex; i < number_of_boids; i++)
			{		
				center += boidsarray[i].transform.position / number_of_boids;
			}	
		}
		
		Debug.Log(center.x + "," + center.y + "," + center.z);

		for (int i = startingIndex; i < number_of_boids; i++) {
			// print(boidsvelocity[i]);
			if (flockHasLeader) {
				boidsarray[i].velocity = getLeaderAttractor(i);
			} else {
				boidsarray[i].velocity += getCenterAttractor(i); // boids will all fly towards center
			}
			boidsarray[i].velocity += getRepulsion(i); // repulse from nearby boids
			boidsarray[i].velocity += matchVelocity(i); // match velocity
			boidsarray[i].velocity += addScatter(i); // Scatter

			if (maintainConstantHeight) {
				boidsarray[i].velocity.y = 0.0f;
			}

			//boidsvelocity[i].Normalize();
			boidsarray[i].transform.rotation = Quaternion.LookRotation(boidsarray[i].velocity);
			boidsarray[i].transform.Translate(Vector3.forward * speed * Time.deltaTime);
			boidsarray[i].transform.Rotate(Vector3.up * 90.0f);
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
				perceivedVelocity += boidsarray[i].velocity;
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
				if ((boidsarray[i].transform.position - boidsarray[boidIndex].transform.position).sqrMagnitude < somethreshold)
				{
					repulsion += boidsarray[boidIndex].transform.position - boidsarray[i].transform.position;
				}
			}
		}
		
		return repulsion * Time.deltaTime * repulsionFactor;
	}

	Vector3 getCenterAttractor(int boidIndex)
	{
		// RULE THE FIRST!!
		// for boid at boidIndex in boidsarray, returns vector3 distance to center
			
		return (center - boidsarray[boidIndex].transform.position) * cohesionFactor * Time.deltaTime;
	}
	
	Vector3 getLeaderAttractor(int boidIndex) {
		return (boidsarray[0].transform.position - boidsarray[boidIndex].transform.position) * cohesionFactor * Time.deltaTime;	
	}
}
