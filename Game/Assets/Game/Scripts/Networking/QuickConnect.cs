using UnityEngine;
using System.Collections;

public class QuickConnect : Photon.MonoBehaviour {
	private string playerName = "";
	private string game_version = "1.0";
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
			if (!PhotonNetwork.connected)
				PhotonNetwork.ConnectUsingSettings(game_version);
		}
	}
	void OnJoinedLobby(){
		if (this.enabled){
			if(PhotonNetwork.JoinRandomRoom()){
				print ("creating ROOm");
				PhotonNetwork.CreateRoom(null);
			}
		}
	}

	void OnJoinedRoom(){
		if (this.enabled){
			ExitGames.Client.Photon.Hashtable player_props = new ExitGames.Client.Photon.Hashtable();
			player_props.Add("Leader", "Altier_Seita");
			PhotonNetwork.player.SetCustomProperties(player_props);
			if (PhotonNetwork.inRoom){
				PhotonNetwork.LoadLevel(Application.loadedLevel + 1);
			}
			else{
				//PhotonNetwork.offlineMode = true;
				Application.LoadLevel(Application.loadedLevel + 1);
			}
		}
	}


	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F13)){
			PhotonNetwork.LoadLevel(0);
		}
	}
}
