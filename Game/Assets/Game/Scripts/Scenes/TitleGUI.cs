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
	private GUIStyle story, options, multiplayer;
	
	public Texture2D clicked, highlight;

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
		story = skin.FindStyle("Story");
		options = skin.FindStyle("Options");
		multiplayer = skin.FindStyle("Multiplayer");
	}
	
	public void UpdateSkin(){
	
		Texture2D special = skin.FindStyle("Story").normal.background;
		Texture2D item = skin.FindStyle("Options").normal.background;
		Texture2D attack = skin.FindStyle("Multiplayer").normal.background;

		
		skin.customStyles[0].hover.background = UnitGUI.CombineTextures(special, highlight);
		skin.customStyles[0].active.background = UnitGUI.CombineTextures(special, clicked);

		skin.customStyles[0].onHover.background = UnitGUI.CombineTextures(special, highlight);
		skin.customStyles[0].onActive.background = UnitGUI.CombineTextures(special, clicked);

		skin.customStyles[1].hover.background = UnitGUI.CombineTextures(item, highlight);
		skin.customStyles[1].active.background = UnitGUI.CombineTextures(item, clicked);
		
		skin.customStyles[1].onHover.background = UnitGUI.CombineTextures(item, highlight);
		skin.customStyles[1].onActive.background = UnitGUI.CombineTextures(item, clicked);

		skin.customStyles[2].hover.background = UnitGUI.CombineTextures(attack, highlight);
		skin.customStyles[2].active.background = UnitGUI.CombineTextures(attack, clicked);
		
		skin.customStyles[2].onHover.background = UnitGUI.CombineTextures(attack, highlight);
		skin.customStyles[2].onActive.background = UnitGUI.CombineTextures(attack, clicked);
		
	}
	
	public void OnGUI(){
		GUI.skin = skin;
		if(!button_clicked){
			
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * _button_height / 15)/2), (Screen.height / 2) - 2 * (Screen.height / 12), 64 * _button_height / 15, Screen.height / 12), "Begin Story", story)){
				button_clicked = true;
				which_button_clicked = BEGIN;
				//Application.LoadLevel("BattleMap");
			}
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * _button_height / 15)/2), (Screen.height / 2) - ((Screen.height / 12) - (_button_height/4)), 64 * _button_height / 15, Screen.height / 12), "Options", options)){
				button_clicked = true;
				which_button_clicked = OPTIONS;
				//Application.LoadLevel("Options");
			}
			if(GUI.Button(new Rect((Screen.width / 2) - ((64 * _button_height / 15)/2), (Screen.height / 2) + 2*((Screen.height / 12)/4), 64 * _button_height / 15, Screen.height / 12), "Multiplayer", multiplayer)){
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
