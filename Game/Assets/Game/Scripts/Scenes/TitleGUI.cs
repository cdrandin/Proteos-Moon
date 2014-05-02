using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {
	public GUISkin skin;
	private int button_width = 150;
	private int button_height = 50;
	private int half_button_width;
	private int half_screen_width;
	private int half_screen_height;
	private int which_button_clicked = 0;
	private const int BEGIN = 0;
	private const int OPTIONS = 1;
	private const int MULTIPLAYER = 2;
	private bool button_clicked = false;
	public bool reset_playerprefs = false;
	public MonoBehaviour componentToEnable;

	public void Start(){
		half_button_width = button_width / 2;
		half_screen_width = Screen.width / 2;
		half_screen_height = Screen.height / 2;
		if (this.componentToEnable == null || this.componentToEnable.enabled)
		{
			Debug.LogError("To use the Login, the ComponentToEnable should be defined in inspector and disabled initially.");
		}
		if(reset_playerprefs)
		{
			PlayerPrefs.SetString("playername", "");
		}
	}
	public void OnGUI(){
		GUI.skin = skin;
		if(!button_clicked){
			if(GUI.Button(new Rect(half_screen_width - half_button_width, half_screen_height - 2 * button_height, button_width, button_height), "Begin Story")){
				button_clicked = true;
				which_button_clicked = BEGIN;
				//Application.LoadLevel("BattleMap");
			}
			if(GUI.Button(new Rect(half_screen_width - half_button_width, half_screen_height - 40, button_width, button_height), "Options")){
				button_clicked = true;
				which_button_clicked = OPTIONS;
				//Application.LoadLevel("Options");
			}
			if(GUI.Button(new Rect(half_screen_width - half_button_width, half_screen_height + 20, button_width, button_height), "Multiplayer")){
				button_clicked = true;
				which_button_clicked = MULTIPLAYER;
				//Application.LoadLevel("Login");
			}
		}
		else
		{
			switch(which_button_clicked)
			{
			case BEGIN:
				Application.LoadLevel("BattleMap");
				break;
			case OPTIONS:
				Application.LoadLevel("Options");
				break;
			case MULTIPLAYER:
				if(PlayerPrefs.GetString("playername") == ""){
					//this.GetComponent
					this.componentToEnable = this.GetComponent<LoginScript>();
					this.componentToEnable.enabled = true;
					//LoginScript.
				}
				else
				{
					PhotonNetwork.playerName = PlayerPrefs.GetString("playername");
					this.componentToEnable = this.GetComponent<LobbyScript>();
					this.componentToEnable.enabled = true;
					this.enabled = false;
				}
				break;
			default:
				break;
			}
		}
	}
}
