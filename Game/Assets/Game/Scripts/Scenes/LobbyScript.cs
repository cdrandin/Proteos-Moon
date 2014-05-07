using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LobbyScript : MonoBehaviour
{
	public Rect LobbyRect;  		// set in inspector to position the lobby screen
	public GUIStyle network_status_style;
	public GUISkin skin;
	public string game_version = "1.0";
	private string room_name = "";
	private Vector2 scroll_position;
	
	void Start()
	{
		CustomTypes.Register();
		
		LobbyRect = new Rect(Screen.width/2 - 180, Screen.height/2 + 96, 360, 256);
		
		if (string.IsNullOrEmpty(room_name))
		{
			room_name = "Room" + Random.Range(1, 9999);
		}
		
		if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		{
			PhotonNetwork.ConnectUsingSettings(game_version);
		}
	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		// Displays the current networking state
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), network_status_style);
		
		if(PhotonNetwork.connectionStateDetailed == PeerState.JoinedLobby)
		{
			GuiInLobby();
		}
		else if(PhotonNetwork.inRoom){
			StartGame ();
		}
		else if(PhotonNetwork.connectionStateDetailed == PeerState.Disconnected || PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		{
			if (GUILayout.Button("Connect"))
				PhotonNetwork.ConnectUsingSettings(game_version);
		}
	}

	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null, true, true, 2);
	}

	private void GuiInLobby()
	{
		GUILayout.BeginArea(LobbyRect);
		GUILayout.Box(string.Format("Lobby\nPlayers in rooms: {0} looking for rooms: {1}  rooms: {2}", PhotonNetwork.countOfPlayersInRooms, PhotonNetwork.countOfPlayersOnMaster, PhotonNetwork.countOfRooms));
		//GUILayout.Label();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Join Random (or create)"))
		{
			PhotonNetwork.JoinRandomRoom(null, 2);
		}
		if (GUILayout.Button("Create New Game"))
		{
			PhotonNetwork.CreateRoom(room_name, true, true, 2);
		}
		GUILayout.EndHorizontal();
		
		if (GUILayout.Button("Refresh"))
		{
			PhotonNetwork.GetRoomList();
		}
		
		scroll_position = GUILayout.BeginScrollView(scroll_position, false, false);
		foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
			GUILayout.Box(game.name + " " + game.playerCount + "/2");
			if (GUILayout.Button("Join Room")) {
				PhotonNetwork.JoinRoom(game.name);
			}
		}
		GUILayout.EndScrollView();
		
		GUILayout.EndArea();
	}

	void StartGame(){
		PhotonNetwork.LoadLevel(Application.loadedLevel + 1);
	}
}
