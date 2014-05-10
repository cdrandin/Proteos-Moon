using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class QuickConnect : Photon.MonoBehaviour {
	private string playerName = "";
	private string game_version = "1.0";
	public RoomOptions roomOptions;
	void Awake()
	{
		if (this.enabled){
			playerName = PlayerPrefs.GetString("playername");
			if (string.IsNullOrEmpty(playerName))
			{
				PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
				playerName = PhotonNetwork.playerName;
			}
			PhotonNetwork.playerName = playerName;
			PhotonNetwork.ConnectUsingSettings(game_version);
		}
		roomOptions.maxPlayers = 2;
	}
	void OnJoinedLobby(){
		if (this.enabled){
			PhotonNetwork.JoinRandomRoom(null, 2);
		}
	}

	void OnPhotonRandomJoinFailed()
	{
		if (this.enabled){
			PhotonNetwork.CreateRoom(null, roomOptions, null);
		}
	}

	void OnJoinedRoom(){
		if (this.enabled){
			Hashtable player_props = new Hashtable();
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
	}


	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Backspace)){
			PhotonNetwork.LoadLevel(0);
		}
	}
}
