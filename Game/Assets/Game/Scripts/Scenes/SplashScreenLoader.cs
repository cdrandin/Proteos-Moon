using UnityEngine;
using System.Collections;

public class SplashScreenLoader : MonoBehaviour {
	public float delayTime = 3;
	
	// Loads the next level after delayTime seconds
	void Start () {
		Invoke ("LoadNextLevel", delayTime);
	}
	
	// Checks if the user wants to skip the splash screen
	void Update () {
		if(Input.anyKey){
			LoadNextLevel();
		}
	}

	// Loads the next level
	void LoadNextLevel(){
		Application.LoadLevel(Application.loadedLevel + 1);
	}

}
