using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public static float getTotalBeats(List<Note> notes) {
		float beats = 0.0f;
		foreach (Note note in notes) {
			beats += note.beat;
		}
		return beats;
	}
}
