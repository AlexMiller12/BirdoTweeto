using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	
//----------------------------------------------------------------INHERITED METHODS:
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
//--------------------------------------------------------------------------METHODS:
	
	private Notes getNote(float frequency) {

		//frequency of second A on piano
		while (frequency > 53.45655)   frequency /= 2;	
	
		if (frequency > 50.4563)   return Notes.G_SHARP;
		if (frequency > 47.6244)   return Notes.G;		
		if (frequency > 44.9514)   return Notes.F_SHARP;
		if (frequency > 42.4284)   return Notes.F;
		if (frequency > 40.0471)   return Notes.E;
		if (frequency > 37.7995)   return Notes.D_SHARP;
		if (frequency > 35.6779)   return Notes.D; 
		if (frequency > 33.6755)   return Notes.C_SHARP;
		if (frequency > 31.7854)   return Notes.C;
		if (frequency > 30.8677)   return Notes.B;
		if (frequency > 28.3176)   return Notes.A_SHARP;
		if (frequency > 26.682)   return Notes.A;
		
		return Notes.A;
		
		//Else, we have a really low frequency
		//Debug.Log("Really low frequency!");
		//return getNote(frequency * 2);
	}
	
}
