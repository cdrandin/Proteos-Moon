using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {
	public GUISkin custom_skin = null;
	private int button_width = 150;
	private int button_height = 50;
	private float padding;
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
	
	public Texture2D clicked, highlight;
	
	public void Start(){
		
		
		
		//button_height = Screen.height / 12;
		padding  = button_height/4;
		button_width = 64 * button_height / 15;
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
		UpdateSkin();
	}
	
	public void UpdateSkin(){
	
		Texture2D normal = custom_skin.button.normal.background;

		
		custom_skin.button.hover.background = UnitGUI.CombineTextures(normal, highlight);
		custom_skin.button.active.background = UnitGUI.CombineTextures(normal, clicked);

		custom_skin.button.onHover.background = UnitGUI.CombineTextures(normal, highlight);
		custom_skin.button.onActive.background = UnitGUI.CombineTextures(normal, clicked);
		
	}
	
	public void OnGUI(){
		GUI.skin = custom_skin;
		if(!button_clicked){
			
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * button_height / 15)/2), (Screen.height / 2) - 2 * (Screen.height / 12), 64 * button_height / 15, Screen.height / 12), "Begin Story")){
				button_clicked = true;
				which_button_clicked = BEGIN;
				//Application.LoadLevel("BattleMap");
			}
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * button_height / 15)/2), (Screen.height / 2) - ((Screen.height / 12) - (button_height/4)), 64 * button_height / 15, Screen.height / 12), "Options")){
				button_clicked = true;
				which_button_clicked = OPTIONS;
				//Application.LoadLevel("Options");
			}
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * button_height / 15)/2), (Screen.height / 2) + 2*((Screen.height / 12)/4), 64 * button_height / 15, Screen.height / 12), "Multiplayer")){
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
