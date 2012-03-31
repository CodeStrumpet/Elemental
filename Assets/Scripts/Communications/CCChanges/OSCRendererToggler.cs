using UnityEngine;
using System.Collections;

public class OSCRendererToggler : OSCMessageListener {


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
		renderer.enabled = x == 0 ? false : true;
	}

	
	protected override string commandOfInterest() {
		return "cc14";
	}
}
