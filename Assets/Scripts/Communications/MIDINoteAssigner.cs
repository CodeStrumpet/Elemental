using UnityEngine;
using System.Collections;

/*This script takes an array of objects with a MIDITrigger
 * component and assigns MIDI notes to them. The first use case
 * is using it alongside the ArrayMaker script to create an array
 * of MIDI triggers.
 * */

public class MIDINoteAssigner : MonoBehaviour {

	public int midiStartNote = 60;
	public int midiAssignerIncrement = 1;
	
	void OnAllCollidersCreated() {
		int curMIDINote = midiStartNote;
		ArrayMaker arrayMaker = transform.GetComponent<ArrayMaker>();
		for (int xNum=0; xNum < arrayMaker.numXElements; xNum++) {
			for (int yNum=0; yNum < arrayMaker.numYElements; yNum++) {
				for (int zNum = 0; zNum < arrayMaker.numZElements; zNum++) {
					arrayMaker.Colliders[xNum, yNum, zNum].GetComponent<MIDITrigger>().midiNote = curMIDINote++;
				}
			}
		}
	}
}
