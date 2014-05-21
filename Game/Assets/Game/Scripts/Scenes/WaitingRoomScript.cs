using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonView))]
public class WaitingRoomScript : Photon.MonoBehaviour {
	
	public GUISkin skin;
	public Texture2D mena_texture, menaBio;
	public Texture2D seita_texture, seitaBio;
	private ProteusChat proteusChat;
	public GameObject mena, seita;
	private GUIStyle header, loading, question, portrait, readyButton, checkmark, leaderInfo;
	public bool gameReady = false;
	private int leaderClicked = 0;
	private float labelHeight;
	private float offSet = 0;
	private GameObject leader, otherLeader;
	private string menaSpecialText, seitaSpecialText;
	public GameObject leftSpawn, rightSpawn, magic;
	private delegate void GUIMethod();
	private GUIMethod guiMethod;
	private int counter = 0;
	
	private string _selected_leader;
	
	void Awake(){
		QuickConnect qc;
		qc = this.GetComponent<QuickConnect>();
		if (qc.enabled == true){
			this.enabled = false;
		}
	}
	// Use this for initialization
	void Start () {
		proteusChat = this.GetComponent<ProteusChat>();
		header = skin.FindStyle("Header");
		loading = skin.FindStyle("Loading");
		question = skin.FindStyle("Question");
		portrait = skin.FindStyle ("Portrait");
		readyButton = skin.FindStyle("ReadyButton");
		checkmark = skin.FindStyle("Checkmark");
		leaderInfo = skin.FindStyle("LeaderInfo");
		menaSpecialText = "\n\n\n\n\t\t\t\t\t\t\t\tSpecial Attack:\n\t\t\t\t\t\t\t\t\t\tEwigen Nacht\n\n\t\tCaptain Mena is the sister of the Altier Seita. A living legend among the Saren Military, she has a reputation for never missing a shot. Over the years she alone ended wars by firing a single bullet over great distances to rout enemy commanders.\n\t\tBorn in the town of Galleone, she was orphaneddue to war and was adopted by Count Emmen, where she earned the nickname,\n\t\t\t\t\"Bellflower of Galleone\"\nbecause they only grew in the places she left in her wake.";
		seitaSpecialText = "\n\n\n\n\t\t\t\t\t\t\t\tSpecial Attack:\n\t\t\t\t\t\t\t\t\t\tFall of the Altier\n\n\t\tBrother to Captain Mena, the Altier is considered a sacred weapon used by the Ralsian People. Seita is one of the only Sarens to ever be chosen by the pendulum hammer.\n\n\t\tA stalwart and compassionate person, he seeks to restore balance between the Ralsian and Saren people. Imbued with the secrets of the Ralse, he has phenomenal strength and unearthly power.";
		guiMethod += WaitingForOtherPlayer;
		guiMethod += TwoPlayerCheck;
		StartCoroutine("Counter");
		proteusChat.photonView.RPC ("GameChat", PhotonTargets.All, "Joined");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Backspace)){
			PhotonNetwork.LeaveRoom();
		}
	}

	void OnLeftRoom(){
		PhotonNetwork.LoadLevel(1);
	}

	void OnGUI(){
		if (!Application.CanStreamedLevelBeLoaded(3) ||  !Application.CanStreamedLevelBeLoaded(2) || Application.GetStreamProgressForLevel(2) < 1 || Application.GetStreamProgressForLevel(3) < 1)
		{
			GUI.skin = skin; 
			PreLoadingGUI();
			return;
		}
		GUI.skin = skin;
		if (guiMethod != null)
			guiMethod();
	}

	void TwoPlayerCheck(){
		if (PhotonNetwork.playerList.Length == 2){
			guiMethod -= WaitingForOtherPlayer;
			StopCoroutine("Counter");
			guiMethod += MainGUI;
			guiMethod -= TwoPlayerCheck;
		}
	}

	void SpawnOtherPlayer(){
		Instantiate(magic, rightSpawn.transform.position, Quaternion.identity);
		otherLeader = Instantiate((PhotonNetwork.otherPlayers[0].customProperties["Leader"].ToString() == "Altier_Seita"?seita:mena), 
		                          rightSpawn.transform.position, rightSpawn.transform.rotation) as GameObject;
		//guiMethod -= SpawnOtherPlayer;
		Invoke ("PrepForNextLevel", 3.0f);
	}

	void PrepForNextLevel(){
		guiMethod = null;
		Destroy(leader);
		Destroy(otherLeader);
		PhotonNetwork.LoadLevel(Application.loadedLevel + 1);
	}
		                        
	IEnumerator Counter(){
		counter = 0;
		while(true){
			counter++;
			if (counter == 4){
				counter = 0;
			}
			yield return new WaitForSeconds(0.5f);
		}
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
		// Checks if there is still two or more players, otherwise returns and waits for more players
		if (PhotonNetwork.otherPlayers.Length != 0)
			GUI.Label(new Rect(Screen.width / 2 + 100, Screen.height / 2 + 50, 256, 50), PhotonNetwork.otherPlayers[0].name, loading);
		else{
			guiMethod -= MainGUI;
			guiMethod += WaitingForOtherPlayer;
			guiMethod += TwoPlayerCheck;
			return;
		}
		// Displays header label
		GUI.Label(new Rect(100, 30, Screen.width - 200, 50), "Choose Your Leader", header);
		// Displays the local player's name
		GUI.Label(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 + 50, 256, 50), PhotonNetwork.playerName, loading);
		// Displays the VS label
		GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 2 + 55, 50, 50), "VS", header);
		// Displays a question mark box if the local player has yet to choose a leader
		if (leaderClicked == 0){
			GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "?", question);
		}
		// Displays a question mark box if the other player has yet to choose a leader
		if (!gameReady){
			GUI.Box(new Rect(Screen.width / 2 + 100, Screen.height / 2 - 256, 256, 256), "?", question);
		}
		// Displays a check mark box if the other player has chosen a leader
		else{
			GUI.Box(new Rect(Screen.width / 2 + 100, Screen.height / 2 - 256, 256, 256), "\u2714", checkmark);
		}
		// Portrait of Mena button
		if(GUI.Button(new Rect(Screen.width / 2 - (256 + 105), Screen.height / 2 + 100, 256 / 2, 256 / 2), mena_texture, portrait)){
			leaderClicked = 1;
			offSet = 0.0f;
		}
		// Portrait of Seita button
		if(GUI.Button(new Rect((Screen.width / 2 - (256 + 95)) + (256 / 2), Screen.height / 2  + 100, 256 / 2, 256 / 2), seita_texture, portrait)){
			leaderClicked = 2;
			offSet = 138;
		}
		// Displays a ready button under selected portrait and handles the click
		if (leaderClicked != 0){
			if(GUI.Button(new Rect(Screen.width / 2 - (256 + 105) + offSet, Screen.height / 2 + 240, 128, 50), "Ready?", readyButton)){
				// Generates a message in the chat that the player is ready
				proteusChat.photonView.RPC("GameChat", PhotonTargets.All, "Ready");
				// Stores the local player's chosen leader into their customerPlayerProperties
				Hashtable player_props = new Hashtable();
				player_props.Add("Leader", _selected_leader);
				PhotonNetwork.player.SetCustomProperties(player_props);
				// Activates the check mark on the other player's game
				this.photonView.RPC("ActivateOtherPlayer", PhotonTargets.Others);
				// Spawns the magic circle on the local player's game
				Instantiate(magic, leftSpawn.transform.position, Quaternion.identity);
				// Spawns Mena to the field if chosen
				if (leaderClicked == 1){
					leader = Instantiate(mena, leftSpawn.transform.position, leftSpawn.transform.rotation) as GameObject;
				}
				// Spawns Seita to the field if chosen
				else if (leaderClicked == 2){
					leader = Instantiate(seita, leftSpawn.transform.position, leftSpawn.transform.rotation) as GameObject;
				}
				guiMethod -= MainGUI;
				// Moves the player's names up to their final position
				StartCoroutine("AnimateLabels");
			}
			// Displays the Mena bio if the Mena portrait is clicked
			if (leaderClicked == 1){
				leaderInfo.normal.background = menaBio;
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "?", question);
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100 + 330), Screen.height / 2 - 256, 300, 470), menaSpecialText, leaderInfo);
				_selected_leader = "Captain_Mena";
			}
			// Displays the Seita bio if the Seita portrait is clicked
			if (leaderClicked == 2){
				leaderInfo.normal.background = seitaBio;
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100 + 330), Screen.height / 2 - 256, 300, 470), seitaSpecialText, leaderInfo);
				GUI.Box(new Rect(Screen.width / 2 - (256 + 100), Screen.height / 2 - 256, 256, 256), "?", question);
				_selected_leader = "Altier_Seita";
			}
		}
	}
	
	void ReadyGUI(){
		if (PhotonNetwork.otherPlayers.Length != 0){
			DisplayPlayersNames();
			if (gameReady){
				SpawnOtherPlayer();
				guiMethod += LoadingGUI;
				gameReady = false;
				//guiMethod += SpawnOtherPlayer;
			}
		}
		else{
			gameReady = false;
			guiMethod = null;
			guiMethod += WaitingForOtherPlayer;
			guiMethod += TwoPlayerCheck;
		}
	}
	
	void LoadingGUI()
	{
		GUI.Label(new Rect(Screen.width / 2 - 128, Screen.height / 2 - 25, 256, 50), "Loading: " + (int)(Application.GetStreamProgressForLevel(Application.loadedLevel + 1) * 100) + "%", loading);
	}
	
	void PreLoadingGUI(){
		GUI.Label(new Rect(Screen.width / 2 - 128, Screen.height / 2 - 25, 256, 50), "Loading: " + (int)(Application.GetStreamProgressForLevel(Application.loadedLevel) * 100) + "%", loading);
	}
	
	IEnumerator AnimateLabels(){
		float startTime = Time.time;
		float duration = 2.0f;
		float elapsed;
		labelHeight = Screen.height / 2 + 50;
		guiMethod += DisplayPlayersNames;
		do
		{ 	
			//DisplayPlayersNames();
			// Calculate how far through we are
			elapsed = Time.time - startTime;
			float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
			labelHeight = Mathf.Lerp((Screen.height / 2 + 50), 35, normalisedTime);
			yield return null;
		}while(elapsed < duration);
		guiMethod -= DisplayPlayersNames;
		guiMethod += ReadyGUI;
	}

	void DisplayPlayersNames(){
		GUI.Label(new Rect(Screen.width / 2 - (256 + 100), labelHeight, 256, 50), PhotonNetwork.playerName, loading);
		GUI.Label(new Rect(Screen.width / 2 - 25, (labelHeight+5), 50, 50), "VS", header);
		GUI.Label(new Rect(Screen.width / 2 + 100, labelHeight, 256, 50), PhotonNetwork.otherPlayers[0].name, loading);
	}
	
	[RPC]
	void ActivateOtherPlayer(PhotonMessageInfo mi)
	{
		gameReady = true;
	}
}
