using System.Collections.Generic;
using UnityEngine;
//using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class LobbyScript : MonoBehaviour
{
	private Rect LobbyRect;  		// set in inspector to position the lobby screen
	public Rect leftToolbar;  		// set in inspector to position the lobby screen
	public GUIStyle network_status_style;
	public GUISkin lobby_skin;
	public GUISkin lobby_skin_alternate;
	public string game_version = "1.0";
	private string room_name = "";
	private Vector2 scroll_position;
	public string loaded_scene;
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
		else if(PhotonNetwork.connectionStateDetailed == PeerState.Disconnected)
		{
			if (GUILayout.Button("Connect"))
				PhotonNetwork.ConnectUsingSettings(game_version);
		}
	}
	
	private void GuiInLobby()
	{
		GUILayout.BeginArea(LobbyRect);
		GUI.skin = lobby_skin;
		GUILayout.Box("Lobby");
		GUILayout.Label(string.Format("Players in rooms: {0} looking for rooms: {1}  rooms: {2}", PhotonNetwork.countOfPlayersInRooms, PhotonNetwork.countOfPlayersOnMaster, PhotonNetwork.countOfRooms));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Join Random (or create)"))
		{
			if(PhotonNetwork.JoinRandomRoom()){
				PhotonNetwork.CreateRoom(null);
			}
		}
		if (GUILayout.Button("Create New Game"))
		{
			PhotonNetwork.CreateRoom(room_name);
			StartGame();
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
				StartGame();
			}
		}
		GUILayout.EndScrollView();
		
		GUILayout.EndArea();
	}
	
	private void GuiInGame()
	{
		GUILayout.BeginArea(leftToolbar);
		//GUILayout.Box(tex);
		GUI.skin.button.stretchWidth = false;
		GUI.skin.button.fixedWidth = 150;
		
		// we are in a room, so we can access CurrentRoom and it's Players
		GUILayout.Label("In Room: " + PhotonNetwork.room.name);
		/*string interestingPropsAsString = FormatRoomProps();
		if (!string.IsNullOrEmpty(interestingPropsAsString))
		{
			GUILayout.Label("Props: " + interestingPropsAsString);
		}*/

		
		if (GUILayout.Button("Leave Room"))
		{
			//PhotonNetwork.DestroyPlayerObjects();
			PhotonNetwork.LeaveRoom();
		}
		if (GUILayout.Button("Back To Main Menu"))
		{
			PhotonNetwork.LeaveRoom();
			//PhotonNetwork.Disconnect();
			PhotonNetwork.LoadLevel("TitleScene");
		}
		GUILayout.EndArea();
	}

	/*void OnLeftRoom()
	{
		EndGame();
	}*/

	void OnJoinedRoom()	
	{ 
		StartGame ();
	}
	
	void OnPhotonPlayerConnected(PhotonPlayer newPlayer)	
	{
		//if (instantiatedAvatars == false && PhotonNetwork.countOfPlayers == 2) 		
		//{
			// do something
			
			// close the room
		//}
	}

	private void StartGame()
	{
		PhotonNetwork.LoadLevel(loaded_scene);
	}

	/*private void EndGame()
	{
		//this.objectToActivate.SetActive(false);
		GM.instance.ResetGameManager();
	}*/
	
	/*private string FormatRoomProps()
	{
		Hashtable customRoomProps = this.GameInstance.CurrentRoom.CustomProperties;
		string interestingProps = "";
		foreach (string propName in GameInstance.roomProps)
		{
			if (customRoomProps.ContainsKey(propName))
			{
				if (!string.IsNullOrEmpty(interestingProps)) interestingProps += " ";
				interestingProps += propName + ":" + customRoomProps[propName];
			}
		}
		return interestingProps;
	}
	
	private string RandomCustomRoomProp()
	{
		string[] roomProps = GameInstance.roomProps;
		return roomProps[Random.Range(0, roomProps.Length)];
	}
	
	private string RandomCustomPlayerProp()
	{
		string[] playerProps = GameInstance.playerProps;
		return playerProps[Random.Range(0, playerProps.Length)];
	}*/
}
