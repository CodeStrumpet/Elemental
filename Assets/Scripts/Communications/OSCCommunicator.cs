using UnityEngine;
using System.Collections;

public class OSCCommunicator : MonoBehaviour {

	private Osc oscHandler;
	
	public string remoteIp = "127.0.0.1";
	public int senderPort = 57000;
	public int listenerPort = 57001; 
	public bool verbose = false; //whether to print out received OSC messages to the console
	
	public delegate void MidiEventReceiver(string status, int byte1, int byte2);
	
	public MidiEventReceiver midiEventReceiver;
	//this is needed because the callback cannot change the scene itself. Only Update() can.
	private int sceneChange = -1;	
	
	~OSCCommunicator() {
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
	
	public void OSCCallback(OscMessage m) {
		if (verbose) {
			print("----------> OSC example message received: (" + m + ")");
		}
		string osc_report_string = "";
		
		string command = (string) m.Values[0];
		
		if (verbose) {
			print("OSC command is " + command);
			for (int i = 0; i < m.Values.Count; i++) {
				osc_report_string = osc_report_string + "Values[" + i + "]: " + m.Values[i] + "***";
			}
			print("osc_report_string: " + osc_report_string + "\n");
		}
		
		if(command == "midievent") {
			midiEventReceiver((string)m.Values[1], (int)m.Values[2], (int)m.Values[3]);
		}
		else if (command == "scenechange") {
			Debug.Log("SCENE CHANGE!!!!!!!!!");
			sceneChange = (int) m.Values[1];
		}
		else if (command == "fx") {
			string messageName = "onFX";
		}
	}	
}