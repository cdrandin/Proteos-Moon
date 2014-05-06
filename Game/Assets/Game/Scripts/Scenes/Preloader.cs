using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Preloader : Photon.MonoBehaviour
{
    public GUISkin skin;
	private GUIStyle preloader;
	void Awake(){
		preloader = skin.FindStyle("Preloader");
	}
    void Update()
    {
        // Checks if the next scene can be loaded
        if (Application.GetStreamProgressForLevel(1) >= 1)
            Application.LoadLevel(1);
    }

    void OnGUI()
    {
		// Displays the percentage loaded for the next scene
        GUI.Label(new Rect(Screen.width / 2 - 128, Screen.height / 2 - 25, 256, 50), "Loading: " + (int)(Application.GetStreamProgressForLevel(1) * 100)+"%", preloader);
    }

}
