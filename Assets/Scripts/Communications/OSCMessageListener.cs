using UnityEngine;
using System.Collections;

public class OSCMessageListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
		OSCCommunicator oscCommunicator = GameObject.FindGameObjectWithTag("osc").GetComponent<OSCCommunicator>();
		oscCommunicator.registerOSCReceiver(this.OSCMessageReceiver, this.commandOfInterest());
	}
	
	void Destroy () {
		OSCCommunicator oscCommunicator = GameObject.FindGameObjectWithTag("osc").GetComponent<OSCCommunicator>();
		oscCommunicator.unregisterOSCReceiver(this.OSCMessageReceiver);
	}
		
	void OSCMessageReceiver(OscMessage oscMessage) {
		//throw new System.Exception("Unregister me DAMMIT!!!");
		print("I CONQUERED OSC!!!");
	}
	
	string commandOfInterest() {return "cc";}
}
