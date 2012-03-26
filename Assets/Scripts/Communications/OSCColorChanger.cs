using UnityEngine;
using System.Collections;

public class OSCColorChanger : OSCMessageListener {

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
		print(x);
		renderer.material.color = new Color(System.Convert.ToInt32(message.Values[2]) / 127.0f, 
		                                    renderer.material.color.g, 
		                                    renderer.material.color.b, 
		                                    renderer.material.color.a);
	}

	
	protected override string commandOfInterest() {
		return "cc";
	}
}
