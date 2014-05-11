using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {
	public GUISkin skin;
	public float _button_width;
	public float _button_height;
	//private float padding;
	private float _half_button_width;
	private float _half_screen_width;
	//private float _half_screen_height;
	private TitleButton which_button_clicked;
	private bool button_clicked = false;
	public bool resetPlayerName = false;
	public MonoBehaviour componentToEnable;
	
	public Texture2D clicked, highlight;
	private enum TitleButton {NONE, NEWGAME, CONTINUE, MATCHMAKING, OPTIONS}

	void Awake(){
		//_button_height = Screen.height / 12;
		
		if (this.componentToEnable == null || this.componentToEnable.enabled)
		{
			Debug.LogError("To use the Login, the ComponentToEnable should be defined in inspector and disabled initially.");
		}
		if(resetPlayerName)
		{
			PlayerPrefs.SetString("playername", "");
		}
		PlayerPrefs.SetInt("state", 0);
		which_button_clicked = TitleButton.NONE;
		UpdateSkin();

		Ratios();
		
	}
	
	private void Ratios(){
	
		_button_height = (10 * Screen.height) / 203;
		//padding  = (135 * Screen.height) / 812;
		
		_button_width = 64 * _button_height / 15;
		_half_button_width = _button_width / 2;
		
		_half_screen_width = Screen.width / 2;
		//_half_screen_height = Screen.height / 2;

		skin.button.fontSize = (int)_button_height/2;
		
	}
	
	public void UpdateSkin(){
	
		Texture2D normal = skin.button.normal.background;
		skin.button.hover.background = UnitGUI.CombineTextures(normal, highlight);
		skin.button.active.background = UnitGUI.CombineTextures(normal, clicked);
		
	}
	
	public void OnGUI(){
		GUI.skin = skin;
		Ratios();
		if(!button_clicked){
			
			if(GUI.Button(new Rect(_half_screen_width - _half_button_width, Screen.height - (255*Screen.height / 812), _button_width, _button_height), "New Game")){
				button_clicked = true;
				which_button_clicked = TitleButton.NEWGAME;
			}
			if(PlayerPrefs.GetInt("state") == 0){
				GUI.enabled = false;
			}
			if(GUI.Button(new Rect(_half_screen_width - _half_button_width, Screen.height - (215*Screen.height / 812), _button_width, _button_height), "Continue")){
				button_clicked = true;
				which_button_clicked = TitleButton.CONTINUE;
			}
			if (!GUI.enabled){
				GUI.enabled = true;
			}
			if(GUI.Button(new Rect(_half_screen_width - _half_button_width, Screen.height - (175*Screen.height / 812), _button_width, _button_height), "Matchmaking")){
				button_clicked = true;
				which_button_clicked = TitleButton.MATCHMAKING;
			}
			if(GUI.Button(new Rect(_half_screen_width - _half_button_width, Screen.height - (135*Screen.height / 812), _button_width, _button_height), "Options")){
				button_clicked = true;
				which_button_clicked = TitleButton.OPTIONS;
			}
		}
		else
		{
			switch(which_button_clicked)
			{
			case TitleButton.NEWGAME:
				//Application.LoadLevel(Application.loadedLevel + 2);
				break;
			case TitleButton.CONTINUE:
				//Application.LoadLevel(Application.loadedLevel + 2);
				break;
			case TitleButton.OPTIONS:
				//Application.LoadLevel(Application.loadedLevel + 1);
				break;
			case TitleButton.MATCHMAKING:
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
