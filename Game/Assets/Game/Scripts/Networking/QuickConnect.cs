using UnityEngine;
using System.Collections;

public class QuickConnect : Photon.MonoBehaviour {
	private string playerName = "";
	private string game_version = "1.0";
	public void Awake()
	{
		playerName = PlayerPrefs.GetString("playername");
		if (string.IsNullOrEmpty(playerName))
		{
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
			playerName = PhotonNetwork.playerName;
		}
		PhotonNetwork.playerName = playerName;
		PhotonNetwork.ConnectUsingSettings(game_version);
	}
	void OnJoinedLobby(){
		if(PhotonNetwork.JoinRandomRoom()){
			PhotonNetwork.CreateRoom(null);
		}
	}

	void OnJoinedRoom(){
		ExitGames.Client.Photon.Hashtable player_props = new ExitGames.Client.Photon.Hashtable();
		player_props.Add("Leader", "Altier_Seita");
		PhotonNetwork.player.SetCustomProperties(player_props);
		if (PhotonNetwork.inRoom){
			PhotonNetwork.LoadLevel(Application.loadedLevel + 1);
		}
		else{
			PhotonNetwork.offlineMode = true;
			Application.LoadLevel(Application.loadedLevel + 1);
		}
	}


	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F13)){
			PhotonNetwork.LoadLevel(0);
		}
	}
}
