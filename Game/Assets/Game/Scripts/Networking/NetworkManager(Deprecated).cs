using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.MonoBehaviour {
	public string room_name = "Proteus Moon";
	public string new_room_name = "Bangarang Games";
	public Transform spawn_object;

	// Use this for initialization
	void Start() {
		PhotonNetwork.ConnectUsingSettings("v .1");
	}

	void OnGUI(){
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString());
	}

	void OnJoinedLobby(){
		PhotonNetwork.JoinRoom(room_name);
		Debug.Log ("Trying to join Room Proteus Moon");
	}

	void OnPhotonJoinRoomFailed(){
		PhotonNetwork.CreateRoom(new_room_name);
		Debug.Log("Joining Proteus Moon failed, Creating Bangarang Games room");
	}

	void OnPhotonCreateRoomFailed(){
		PhotonNetwork.JoinRandomRoom();
		Debug.Log("Creating Room Failed, Joining random room");
	}

	void OnJoinedRoom(){
		GameObject myplayer = PhotonNetwork.Instantiate("NetworkPlayer", spawn_object.position, Quaternion.identity, 0);
		Camera.main.GetComponent<SmoothLookAt>().target = myplayer.transform;
		Camera.main.GetComponent<SmoothFollow>().target = myplayer.transform;
	}
	
	// Update is called once per frame
	//void Update() {
	
	//}
}
