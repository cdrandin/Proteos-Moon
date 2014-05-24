using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {
	public GUISkin skin;
	public Texture2D clicked, highlight;
	public MonoBehaviour componentToEnable;
	public bool resetPlayerName = false;
	private float _buttonWidth = 180;
	private float _buttonHeight = 40;
	private float _halfButtonWidth;
	private float _halfScreenWidth;
	private bool _buttonClicked = false;
	private TitleButton _whichButtonClicked;
	

	private enum TitleButton {NONE, NEWGAME, CONTINUE, MATCHMAKING, OPTIONS}

	void Awake(){
		
		if (this.componentToEnable == null || this.componentToEnable.enabled)
		{
			Debug.LogError("To use the Login, the ComponentToEnable should be defined in inspector and disabled initially.");
		}
		if(resetPlayerName)
		{
			PlayerPrefs.SetString("playername", "");
		}
		PlayerPrefs.SetInt("state", 0);
		_whichButtonClicked = TitleButton.NONE;
		UpdateSkin();

		Ratios();
		
	}
	
	private void Ratios(){
	
		_buttonHeight = (10 * Screen.height) / 203;
		
		_buttonWidth = 64 * _buttonHeight / 15;
		_halfButtonWidth = _buttonWidth / 2;
		
		_halfScreenWidth = Screen.width / 2;

		skin.button.fontSize = (int)_buttonHeight/2;
		
	}
	
	public void UpdateSkin(){
	
		Texture2D normal = skin.button.normal.background;
		skin.button.hover.background = GM.CombineTextures(normal, highlight);
		skin.button.active.background = GM.CombineTextures(normal, clicked);
		
	}
	
	public void OnGUI(){
		GUI.skin = skin;
		if(!_buttonClicked){
			
			if(GUI.Button(new Rect(_halfScreenWidth - _halfButtonWidth, Screen.height - (255*Screen.height / 812), _buttonWidth, _buttonHeight), "New Game")){
				_buttonClicked = true;
				_whichButtonClicked = TitleButton.NEWGAME;
			}
			if(PlayerPrefs.GetInt("state") == 0){
				GUI.enabled = false;
			}
			if(GUI.Button(new Rect(_halfScreenWidth - _halfButtonWidth, Screen.height - (215*Screen.height / 812), _buttonWidth, _buttonHeight), "Continue")){
				_buttonClicked = true;
				_whichButtonClicked = TitleButton.CONTINUE;
			}
			if (!GUI.enabled){
				GUI.enabled = true;
			}
			if(GUI.Button(new Rect(_halfScreenWidth - _halfButtonWidth, Screen.height - (175*Screen.height / 812), _buttonWidth, _buttonHeight), "Matchmaking")){
				_buttonClicked = true;
				_whichButtonClicked = TitleButton.MATCHMAKING;
			}
			if(GUI.Button(new Rect(_halfScreenWidth - _halfButtonWidth, Screen.height - (135*Screen.height / 812), _buttonWidth, _buttonHeight), "Options")){
				_buttonClicked = true;
				_whichButtonClicked = TitleButton.OPTIONS;
			}
		}
		else
		{
			switch(_whichButtonClicked)
			{
			case TitleButton.NEWGAME:
				//Application.LoadLevel(Application.loadedLevel + 2);
				//break;
			case TitleButton.CONTINUE:
				//Application.LoadLevel(Application.loadedLevel + 2);
				//break;
			case TitleButton.OPTIONS:
				//Application.LoadLevel(Application.loadedLevel + 1);
				//break;
			case TitleButton.MATCHMAKING:
				if(PlayerPrefs.GetString("playername") == ""){
					this.componentToEnable = this.GetComponent<LoginScript>();
					this.componentToEnable.enabled = true;
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
