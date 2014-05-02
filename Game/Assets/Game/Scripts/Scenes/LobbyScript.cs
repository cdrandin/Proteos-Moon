using System.Collections.Generic;
using UnityEngine;
//using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class LobbyScript : MonoBehaviour
{
	//public Game GameInstance;
	private Rect LobbyRect;  		// set in inspector to position the lobby screen
	public Rect leftToolbar;  		// set in inspector to position the lobby screen
	public GUIStyle network_status_style;
	public GUISkin skin;
	//public Texture tex;
	public string game_version = "1.0";
	private string room_name = "";
	private Vector2 scroll_position;
	//public GameObject objectToActivate;
	//public string loaded_scene;
	public void Start()
	{
		CustomTypes.Register();
		LobbyRect = new Rect(Screen.width/2 - 250, Screen.height/2 - 112, 500, 450);
		leftToolbar = new Rect(leftToolbar.x, leftToolbar.y, leftToolbar.width, Screen.height - leftToolbar.y);

		if (string.IsNullOrEmpty(room_name))
		{
			room_name = "Room" + Random.Range(1, 9999);
		}

		if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		{
			PhotonNetwork.ConnectUsingSettings(game_version);
		}
	}
	
	public void OnGUI()
	{
		GUI.skin = skin;
		// Displays the current networking state
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), network_status_style);

		if(PhotonNetwork.connectionStateDetailed == PeerState.JoinedLobby)
		{
			GuiInLobby();
		}
		else if(PhotonNetwork.connectionStateDetailed == PeerState.Joined)
		{
			GuiInGame();
		}
		else if(PhotonNetwork.connectionStateDetailed == PeerState.Disconnected || PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		{
			if (GUILayout.Button("Connect"))
				PhotonNetwork.ConnectUsingSettings(game_version);
		}
	}
	
	private void GuiInLobby()
	{
		GUILayout.BeginArea(LobbyRect);
		GUILayout.Box("Lobby");
		GUILayout.Label(string.Format("Players in rooms: {0} looking for rooms: {1}  rooms: {2}", PhotonNetwork.countOfPlayersInRooms, PhotonNetwork.countOfPlayersOnMaster, PhotonNetwork.countOfRooms));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Join Random (or create)"))
		{
			if(!PhotonNetwork.JoinRandomRoom()){
				print ("JoinRandom Failed, Creating Room");
				PhotonNetwork.CreateRoom(null);
			}
		}
		if (GUILayout.Button("Create New Game"))
		{
			PhotonNetwork.CreateRoom(room_name);
			//PhotonNetwork.LoadLevel(Application.loadedLevel + 1);
		}
		GUILayout.EndHorizontal();
		//GUILayout.Space(20);
		
		if (GUILayout.Button("Refresh"))
		{
			PhotonNetwork.GetRoomList();
		}
		//GUILayout.Space(20);
		
		scroll_position = GUILayout.BeginScrollView(scroll_position, false, false);
		foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
			//GUI.skin = lobby_skin_alternate;
			GUILayout.Box(game.name + " " + game.playerCount + "/2");
			//GUI.skin = lobby_skin;
			if (GUILayout.Button("Join Room")) {
				PhotonNetwork.JoinRoom(game.name);
				//PhotonNetwork.LoadLevel(Application.loadedLevel + 1);
			}
		}
		GUILayout.EndScrollView();
		
		GUILayout.EndArea();
	}
	
	private void GuiInGame()
	{
		GUILayout.BeginArea(leftToolbar);
		
		// we are in a room, so we can access CurrentRoom and it's Players
		GUILayout.Label("In Room: " + PhotonNetwork.room.name);

		
		if (GUILayout.Button("Leave Room"))
		{
			PhotonNetwork.LeaveRoom();
		}
		if (GUILayout.Button("Back To Main Menu"))
		{
			PhotonNetwork.LeaveRoom();
			Application.LoadLevel(0);
		}
		GUILayout.EndArea();
	}

	void OnJoinedRoom()	
	{ 
		PhotonNetwork.LoadLevel(Application.loadedLevel + 1);
	}
}
