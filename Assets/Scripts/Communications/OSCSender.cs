using UnityEngine;
using System.Collections;

public class OSCSender : MonoBehaviour {

	private Osc oscHandler;
	
	public string remoteIp;
	public int senderPort;
	public int listenerPort; 
	
	public delegate void MidiEventReceiver(string status, int byte1, int byte2);
	
	public MidiEventReceiver midiEventReceiver;
	private int sceneChange = -1;	
	
	~OSCSender() {
		print("Destructor called");
		if (oscHandler != null) {
			oscHandler.Cancel();
		}
		
		oscHandler = null;
		System.GC.Collect();
	} 
	
	// Use this for initialization
	void Start () {
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init(remoteIp, senderPort, listenerPort);
		
		oscHandler = GetComponent<Osc>();
		oscHandler.init(udp);
		oscHandler.SetAddressHandler("/acw", OSCCallback);
	}
	
	// Update is called once per frame
	void Update () {
		if (sceneChange != -1) {
			Application.LoadLevel(sceneChange);
			sceneChange = -1;
		}
	}
	
	void onDisable() {
		oscHandler.Cancel();
		oscHandler = null;
	}
	
	public void SendNoteOn(int noteNum) {
		
		OscMessage oscM = Osc.StringToOscMessage("/noteon " +  noteNum + " 120");
		oscHandler.Send(oscM);
	}
	
	public void SendNoteOff(int noteNum) {
		OscMessage oscM = Osc.StringToOscMessage("/noteoff " + noteNum);
		oscHandler.Send(oscM);
	}
	
	//DEVEL: get rid of this
	private void moveSphere() {
		//oscBallsGO.transform.Translate(1.0f, 1.0f, 1.0f);
	}
	
	public void OSCCallback(OscMessage m) {
		//print("----------> OSC example message received: (" + m + ")");
		string osc_report_string = "";
		
		string command = (string) m.Values[0];
		/*
		for (int i = 0; i < m.Values.Count; i++) {
			osc_report_string = osc_report_string + "Values[" + i + "]: " + m.Values[i] + "***";
		}
		print("osc_report_string: " + osc_report_string + "\n");
		*/
		if(command == "midievent") {
			midiEventReceiver((string)m.Values[1], (int)m.Values[2], (int)m.Values[3]);
		}
		else if (command == "scenechange") {
			Debug.Log("SCENE CHANGE!!!!!!!!!");
			//Application.LoadLevel((int) m.Values[1]);
			sceneChange = (int) m.Values[1];
		}
	}	
}