using UnityEngine;
using System.Collections;

public class WaitingRoomScript : MonoBehaviour {
	
	public GUISkin skin;
	public bool forceStart = false;
	public Texture2D mena_texture;
	public Texture2D seita_texture;
	public ProteusChat proteusChat;
	public GameObject mena, seita, otherMena, otherSeita;
	public GUIStyle header, loading, question;
	public PhotonView menaPV, seitaPV;
	private bool leader_chosen = false;
	private bool gameReady = false;
	private bool animatinglabels = false;
	private float labelHeight;
	private float startTime;
	private float timer;
	public int counter;
	public GameObject leftSpawn, rightSpawn, particle;
	// Use this for initialization
	void Start () {
		proteusChat = this.GetComponent<ProteusChat>();
		header = skin.FindStyle("Header");
		loading = skin.FindStyle("Loading");
		question = skin.FindStyle("Question");
		startTime = 0.0f;
		counter = 0;
		leftSpawn = GameObject.Find("Left Spawn");
		rightSpawn = GameObject.Find("Right Spawn");
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
		case 0: GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 250, 50), "Waiting For Other Player...", loading);
			break;
		case 1:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 250, 50), "   Waiting For Other Player", loading);
			break;
		case 2:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 250, 50), "  Waiting For Other Player.", loading);
			break;
		case 3:	GUI.Label(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 250, 50), " Waiting For Other Player..", loading);
			break;
		default:
			Debug.LogError("The counter in WaitingForOtherPlayer went out of bounds");
			break;
		}
	}

	void MainGUI(){
		GUI.Label(new Rect(100, 30, Screen.width - 200, 50), "Choose Your Leader", header);
		GUI.Label(new Rect(Screen.width / 2 - (mena_texture.width + 100), Screen.height / 2 + 50, mena_texture.width, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 2 + 55, 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, Screen.height / 2 + 50, seita_texture.width, 50), PhotonNetwork.otherPlayers[0].name, loading);
		GUI.Box(new Rect(Screen.width / 2 - (mena_texture.width + 100), Screen.height / 2 - mena_texture.height, mena_texture.width, mena_texture.height), "?", question);
		GUI.Box(new Rect(Screen.width / 2 + 100, Screen.height / 2 - seita_texture.height, seita_texture.width, seita_texture.height), "?", question);
		if(GUI.Button(new Rect(Screen.width / 2 - (mena_texture.width + 100), Screen.height / 2 + 100, mena_texture.width / 2, mena_texture.height / 2), mena_texture)){
			//AnimateLabels();
			animatinglabels = true;
			if (PhotonNetwork.inRoom){
				proteusChat.photonView.RPC("GameChat", PhotonTargets.All, "Ready");
				menaPV.RPC ("ActivateOtherPlayer", PhotonTargets.Others, "Mena");
				PhotonNetwork.Instantiate("RuneOfMagic", leftSpawn.transform.position, Quaternion.identity, 0);
			}
			
			mena.SetActive(true);
			//print ("You Chose Mena");
			leader_chosen = true;
		}
		
		if(GUI.Button(new Rect((Screen.width / 2 - (seita_texture.width + 100)) + (seita_texture.width / 2), Screen.height / 2  + 100, seita_texture.width / 2, seita_texture.height / 2), seita_texture)){
			//AnimateLabels();
			animatinglabels = true;
			if (PhotonNetwork.inRoom){
				proteusChat.photonView.RPC("GameChat", PhotonTargets.All, "Ready");
				seitaPV.RPC ("ActivateOtherPlayer", PhotonTargets.Others, "Seita");
				PhotonNetwork.Instantiate("RuneOfMagic", leftSpawn.transform.position, Quaternion.identity, 0);
			}
			seita.SetActive(true);
			//print ("You Chose Seita");
			leader_chosen = true;
		}
	}

	void ReadyGUI(){
		GUI.Label(new Rect(Screen.width / 2 - (mena_texture.width + 100), 30, mena_texture.width, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, 35, 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, 30, seita_texture.width, 50), PhotonNetwork.otherPlayers[0].name, loading);
		WaitingForOtherPlayer();
	}

	void LoadingGUI()
	{
		GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 12, 140, 25), "Loading: " + (int)(Application.GetStreamProgressForLevel(2) * 100) + "%", loading);
		//Application.LoadLevel(Application.loadedLevel + 1);
	}

	void AnimateLabels(){
		labelHeight = Mathf.Lerp((Screen.height / 2 + 50), 35, (Time.time - startTime) / 3);
		GUI.Label(new Rect(Screen.width / 2 - (mena_texture.width + 100), labelHeight, mena_texture.width, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, (labelHeight+5), 50, 50), "VS", header);
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, labelHeight, seita_texture.width, 50), PhotonNetwork.otherPlayers[0].name, loading);
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting){
			stream.SendNext(otherMena);
			stream.SendNext(otherSeita);
		}
	}
	
	[RPC]
	void ActivateOtherPlayer(string character, PhotonMessageInfo mi){
		GameObject spawn = GameObject.Find("Right Spawn");
		if (character == "Mena")
			otherMena.transform.position = spawn.transform.position;
		else if (character == "Seita")
			otherSeita.transform.position = spawn.transform.position;
		else
			Debug.LogError("Error sending leader info");
	}
}
