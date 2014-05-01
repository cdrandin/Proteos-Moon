﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class WaitingRoomScript : Photon.MonoBehaviour {
	
	public GUISkin skin;
	public bool forceStart = false;
	public Texture2D mena_texture;
	public Texture2D seita_texture;
	public ProteusChat proteusChat;
	public GameObject mena, seita;
	public GUIStyle header, loading, question, portrait, readyButton;
	private bool leader_chosen = false;
	public bool gameReady = false;
	private bool animatinglabels = false;
	private int leaderClicked = 0;
	private float labelHeight;
	private float startTime;
	private float timer;
	public int counter;
	public GameObject leftSpawn, rightSpawn, magic;
	// Use this for initialization
	void Start () {
		proteusChat = this.GetComponent<ProteusChat>();
		header = skin.FindStyle("Header");
		loading = skin.FindStyle("Loading");
		question = skin.FindStyle("Question");
		portrait = skin.FindStyle ("Portrait");
		readyButton = skin.FindStyle("ReadyButton");
		startTime = 0.0f;
		counter = 0;
		//leftSpawn = GameObject.Find("Left Spawn");
		//rightSpawn = GameObject.Find("Right Spawn");
		//menaPV = GameObject.Find("Captain_Mena_R").GetPhotonView();
		//seitaPV = GameObject.Find("Altier_Seita_R").GetPhotonView();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			forceStart = !forceStart;
		}
		startTime += Time.deltaTime;
		if (startTime >= 0.5f){
			startTime -= 0.5f;
			if (counter == 3)
				counter = 0;
			else
				counter++;
		}
	}
	
	void OnGUI(){
		if (!Application.CanStreamedLevelBeLoaded(3) ||  !Application.CanStreamedLevelBeLoaded(2) || Application.GetStreamProgressForLevel(2) < 1 || Application.GetStreamProgressForLevel(3) < 1)
		{
			GUI.skin = skin; 
			LoadingGUI();
			return;
		}
		GUI.skin = skin;
		if (PhotonNetwork.playerList.Length <= 1 && !forceStart)
			WaitingForOtherPlayer();
		else if ((PhotonNetwork.playerList.Length == 2 && !leader_chosen) || (forceStart && !leader_chosen))
			MainGUI();
		else if (animatinglabels){
			AnimateLabels();
			if (labelHeight == 35.0f)
				animatinglabels = false;
		}
		else if ((PhotonNetwork.playerList.Length == 2 && leader_chosen && !gameReady) || (forceStart && leader_chosen && !gameReady))
			ReadyGUI();
		else
			LoadingGUI();
	}

	void WaitingForOtherPlayer(){
		switch (counter){
		case 0: GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 256, 50), "Waiting For Other Player...", loading);
			break;
		case 1:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 256, 50), "Waiting For Other Player", loading);
			break;
		case 2:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 256, 50), "Waiting For Other Player.", loading);
			break;
		case 3:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 256, 50), "Waiting For Other Player..", loading);
			break;
		default:
			Debug.LogError("The counter in WaitingForOtherPlayer went out of bounds");
			break;
		}
	}

	void MainGUI(){
		GUI.Label(new Rect(100, 30, Screen.width - 200, 50), "Choose Your Leader", header);
		GUI.Label(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 + 50, 256, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 2 + 55, 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, Screen.height / 2 + 50, 256, 50), PhotonNetwork.otherPlayers[0].name, loading);
		if (leaderClicked == 0){
			GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "?", question);
		}
		GUI.Box(new Rect(Screen.width / 2 + 100, Screen.height / 2 - 256, 256, 256), "?", question);
		if(GUI.Button(new Rect(Screen.width / 2 - (256 + 105), Screen.height / 2 + 100, 256 / 2, 256 / 2), mena_texture, portrait)){
			leaderClicked = 1;
		}
		
		if(GUI.Button(new Rect((Screen.width / 2 - (256 + 95)) + (256 / 2), Screen.height / 2  + 100, 256 / 2, 256 / 2), seita_texture, portrait)){
			leaderClicked = 2;
		}
		if (leaderClicked != 0){
			if(GUI.Button(new Rect(Screen.width / 2 - (256 + 105), Screen.height / 2 + 240, 128, 50), "Ready?", readyButton)){
				animatinglabels = true;
				if (PhotonNetwork.inRoom){
					proteusChat.photonView.RPC("GameChat", PhotonTargets.All, "Ready");
					this.photonView.RPC("ActivateOtherPlayer", PhotonTargets.Others);
				}
				Instantiate(magic, leftSpawn.transform.position, Quaternion.identity);
				if (leaderClicked == 1){
					mena.SetActive(true);

				}
				else if (leaderClicked == 2){
					seita.SetActive(true);
				}
				leader_chosen = true;
			}
			if (leaderClicked == 1){
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "MENA", question);
			}
			if (leaderClicked == 2){
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "SEITA", question);
			}
		}
	}

	void ReadyGUI(){
		GUI.Label(new Rect(Screen.width / 2 - (256 + 100), 30, 256, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, 35, 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, 30, 256, 50), PhotonNetwork.otherPlayers[0].name, loading);

		WaitingForOtherPlayer();
	}

	void LoadingGUI()
	{
		GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 12, 140, 25), "Loading: " + (int)(Application.GetStreamProgressForLevel(2) * 100) + "%", loading);
		Application.LoadLevel(Application.loadedLevel + 1);
	}

	void AnimateLabels(){
		labelHeight = Mathf.Lerp((Screen.height / 2 + 50), 35, (Time.time - startTime) / 3.5f);
		GUI.Label(new Rect(Screen.width / 2 - (256 + 100), labelHeight, 256, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, (labelHeight+5), 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, labelHeight, 256, 50), PhotonNetwork.otherPlayers[0].name, loading);
	}

	/*void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting){
			stream.SendNext(otherMena);
			stream.SendNext(otherSeita);
		}
	}*/
	
	[RPC]
	void ActivateOtherPlayer(PhotonMessageInfo mi){
		gameReady = true;
	}
}
