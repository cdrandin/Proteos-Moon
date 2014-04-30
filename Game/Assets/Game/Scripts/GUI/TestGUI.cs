using UnityEngine;
using System.Collections;

public class TestGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	string hover; 
	

	void OnGUI(){ 
		
		GUI.Box(new Rect(5, 35, 300, 300), new GUIContent("Box", "this box has a tooltip"));
		GUI.Button(new Rect(50, 55, 100, 20), "No tooltip here");
		GUI.Button(new Rect(80, 80, 100, 20), new GUIContent("I have a tooltip", "The button overrides the box"));
		GUI.Label(new Rect(10, 40, 100, 40), GUI.tooltip);
			
			
	} 

	// Update is called once per frame
	void Update () { 
	}
}
