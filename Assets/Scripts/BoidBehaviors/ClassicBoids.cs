using UnityEngine;
using System.Collections;

public class ClassicBoids : MonoBehaviour {
	
	public ClassicBoid boid;
	public int number_of_boids = 10;  
	
	public float cohesionFactor = 2.0f;
	public float repulsionFactor = 0.1f;
	public float velocitySimilarityFactor = 0.01f;

	public float scatterFactor = 0.0f;
	public float speed = 0.05f;

	public bool	maintainConstantHeight = false;
	public bool flockHasLeader = false;
	public bool stationaryCenter = true;
	public bool fixYValue = true;
	
	public Vector3 center = Vector3.zero;
	
	private ClassicBoid[] boidsarray;
	//private Vector3[] boidsvelocity;
	//private Vector3 flockLeaderVelocity;
	
	// Use this for initialization
	void Start () {

		boidsarray = new ClassicBoid[number_of_boids];
		//boidsvelocity = new Vector3[number_of_boids];
		if (!stationaryCenter) {
			center = Vector3.zero;	
		} else {
			center = transform.position;
		}
		
		for (int i=0; i < number_of_boids; i++) {
			ClassicBoid b = Instantiate(boid, new Vector3( Random.value, Random.value, Random.value), Quaternion.identity) as ClassicBoid; 
			b.transform.parent = transform;
			b.transform.localScale = Vector3.one;
			boidsarray[i] = b; 
			//b.velocity = flockInitialVelocity;
		}	
		
		//flockInitialVelocity = Vector3.Normalize(flockInitialVelocity);
		
	}
	
	void Update () {
				
		if (!stationaryCenter) {
			center = Vector3.zero;
			for (int i = 0; i < number_of_boids; i++) {		
				center += boidsarray[i].transform.position / number_of_boids;
			}	
		}
		
		for (int i = 0; i < number_of_boids; i++) {

				boidsarray[i].velocity += getCenterAttractor(i); // boids will all fly towards center
				boidsarray[i].velocity += getRepulsion(i); // repulse from nearby boids
				boidsarray[i].velocity += matchVelocity(i); // match velocity
				boidsarray[i].velocity += addScatter(i); // Scatter
	
				if (fixYValue) {
					boidsarray[i].velocity.y = 0.0f;
				}
	
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
		Vector3 attractorPoint;
		
		if (flockHasLeader) {
			attractorPoint = boidsarray[0].transform.position;
		} else {
			attractorPoint = center;
		}
		
		return (attractorPoint - boidsarray[boidIndex].transform.position) * cohesionFactor * Time.deltaTime;
	}
}
