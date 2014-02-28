using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class MatchmakerLobbyScript : Photon.MonoBehaviour {

	//private PhotonView pv;
	private string room_name = "";
	private string player_name = "";
	//private string room_status = "";
	private int max_players = 20;
	private string max_players_text = "2";
	private Room[] game;
	//maybe remove
	//HACK
	private Vector2 scroll_position;
	private float native_width = 1920;
	private float native_height = 1080;
	private char[] arr = new char[] { '\n', ' ' };

	public string player_prefab = "NetworkPlayer";
	public Transform spawn_object;
	public GUIStyle network_status_style;
	public GUISkin lobby_skin;
	public GUISkin lobby_skin_alternate;
	public int lobby_width = 800;
	public int lobby_height = 600;

	// Use this for initialization
	void Start () {
		//PhotonNetwork.ConnectUsingSettings("0.1");
	}

	public void Awake()
	{
		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.automaticallySyncScene = true;
		
		// the following line checks if this client was just created (and not yet online). if so, we connect
		if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		{
			// Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
			PhotonNetwork.ConnectUsingSettings("1.0");
		}
		
		// generate a name for this player, if none is assigned yet
		if (String.IsNullOrEmpty(PhotonNetwork.playerName))
		{
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
			player_name = PhotonNetwork.playerName;
		}
		if (String.IsNullOrEmpty(room_name))
		{
			room_name = "Room" + Random.Range(1, 9999);
		}
		// if you wanted more debug out, turn this on:
		// PhotonNetwork.logLevel = NetworkLogLevel.Full;
	}

	void OnPhotonCreateRoomFailed() {
		try {
			if (room_name != "" && max_players > 0) {
				PhotonNetwork.CreateRoom(room_name, true, true, max_players);
			}
		}
		catch {
			Debug.Log ("Unable to create room");
			Application.LoadLevel("Networking");
		}
	}
	void OnJoinedLobby(){
		//PhotonNetwork.JoinRandomRoom();
		//Debug.Log ("Trying to join random room");
	}

	//void OnPhotonRandomJoinFailed() {
	//	PhotonNetwork.CreateRoom(null);
	//}

	void OnJoinedRoom(){
		GameObject myplayer = PhotonNetwork.Instantiate(player_prefab, spawn_object.position, Quaternion.identity, 0);
		//TODO
		myplayer.GetComponent<ThirdPersonController>().isControllable = true;
		Camera.main.GetComponent<SmoothLookAt>().target = myplayer.transform;
		Camera.main.GetComponent<SmoothFollow>().target = myplayer.transform;
		//myplayer.GetComponent<PersonController>().isControllable = true;
		//pv = myplayer.GetComponent<PhotonView>();
	}

	void OnJoinedRandomRoomFailed(){
		if (room_name != "" && max_players > 0) {
			PhotonNetwork.CreateRoom(room_name, true, true, max_players);
		}
	}

	void OnGUI() {
		GUI.skin = lobby_skin;
		// Scales to match any client's resolution
		float rx = Screen.width / native_width;
		float ry = Screen.height / native_height;
		GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1));
		// Displays the current networking state
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), network_status_style);
		/*if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
			//HACK
			bool shoutMarco = GameLogic.playerWhoIsIt == PhotonNetwork.player.ID;
			if (shoutMarco && GUILayout.Button ("Marco!")) {
				pv.RPC ("Marco", PhotonTargets.All);
			}
			if (!shoutMarco && GUILayout.Button("Polo!")) {
				pv.RPC("Polo", PhotonTargets.All);
			}
		}*/

		if (PhotonNetwork.insideLobby == true) {
			GUI.Box(new Rect((native_width / 2f) - (lobby_width / 2f), (native_height / 2f) - (lobby_height /2f), lobby_width, lobby_height), "");
			GUILayout.BeginArea(new Rect((native_width / 2f) - (lobby_width / 2f), (native_height / 2f) - (lobby_height / 2f), lobby_width, lobby_height - 25));
			GUILayout.Box("Matchmaking Lobby");
			// Layout design
			GUILayout.Label("Player name:");
			player_name = GUILayout.TextField(player_name, 20);
			player_name = player_name.TrimStart(arr);
			player_name = player_name.TrimEnd(arr);
			PhotonNetwork.playerName = player_name;
			GUILayout.Space(2);
			if (GUI.changed)
			{
				// Save name
				PlayerPrefs.SetString("playername", PhotonNetwork.playerName);
			}
			GUILayout.Label("Room Name:");
			room_name = GUILayout.TextField(room_name);
			GUILayout.Label("Max amount of players 1 - 20:");
			max_players_text = GUILayout.TextField(max_players_text, 2);
			if (max_players_text != "") {
				max_players = int.Parse(max_players_text);
				if (max_players > 20) max_players = 20;
				if (max_players == 0) max_players = 1;
			}else {
				max_players = 1;
			}
			if (GUILayout.Button("Create Room ")) {
				if (room_name != "" && max_players > 0 && player_name != "") {
					PhotonNetwork.CreateRoom(room_name, true, true, max_players);
				}
			}
			GUILayout.Space(2);
			if (GUILayout.Button("Join Random"))
			{
				PhotonNetwork.JoinRandomRoom();
			}
			GUILayout.Space(2);
			scroll_position = GUILayout.BeginScrollView(scroll_position, false, true, GUILayout.Width(lobby_width), GUILayout.Height(lobby_height - 100));
			foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
				//GUI.color = Color.green;
				GUI.skin = lobby_skin_alternate;
				GUILayout.Box(game.name + " " + game.playerCount + "/" + game.maxPlayers);
				GUI.skin = lobby_skin;
				//GUI.color = Color.white;
				if (GUILayout.Button("Join Room")) {
					PhotonNetwork.JoinRoom(game.name);
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}
}