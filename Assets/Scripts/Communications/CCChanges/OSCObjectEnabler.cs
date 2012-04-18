using UnityEngine;
using System.Collections;

public class OSCObjectEnabler : OSCMessageListener {
	
	public GameObject objToEnable;
	
	// Use this for initialization
	void Start () {
		base.Start();
	}

	// Update is called once per frame
	void Update () {
		base.Destroy();
	}
	
	public override void OSCMessageReceiver(OscMessage message) {

		int x = System.Convert.ToInt32(message.Values[2]);
		print("Got x to instantiate object");
		if (x > 0) Instantiate(objToEnable, objToEnable.transform.position, Quaternion.identity);
	}

	
	protected override string commandOfInterest() {
		return "cc20";
	}
	
	
}
