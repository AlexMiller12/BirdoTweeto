using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelFactory : Singleton<LevelFactory> {
	
//------------------------------------------------------------------------CONSTANTS:

	private const float SPEED_SCALAR = 0.0f; //TODO: do this
	private const float VERT_BLOCK_SPACING = 0.1f; 
	private const float CIRCUMFERENCE_SCALAR = 1.4f;

//---------------------------------------------------------------------------FIELDS:

	public static GameObject notePrefab;
	public static GameObject blockPrefab;
	public static GameObject levelPrefab;

//----------------------------------------------------------------INHERITED METHODS:
	
//--------------------------------------------------------------------------METHODS:

	public static GameObject makeLevel(List<Note> notes) { 
		//TODO: take in argument for playback speed

		GameObject level = Instantiate(levelPrefab, 
		                               new Vector3(0,0,0), 
		                               Quaternion.identity) as GameObject;
		return level;
	}

	public static void makeNoteColumn(Note note, Vector3 postion) {

		//TODO: make note column

	}

	private static List<Note> makeTestSong() {
		List<Note> song = new List<Note>();
		for (int i = 0; i < 20; i++) {
			song.Add(new Note(Notes.C, 0, 1.0f));
		}
		return song;
	}

}
