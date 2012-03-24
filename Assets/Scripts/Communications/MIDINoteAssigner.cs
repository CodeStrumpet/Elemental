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
		print("All Colliders Were Created");
		ArrayMaker arrayMaker = transform.GetComponent<ArrayMaker>();
		//Transform[,,] colliders = arrayMaker.Colliders;
		//int numColliders = colliders.GetLength(0) * colliders.GetLength(1) * colliders.GetLength(2);
		for (int xNum=0; xNum < arrayMaker.numXCubes; xNum++) {
			for (int yNum=0; yNum < arrayMaker.numYCubes; yNum++) {
				for (int zNum = 0; zNum < arrayMaker.numZCubes; zNum++) {
					arrayMaker.Colliders[xNum, yNum, zNum].GetComponent<MIDITrigger>().midiNote = curMIDINote++;
				}
			}
		}
	}
}
