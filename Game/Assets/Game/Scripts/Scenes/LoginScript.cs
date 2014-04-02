using UnityEngine;
using Random = UnityEngine.Random;


public class LoginScript : MonoBehaviour
{
	public Vector2 GuiSize = new Vector2(300, 400);
	public static string username = string.Empty;
	
	private Rect guiCenteredRect;
	public MonoBehaviour componentToEnable;
	public string logintext = "Please Log In";
	private const string UserNamePlayerPref = "Guest";
	
	
	public void Awake()
	{
		this.guiCenteredRect = new Rect(Screen.width/2-GuiSize.x/2, Screen.height/2-GuiSize.y/2, GuiSize.x, GuiSize.y);
		
		if (this.componentToEnable == null || this.componentToEnable.enabled)
		{
			Debug.LogError("To use the Login, the ComponentToEnable should be defined in inspector and disabled initially.");
		}
		
		string prefsName = PlayerPrefs.GetString(LoginScript.UserNamePlayerPref);
		if (!string.IsNullOrEmpty(prefsName))
		{
			LoginScript.username = prefsName;
		}
	}
	
	public void OnGUI()
	{
		// Enter-Key handling:
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(LoginScript.username))
			{
				this.ConnectToLobby();
				return;
			}
		}
		GUI.skin.label.wordWrap = true;
		
		GUILayout.BeginArea(guiCenteredRect);
		
		GUILayout.Label(this.logintext);
		
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("NameInput");
		LoginScript.username = GUILayout.TextField(LoginScript.username);
		if (GUILayout.Button("Connect", GUILayout.Width(80)))
		{
			this.ConnectToLobby();
		}
		GUI.FocusControl("NameInput");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Back To Main Menu"))
		{
			Application.LoadLevel("TitleScene");
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void ConnectToLobby()
	{
		PlayerPrefs.SetString(LoginScript.UserNamePlayerPref, LoginScript.username);
		this.componentToEnable.enabled = true;
		this.enabled = false;
	}
}

