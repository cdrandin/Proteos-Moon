using UnityEngine;
using System.Collections;

public class WaitingRoomScript : MonoBehaviour {
	
	public GUISkin skin;
	public bool forceStart = false;
	public Texture2D mena_texture;
	public Texture2D seita_texture;
	public ProteusChat proteusChat;
	private bool leader_chosen = false;
	// Use this for initialization
	void Start () {
		proteusChat = this.GetComponent<ProteusChat>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if (!Application.CanStreamedLevelBeLoaded(3) ||  !Application.CanStreamedLevelBeLoaded(2) || Application.GetStreamProgressForLevel(2) < 1 || Application.GetStreamProgressForLevel(3) < 1)
		{
			GUI.skin = skin; 
			LoadingGUI();
			return;
		}

		if (PhotonNetwork.playerList.Length <= 1 && !forceStart)
			WaitingForOtherPlayer();
		else if ((PhotonNetwork.playerList.Length == 2 && !leader_chosen) || (forceStart && !leader_chosen))
			MainGUI();
		else
			LoadingGUI();
	}
	void WaitingForOtherPlayer(){
		GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 12, 250, 40), "Waiting For Other Player...");
	}

	void MainGUI(){
		GUI.Label(new Rect(Screen.width / 2 - (mena_texture.width), Screen.height / 2 + 50, 250, 40), PhotonNetwork.playerName);
		GUI.Label(new Rect(Screen.width / 2 + 200, Screen.height / 2 + 50, 250, 40), PhotonNetwork.otherPlayers[0].name);

		if(GUI.Button(new Rect(Screen.width / 2 - (mena_texture.width + 100), Screen.height / 2 - mena_texture.height, mena_texture.width, mena_texture.height), mena_texture)){
			proteusChat.photonView.RPC("GameChat", PhotonTargets.All, "Ready");
			//print ("You Chose Mena");
			//leader_chosen = true;
		}

		if(GUI.Button(new Rect(Screen.width / 2 + 100, Screen.height / 2 - seita_texture.height, seita_texture.width, seita_texture.height), seita_texture)){
			proteusChat.photonView.RPC("GameChat", PhotonTargets.All, "Ready");
			//print ("You Chose Seita");
			//leader_chosen = true;
		}
	}

	void LoadingGUI()
	{
		GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 12, 140, 25), "Loading: " + (int)(Application.GetStreamProgressForLevel(2) * 100) + "%");
		//Application.LoadLevel(Application.loadedLevel + 1);
	}
}
