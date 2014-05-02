using UnityEngine;
using System.Collections;

public class QuickConnect : MonoBehaviour {
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
		if(PhotonNetwork.JoinRandomRoom()){
			PhotonNetwork.CreateRoom(null);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F13)){
			PhotonNetwork.LoadLevel(0);
		}
	}
}
