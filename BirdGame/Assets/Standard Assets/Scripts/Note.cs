using UnityEngine;
using System.Collections;

public class Note {

//---------------------------------------------------------------------------FIELDS:

	public Notes note;
	public int octave;
	public float beat;

//----------------------------------------------------------------------CONSTRUCTOR:

	public Note(Notes note, int octave, float beat) {

		this.note = note;
		this.octave = octave;
		this.beat = beat;
	}
}
