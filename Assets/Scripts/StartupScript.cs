using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupScript : MonoBehaviour {

	//Reference to the DataService script
	public static DataService ds = new DataService("smileDB.db");

	// Use this for initialization
	void Start () {
		// Locks screen orientation to landscape
		Screen.orientation = ScreenOrientation.LandscapeLeft;
	
		//Create new DataService object and set reference
	}

}
