using UnityEngine;
using System.Collections;

public class MIDITrigger : MonoBehaviour {
	
	private OSCSender oscSender;
	public int midiNote = 88;

	// Use this for initialization
	void Start () {
		GameObject oscSenderObject = GameObject.FindWithTag("osc");
		oscSender = oscSenderObject.GetComponent(typeof(OSCSender)) as OSCSender;
	}
	
	// Update is called once per frame
	void Update () {
		//print("Updating");
	}
	
	void OnTriggerEnter() {
		//print("Trigger Entered");
		
		oscSender.SendNoteOn(midiNote);
	}
	
	void OnTriggerExit() {
		oscSender.SendNoteOff(midiNote);
	}
	
	void OnCollisionEnter() {
		//sending to OnTriggerEnter because we should we using triggers instead of collisions. Should
		//switch once we figure out why the fuck trigger's aren't working. Collisions don't seem
		//performant
		this.OnTriggerEnter();
	}
	
	void OnCollisionExit() {
		//see rant in OnCollisionEnter() definition for why this is going to OnTriggerExit
		this.OnTriggerExit();
	}
	
	void OnMouseDown() {
		print("Mouse button pressed");
		
		oscSender.SendNoteOn(midiNote);
	}
	
	void OnMouseUp() {
		oscSender.SendNoteOff(midiNote);
	}
}