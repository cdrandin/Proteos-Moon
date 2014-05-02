using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Preloader : Photon.MonoBehaviour
{
	public GUISkin skin;
	//private GUIStyle loading;
	/*void Awake(){
		loading = skin.FindStyle("Loading");
	}*/
	
	void Update()
	{
		
		//Wait for both mainmenu&game scene to be loaded (since mainmenu doesnt check on game scene)
		if (Application.GetStreamProgressForLevel(1) >= 1 && Application.GetStreamProgressForLevel(2) >= 1 && Application.GetStreamProgressForLevel(3) >= 1)
			PhotonNetwork.LoadLevel(1);
		
	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 12, 140, 25), "Loading: " + (int)(Application.GetStreamProgressForLevel(3) * 100)+"%");
	}
	
}
