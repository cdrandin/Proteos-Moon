using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {
	public GUISkin skin;
	private int _button_width = 150;
	private int _button_height = 50;
	private float padding;
	private int _half_button_width;
	private int _half_screen_width;
	private int _half_screen_height;
	private int which_button_clicked = 0;
	private const int BEGIN = 0;
	private const int OPTIONS = 1;
	private const int MULTIPLAYER = 2;
	private bool button_clicked = false;
	public bool resetPlayerName = false;
	public MonoBehaviour componentToEnable;
	
	public Texture2D clicked, highlight;

	private enum Style
	{
		header, loading, question, portrait, readybutton, checkmark, leaderinfo, 
		chatstyle 
	}

	void Awake(){
		//_button_height = Screen.height / 12;
		padding  = _button_height/4;
		_button_width = 64 * _button_height / 15;
		_half_button_width = _button_width / 2;
		_half_screen_width = Screen.width / 2;
		_half_screen_height = Screen.height / 2;
		if (this.componentToEnable == null || this.componentToEnable.enabled)
		{
			Debug.LogError("To use the Login, the ComponentToEnable should be defined in inspector and disabled initially.");
		}
		if(resetPlayerName)
		{
			PlayerPrefs.SetString("playername", "");
		}
		UpdateSkin();
	}
	
	public void UpdateSkin(){
	
		Texture2D normal = skin.button.normal.background;

		
		skin.button.hover.background = UnitGUI.CombineTextures(normal, highlight);
		skin.button.active.background = UnitGUI.CombineTextures(normal, clicked);

		skin.button.onHover.background = UnitGUI.CombineTextures(normal, highlight);
		skin.button.onActive.background = UnitGUI.CombineTextures(normal, clicked);
		
	}
	
	public void OnGUI(){
		GUI.skin = skin;
		if(!button_clicked){
			
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * _button_height / 15)/2), (Screen.height / 2) - 2 * (Screen.height / 12), 64 * _button_height / 15, Screen.height / 12), "Begin Story")){
				button_clicked = true;
				which_button_clicked = BEGIN;
				//Application.LoadLevel("BattleMap");
			}
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * _button_height / 15)/2), (Screen.height / 2) - ((Screen.height / 12) - (_button_height/4)), 64 * _button_height / 15, Screen.height / 12), "Options")){
				button_clicked = true;
				which_button_clicked = OPTIONS;
				//Application.LoadLevel("Options");
			}
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * _button_height / 15)/2), (Screen.height / 2) + 2*((Screen.height / 12)/4), 64 * _button_height / 15, Screen.height / 12), "Multiplayer")){
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

	#region Helper Functions
	
	private bool MakeButton(float left, float top, string buttonName, Style index){
		float height = Screen.height / 16;
		return GUI.Button(new Rect(left, top, 64* height / 15, height), buttonName, skin.customStyles[(int)index]);
		
	}
	private bool MakeButton(Rect box, string buttonName, Style index){
		return GUI.Button ( box, buttonName, skin.customStyles[(int)index]);
	}

	#endregion
}
