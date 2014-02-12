using UnityEngine;
using System.Collections;

public class NetworkManagerScript : MonoBehaviour {
	/*float buttonX;
	float buttonY;
	float buttonW;
	float buttonH;*/
	public GameObject playerPrefab;
	public Transform spawnObject;
	public string gameName = "Bangarang Games";
	private bool refreshing;
	private HostData[] hostData;
	// Use this for initialization
	void Start () {
		/*buttonX = Screen.width * 0.05;
		buttonY = Screen.width * 0.05;
		buttonW = Screen.width * 0.1;
		buttonH = Screen.width * 0.1;*/
	}

	void StartServer () {
		Network.InitializeServer(3, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost (gameName, "Proteus Moon", "This is a test game");
	}

	void RefreshHostList() {
		MasterServer.RequestHostList (gameName);
		refreshing = true;
	}

	void SpawnPlayer (){
		Network.Instantiate (playerPrefab, spawnObject.position, Quaternion.identity, 0);	
	}

	void OnServerInitialized () {
		Debug.Log ("Server initialized!");
		SpawnPlayer ();
	}

	void OnConnectedToServer(){
		SpawnPlayer ();
	}

	void OnMasterServerEvent(MasterServerEvent mse){
		if (mse == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log ("Registered Server");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (refreshing){
			if(MasterServer.PollHostList().Length > 0){
				refreshing = false;
				Debug.Log (MasterServer.PollHostList().Length);
				hostData = MasterServer.PollHostList();
			}
		}
	}

	//GUI
	void OnGUI () {
		if(!Network.isClient && !Network.isServer){
			if(GUI.Button(new Rect(Screen.width - 105, 5, 100, 30), "Starting Server")){
				Debug.Log ("Starting Server");
				StartServer();
			}
			if(GUI.Button(new Rect(Screen.width - 105, 40, 100, 30), "Refresh Hosts")){
				Debug.Log ("Refreshing");
				RefreshHostList();
			}
			if (hostData != null) {
				for (int i = 0; i < hostData.Length; i++) {
					if(GUI.Button(new Rect (Screen.width - 105, 75, 100, 30), hostData[i].gameName)){
					   Network.Connect(hostData[i]);
					}
				}
			}
		}
	}
}
