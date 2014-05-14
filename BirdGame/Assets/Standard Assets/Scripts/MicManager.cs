using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MicManager : Singleton<MicManager> {

/*
 * Code adapted from http://forum.unity3d.com/threads/119595-Using-device-microphone-to-interact-with-objects
 */

//------------------------------------------------------------------------CONSTANTS:
	
  	private const int DEFAULT_MAX_FREQUENCY = 44100;    // Recording sampling range
	private const int MAX_PITCH = 24000;
  	private const int SAMPLE_COUNT = 2048;   // Sample Count -- increase for precision
  	private const int CHANNEL = 0;
  	private const float REFVALUE = 0.1f;    // RMS value for 0 dB.
  	private const float THRESHOLD = 0.02f;  // Minimum amplitude to extract pitch
  	private const float ALPHA = 0.05f;      // The alpha for the low pass filter
	public int CLAMP = 160;            		// Used to clamp dB 

//---------------------------------------------------------------------------FIELDS:
	

	private int maxFrequency;
	private int minFrequency;

	public int recordedLength = 50;    // How many previous frames of sound are analyzed.
	public int requiedBlowTime = 4;    // How long a blow must last to be classified as a blow (and not a sigh for instance).
	
	private float rmsValue;            // Volume in RMS //TODO necessary? should be in function!
	private float dbValue;             // Volume in DB
	private float pitchValue;          // Pitch - Hz (is this frequency?)
	private int blowingTime;           // How long each blow has lasted
	private float lowPassResults;      // Low Pass Filter result
	private float peakPowerForChannel; //
	
	private float[] samples;           // Samples
	private float[] spectrum;          // Spectrum
	
	private List<float> dbValues;      // Used to average recent volume.
	private List<float> pitchValues;   // Used to average recent pitch.

//----------------------------------------------------------------INHERITED METHODS:
	
  	public void Start () {

	    samples = new float[SAMPLE_COUNT];
	    spectrum = new float[SAMPLE_COUNT];
	    dbValues = new List<float>();
	    pitchValues = new List<float>();

	 	startMicListener();

  	}

	public void Update () {

	    // If the audio has stopped playing, this will restart the mic play the clip.
	    //startMicListener();
	
	    // Gets volume and pitch values
	    dbValue = getVolume();
		pitchValue = getFrequency();
	 
		//Debug.Log("dbValue = " + dbValue);
		Debug.Log("pitchValue = " + pitchValue);
		Debug.Log("Node = " + getNote(pitchValue));
	    // Runs a series of algorithms to decide whether a blow is occuring.
	    deriveBlow();
  	}
 	
//--------------------------------------------------------------------------METHODS:
  /* Starts the Mic, and plays the audio back in (near) real-time. */
  	private void startMicListener() {
		
		audio.loop = true;
		audio.mute = true;
		
		//Check if there is at least one microphone connected  
        if(Microphone.devices.Length <= 0)  
        {  
            //Throw a warning message at the console if there isn't a mic
            Debug.LogWarning("Microphone not connected!");  //TODO
        }  
		
		Microphone.GetDeviceCaps(null, out minFrequency, out maxFrequency);
		
		//According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
        if(minFrequency == 0 && maxFrequency == 0) {  
	        //...meaning 44100 Hz can be used as the recording sampling rate  
	        maxFrequency = DEFAULT_MAX_FREQUENCY;  
        }  
		
							//(device, loop?, seconds to record, sample rate)
	    audio.clip = Microphone.Start(null, true, 999, maxFrequency);
	
	    // HACK - Forces the function to wait until the microphone has started, before moving onto the play function.
		//TODO start coroutine instead
	   	while ( ! (Microphone.GetPosition(null) > 0)) {;} 
		
		audio.Play();
	
 	}
	
	/* Sets the volume and pitch of most recent input */
	private void deriveBlow() {

	    updateRecords(dbValue, dbValues);
	    updateRecords(pitchValue, pitchValues);
		 	
	    // Find the average pitch in our records (used to decipher against whistles, clicks, etc).
	    float sumPitch = 0;
	
	    foreach (float num in pitchValues)    sumPitch += num;
	    
	    sumPitch /= pitchValues.Count;
    	// Run our low pass filter.
   		lowPassResults = lowPassFilter(dbValue);
	    // Decides whether this instance of the result could be a blow or not.
	    if (lowPassResults > -30 && sumPitch == 0)   blowingTime += 1;	
	    else   blowingTime = 0;

	    // Once enough successful blows have occured over the previous frames (requiredBlowTime), the blow is triggered.
	    if (blowingTime > requiedBlowTime)   Debug.Log("Blowing!");
	    else   Debug.Log("Not Blowing!");		
	}
	
	private float getFrequency() {
	
		int maxN = 0;
		// Gets the sound spectrum.  Each bucket is responsible for FREQ / SAMPLE_COUNT
	    audio.GetSpectrumData(spectrum, CHANNEL, FFTWindow.BlackmanHarris);
	    // Find the highest sample.
	    for (int i = 0; i < SAMPLE_COUNT; i++){
	
	      	if (spectrum[i] > spectrum[maxN] && spectrum[i] > THRESHOLD) {
				
	        	maxN = i; // maxN is the index of max
	      	}
	    }
	    // Pass the index to a float variable
	    float freqN = maxN;
	    // Interpolate index using neighbors
	    if (maxN > 0 && maxN < SAMPLE_COUNT - 1) {
	
	    	float dL = spectrum[maxN - 1] / spectrum[maxN];
	      	float dR = spectrum[maxN + 1] / spectrum[maxN];
	
	      	freqN += 0.5f * (dR * dR - dL * dL); //TODO Math.abs instead of squaring?
	    }
	    // Convert index to frequency
		return freqN * MAX_PITCH / SAMPLE_COUNT;
	}
	
	private float getVolume() {
		float sum = 0, dbValue;
	    // Get all of our samples from the mic.
	    audio.GetOutputData(samples, CHANNEL);
	    // Sums squared samples
	    for (int i = 0; i < SAMPLE_COUNT; i++)    sum += samples[i] * samples[i];
	    // RMS is the square root of the average value of the samples.
	    rmsValue = Mathf.Sqrt(sum / SAMPLE_COUNT);
	    dbValue = 20 * Mathf.Log10(rmsValue / REFVALUE); //TODO why 20?
	    // Clamp it to {clamp} min
	    if (dbValue < -CLAMP)   dbValue = -CLAMP;
		
		return dbValue;
	}

  // Updates a record, by removing the oldest entry and adding the newest value (val).
	private void updateRecords(float val, List<float> record) {

    	if (record.Count > recordedLength)   record.RemoveAt(0);
  
		record.Add(val);
  }

	/* Gives a result based on the peak volume of the record and the previous 
	 * low pass results. */
	private float lowPassFilter(float peakVolume) {
	
    	return ALPHA * peakVolume + (1.0f - ALPHA) * lowPassResults;

  	}
	
	
	//TEMP! for debugging
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
