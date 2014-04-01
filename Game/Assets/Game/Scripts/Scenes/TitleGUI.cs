using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {
	public GUISkin custom_skin = null;

	public void OnGUI(){
		GUI.skin = custom_skin;

		int button_width = 150;
		int button_height = 50;
		int half_button_width = button_width / 2;
		int half_screen_width = Screen.width / 2;

		if(GUI.Button(new Rect(half_screen_width - half_button_width, 350, button_width, button_height), "Begin Story")){
			Application.LoadLevel("BattleMap");
		}
		if(GUI.Button(new Rect(half_screen_width - half_button_width, 410, button_width, button_height), "Options")){
			Application.LoadLevel("Options");
		}
		if(GUI.Button(new Rect(half_screen_width - half_button_width, 470, button_width, button_height), "Multiplayer")){
			Application.LoadLevel("Login");
		}
	}
}
