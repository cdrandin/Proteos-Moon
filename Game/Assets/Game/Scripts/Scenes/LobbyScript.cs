using System.Collections.Generic;
using UnityEngine;
//using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class LobbyScript : Photon.MonoBehaviour
{
	//public Game GameInstance;
	public Rect LobbyRect;  		// set in inspector to position the lobby screen
	public Rect leftToolbar;  		// set in inspector to position the lobby screen
	public GUIStyle network_status_style;
	public string game_version = "1.0";
	private string room_name = "";
	public GameObject objectToActivate;
	
	public void Start()
	{
		CustomTypes.Register();
		leftToolbar = new Rect(leftToolbar.x, leftToolbar.y, leftToolbar.width, Screen.height - leftToolbar.y);
		//this.GameInstance = new Game();
		if (string.IsNullOrEmpty(room_name))
		{
			room_name = "Room" + Random.Range(1, 9999);
		}
		//this.GameInstance.OnStateChangeAction += this.OnStateChanged;
		if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		{
			PhotonNetwork.ConnectUsingSettings(game_version);
		}
	}

	
	
	public void OnApplicationQuit()
	{
		if(PhotonNetwork.connected)
			PhotonNetwork.Disconnect();
		/*if (this.GameInstance != null && this.GameInstance.loadBalancingPeer != null)
		{
			this.GameInstance.Disconnect();
			this.GameInstance.loadBalancingPeer.StopThread();
		}
		this.GameInstance = null;*/
	}
	
	public void Update()
	{
		//this.GameInstance.Service();
		
		// "back" button of phone will quit
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			GUILayout.BeginArea(LobbyRect);
			GUILayout.Label("Are you sure you want to quit?");
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Yes"))
				Application.Quit();
			else if(GUILayout.Button("No"))
				return;
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		} 
	}
	
	public void OnGUI()
	{
		GUI.skin.button.stretchWidth = true;
		GUI.skin.button.fixedWidth = 0;
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
		GUILayout.Label("Lobby Screen");
		GUILayout.Label(string.Format("Players in rooms: {0} looking for rooms: {1}  rooms: {2}", PhotonNetwork.countOfPlayersInRooms, PhotonNetwork.countOfPlayersOnMaster, PhotonNetwork.countOfRooms));
		
		if (GUILayout.Button("Join Random (or create)"))
		{
			print ("random");
			if(!PhotonNetwork.JoinRandomRoom())
				PhotonNetwork.CreateRoom(room_name);
		}
		else if (GUILayout.Button("Create New Game"))
		{
			PhotonNetwork.CreateRoom(room_name);
			StartGame();
		}
		GUILayout.Space(20);
		
		if (GUILayout.Button("Refresh", GUILayout.Width(150)))
		{
			PhotonNetwork.GetRoomList();
		}
		GUILayout.Space(20);
		
		/*GUILayout.Label("Rooms in lobby: " + PhotonNetwork.countOfRooms);
		foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
		{
			if (GUILayout.Button(roomInfo.name + " turn: " + roomInfo.customProperties["t#"]))
			{
				this.GameInstance.OpJoinRoom(roomInfo.name);
			}
		}*/
		
		GUILayout.EndArea();
	}
	
	private void GuiInGame()
	{
		GUILayout.BeginArea(leftToolbar);
		GUI.skin.button.stretchWidth = false;
		GUI.skin.button.fixedWidth = 150;
		
		// we are in a room, so we can access CurrentRoom and it's Players
		GUILayout.Label("In Room: " + PhotonNetwork.room.name);
		/*string interestingPropsAsString = FormatRoomProps();
		if (!string.IsNullOrEmpty(interestingPropsAsString))
		{
			GUILayout.Label("Props: " + interestingPropsAsString);
		}*/

		
		GUILayout.Space(15);

		
		if (GUILayout.Button("Leave (return later)"))
		{
			PhotonNetwork.LeaveRoom();
		}
		GUILayout.EndArea();
	}

	void OnLeftRoom()
	{
		EndGame();
	}

	private void StartGame()
	{
		this.objectToActivate.SetActive(true);
	}

	private void EndGame()
	{
		this.objectToActivate.SetActive(false);
		GameManager.ResetGameState();
	}
	
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
