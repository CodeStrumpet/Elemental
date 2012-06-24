using UnityEngine;
using System.Collections;
using OpenNI;

public class KinectParticleEmitter : MonoBehaviour {
		
	public bool freezeParticles = false; //turns off all particle motion
	public bool verbose = false;
	
	private Point3D[] p3d;
	private ParticleEmitter pe;
	private ParticleAnimator pa;
		
	void UpdatePointPositions() {	
		
	}
	
	// Use this for initialization
	void Start () {
	
		getRequiredComponents();
		
		SpoofKinectData();
				
		//num particles should be number of Kinect 3D points
		pe.minEmission = p3d.GetLength(0);
		pe.maxEmission = p3d.GetLength(0);
		
		//TODO: this should be Mathf.Infinity, but this makes particles disappear
		//instead, keeping alive for an hour
		//pe.maxEnergy = Mathf.Infinity;
		//pe.minEnergy = Mathf.Infinity;
		pe.maxEnergy = 3600;
		pe.minEnergy = 3600;
		
		FreezeParticles ();
		pe.emit = false;
		setParticlesToPointCloudPoints();
		pe.Emit();
	}
	
	void getRequiredComponents() {
		pe = gameObject.GetComponent<ParticleEmitter>();
		if (pe == null) {
			throw new System.Exception("This script can only be used if the gameobject has a ParticleEmitter");
		}
		
		pa = gameObject.GetComponent<ParticleAnimator>();
		if (pa == null) {
			throw new System.Exception("This script can only be used if the gameobject has a ParticleAnimator");
		}
	}
	
	void setParticlesToPointCloudPoints() {
		Particle[] particles = pe.particles;
		
		if (verbose) {
			Debug.Log("particles.Length is " + particles.Length);
		}
				
		for (int i=0; i < particles.Length; i++) {
			Vector3 partPosition = new Vector3(p3d[i].X, p3d[i].Y, p3d[i].Z);

			particles[i].position = partPosition;
			
			if (verbose) {
				Debug.Log("particle num " + i + " position is " + partPosition);
			}
		}
		
		particleEmitter.particles = particles;
	}
	
	void SpoofKinectData() {
		p3d = new Point3D[5];
		
		float scalar = 1.0f;
		
		for (int i=0; i < p3d.GetLength(0); i++) {
			p3d[i] = new Point3D(i * scalar, i * scalar, 0);
			
			if (false) {
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		        cube.transform.position = new Vector3(p3d[i].X, p3d[i].Y, p3d[i].Z);
				cube.transform.localScale = Vector3.one * 0.5f;
			}
			
		}
		
		if (freezeParticles) {
			FreezeParticles ();
		}
	}
	
	//set particles to stay in place and accurately depict point-cloud data points
	void FreezeParticles() {
		pe.localVelocity = Vector3.zero;
		pe.worldVelocity = Vector3.zero;
		pa.localRotationAxis = Vector3.zero;
		pa.doesAnimateColor = false;
		pa.rndForce = Vector3.zero;
		pa.sizeGrow = 0;
	}	
	
	void Update() {
		//FakeKinectDataMovement();
		setParticlesToPointCloudPoints();
	}
	
	void FakeKinectDataMovement() {
		for (int i=0; i < p3d.GetLength(0); i++) {
			p3d[i].X += Time.deltaTime * 0.1f;
		}
		
	}
}
