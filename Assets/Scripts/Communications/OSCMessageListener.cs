using UnityEngine;
using System.Collections;

public abstract class OSCMessageListener : MonoBehaviour {
	
	// Use this for initialization
	protected virtual void Start () {
		OSCCommunicator oscCommunicator = GameObject.FindGameObjectWithTag("osc").GetComponent<OSCCommunicator>();
		oscCommunicator.registerOSCReceiver(this.OSCMessageReceiver, this.commandOfInterest());
	}
	
	protected virtual void Destroy () {
		OSCCommunicator oscCommunicator = GameObject.FindGameObjectWithTag("osc").GetComponent<OSCCommunicator>();
		oscCommunicator.unregisterOSCReceiver(this.OSCMessageReceiver);
	}
		
	public abstract void OSCMessageReceiver(OscMessage message);
	protected abstract string commandOfInterest();
	
}
