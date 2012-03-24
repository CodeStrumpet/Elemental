using UnityEngine;
using System.Collections;

public class MIDITrigger : MonoBehaviour {
	
	public int midiNote = 88;
	public float minSecsBeforeRetrigger = 0.0f;

	private OSCSender oscSender;
	private float secsUntilCanRetrigger = 0.0f;

	// Use this for initialization
	void Start () {
		//have to get the OSCSender with a tag if we want this script to be
		//part of a prefab
		GameObject oscSenderObject = GameObject.FindWithTag("osc");
		oscSender = oscSenderObject.GetComponent(typeof(OSCSender)) as OSCSender;
	}
	
	// Update is called once per frame
	void Update () {
		//print("Updating");
		if (secsUntilCanRetrigger > 0.0f) {
			secsUntilCanRetrigger -= Time.deltaTime;
			secsUntilCanRetrigger = Mathf.Max(0.0f, secsUntilCanRetrigger);
		}
	}
	
	void OnTriggerEnter() {
		//print("Trigger Entered");
		
		if(secsUntilCanRetrigger == 0.0f) {
			oscSender.SendNoteOn(midiNote);
			secsUntilCanRetrigger = minSecsBeforeRetrigger;
		}
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