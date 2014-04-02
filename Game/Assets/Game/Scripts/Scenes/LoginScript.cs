﻿using UnityEngine;
using Random = UnityEngine.Random;


public class LoginScript : MonoBehaviour
{
	public Vector2 GuiSize = new Vector2(300, 400);
	private string player_name = "";
	private Rect guiCenteredRect;
	public MonoBehaviour componentToEnable;
	public string logintext = "Please Log In";
	private char[] arr = new char[] { '\n', ' ' };
	
	
	public void Awake()
	{
		this.guiCenteredRect = new Rect(Screen.width/2-GuiSize.x/2, Screen.height/2-GuiSize.y/2, GuiSize.x, GuiSize.y);
		
		if (this.componentToEnable == null || this.componentToEnable.enabled)
		{
			Debug.LogError("To use the Login, the ComponentToEnable should be defined in inspector and disabled initially.");
		}

		if (string.IsNullOrEmpty(PhotonNetwork.playerName))
		{
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
			player_name = PhotonNetwork.playerName;
		}
	}
	
	public void OnGUI()
	{
		// Enter-Key handling:
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(player_name))
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
		player_name = GUILayout.TextField(player_name, 20);
		player_name = player_name.TrimStart(arr);
		player_name = player_name.TrimEnd(arr);
		PhotonNetwork.playerName = player_name;
		if (GUI.changed)
		{
			// Save name
			PlayerPrefs.SetString("playername", PhotonNetwork.playerName);
		}
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
		PhotonNetwork.playerName = player_name;
		this.componentToEnable.enabled = true;
		this.enabled = false;
	}
}

