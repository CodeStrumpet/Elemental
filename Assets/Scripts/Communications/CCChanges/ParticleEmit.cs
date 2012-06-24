using UnityEngine;
using System.Collections;

public class ParticleEmit : OSCMessageListener {
	
	public ParticleEmitter pe;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	protected override void Destroy() {
		base.Destroy();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public override void OSCMessageReceiver(OscMessage message) {

		float x = System.Convert.ToInt32(message.Values[2]) / 127.0f;
		x *= 25;
		if (x > 0) {
			pe.maxSize = x;
			pe.minSize = x;
			pe.Emit();
		}
		
	}

	
	protected override string commandOfInterest() {
		return "cc21";
	}
}
