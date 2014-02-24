using UnityEngine;
using System.Collections;

public class MatchmakerLobbyScript : Photon.MonoBehaviour {

	private PhotonView pv;
	private string room_name = "Room 1";
	//private string room_status = "";
	private int max_players = 2;
	private string max_players_text = "2";
	private Room[] game;
	//maybe remove
	//HACK
	private Vector2 scroll_position;

	public string player_prefab = "NetworkPlayer";
	public Transform spawn_object;

	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	void OnJoinedLobby(){
		//PhotonNetwork.JoinRandomRoom();
		Debug.Log ("Trying to join random room");
	}

	//void OnPhotonRandomJoinFailed() {
	//	PhotonNetwork.CreateRoom(null);
	//}

	void OnJoinedRoom(){
		GameObject myplayer = PhotonNetwork.Instantiate(player_prefab, spawn_object.position, Quaternion.identity, 0);
		//TODO
		
		Camera.main.GetComponent<SmoothLookAt>().target = myplayer.transform;
		Camera.main.GetComponent<SmoothFollow>().target = myplayer.transform;
		//myplayer.GetComponent<PersonController>().isControllable = true;
		pv = myplayer.GetComponent<PhotonView>();
	}

	void OnGUI() {
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joined) {
			//HACK
			bool shoutMarco = GameLogic.playerWhoIsIt == PhotonNetwork.player.ID;
			if (shoutMarco && GUILayout.Button ("Marco!")) {
				pv.RPC ("Marco", PhotonTargets.All);
			}
			if (!shoutMarco && GUILayout.Button("Polo!")) {
				pv.RPC("Polo", PhotonTargets.All);
			}
		}
		if (PhotonNetwork.insideLobby == true) {
			GUI.Box(new Rect(Screen.width / 2.5f, Screen.height / 3f, 400, 550), "");
			GUILayout.BeginArea(new Rect(Screen.width /2.5f, Screen.height / 3f, 400, 500));
			GUI.color = Color.red;
			GUILayout.Box("Lobby");
			GUI.color = Color.white;
			// Layout design
			GUILayout.Label("Room Name:");
			room_name = GUILayout.TextField(room_name);
			GUILayout.Label("Max amount of players 1 - 2:");
			max_players_text = GUILayout.TextField(max_players_text, 2);
			if (max_players_text != "") {
				max_players = int.Parse(max_players_text);
				if (max_players > 2) max_players = 2;
				if (max_players == 0) max_players = 1;
			}else {
				max_players = 1;
			}
			if (GUILayout.Button("Create Room ")) {
				if (room_name != "" && max_players > 0) {
					PhotonNetwork.CreateRoom(room_name, true, true, max_players);
				}
			}
			GUILayout.Space(2);
			scroll_position = GUILayout.BeginScrollView(scroll_position, false, true, GUILayout.Width(400), GUILayout.Height(300));
			foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
				GUI.color = Color.green;
				GUILayout.Box(game.name + " " + game.playerCount + "/" + game.maxPlayers);
				GUI.color = Color.white;
				if (GUILayout.Button("Join Room")) {
					PhotonNetwork.JoinRoom(game.name);
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
	}
}