using UnityEngine;
using System.Collections;

public class WaitingRoomScript : MonoBehaviour {
	
	public GUISkin skin;
	public Texture2D mena_texture;
	public Texture2D seita_texture;
	private bool leader_chosen = false;
	// Use this for initialization
	void Start () {
	
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

		if (PhotonNetwork.playerList.Length <= 1)
			WaitingForOtherPlayer();
		else if (PhotonNetwork.playerList.Length == 2 && !leader_chosen)
			MainGUI();
		else
			LoadingGUI();
	}
	void WaitingForOtherPlayer(){
		GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 12, 250, 40), "Waiting For Other Player...");
	}

	void MainGUI(){

		if(GUI.Button(new Rect(Screen.width / 2 - (mena_texture.width + 100), Screen.height / 2 - mena_texture.height, mena_texture.width, mena_texture.height), mena_texture)){
			GUI.Label(new Rect(Screen.width / 2 - 125, 10, 250, 40), "You Chose Mena");
			print ("You Chose Mena");
			leader_chosen = true;
		}

		if(GUI.Button(new Rect(Screen.width / 2 + 100, Screen.height / 2 - seita_texture.height, seita_texture.width, seita_texture.height), seita_texture)){
			GUI.Label(new Rect(Screen.width / 2 - 125, 10, 250, 40), "You Chose Seita");
			print ("You Chose Seita");
			leader_chosen = true;
		}
	}

	void LoadingGUI()
	{
		GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 12, 140, 25), "Loading: " + (int)(Application.GetStreamProgressForLevel(2) * 100) + "%");
		Application.LoadLevel(Application.loadedLevel + 1);
	}
}
