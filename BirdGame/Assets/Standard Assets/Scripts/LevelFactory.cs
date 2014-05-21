using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelFactory : Singleton<LevelFactory> {
	
//------------------------------------------------------------------------CONSTANTS:

	private const float SPEED_SCALAR = 0.0f; //TODO: do this
	private const float VERT_BLOCK_SPACING = 0.1f; 
	private const float CIRCUMFERENCE_SCALAR = 1.4f;

//---------------------------------------------------------------------------FIELDS:

	public GameObject notePrefab;
	public GameObject blockPrefab;
	public GameObject levelPrefab;

//----------------------------------------------------------------INHERITED METHODS:
	
//--------------------------------------------------------------------------METHODS:

	public GameObject makeLevel(List<Note> notes, int notesPerColumn) { 
		//TODO: take in argument for playback speed

		float totalBeats = Note.getTotalBeats(notes);
		float circumference = totalBeats * CIRCUMFERENCE_SCALAR;
		float radius = circumference / (2.0f * Mathf.PI);

		GameObject level = Instantiate(levelPrefab, 
		                               new Vector3(0,0,0), 
		                               Quaternion.identity) as GameObject;
		level.transform.localScale = new Vector3(radius, 
		                                         notesPerColumn/2.0f, 
		                                         radius); //TODO: check

		foreach (Note note in notes) {

			GameObject noteColumn = makeNoteColumn(note);
			noteColumn.transform.parent = level.transform;
			noteColumn.transform.position = new Vector3(0, 0, radius);
			level.transform.Rotate(Vector3.up * (360.0f/ totalBeats));

		}


		return level;
	}

	public GameObject makeNoteColumn(Note note) {

		GameObject noteColumn = new GameObject();

		float y = 0.0f;
		for (int i = 0; i < 22; i++) {
			int noteValue = i % 13;
			int octave = i / 13;

			GameObject prefab;

			if ((int)note.note == noteValue && note.octave == octave) {
				prefab = notePrefab;
			} 
			else {
				prefab = blockPrefab;
			}

			y = i + i * VERT_BLOCK_SPACING;
			GameObject piece = Instantiate(prefab, 
			                    		   new Vector3(0,y,0), 
			                    		   Quaternion.identity) as GameObject;
			piece.transform.parent = noteColumn.transform;
		}

		return noteColumn;
	}

	public static List<Note> makeTestSong() {
		List<Note> song = new List<Note>();
		for (int i = 0; i < 20; i++) {
			song.Add(new Note(Notes.C, 1, 1.0f));
		}
		return song;
	}

}
