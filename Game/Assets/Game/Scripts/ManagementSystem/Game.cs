/*
 * Game.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(RecruitSystem))]
public class Game : Photon.MonoBehaviour
{
	public GameObject load_game_objects;

	public int num_of_players;

	public int resource_limit;

	public GameObject[] available_leaders;

	public bool testing;
	private Terrain fow_terrain;
	public Material fow_material;

	/* 
	 * Variables used for testing GameManager
	 */
	private delegate void GUIMethod();
	private GUIMethod gui_method;

	private WorldCamera wcm;


	private GameObject __focus_object;
	
	// Use this for initialization
	void Start () 
	{
		__focus_object = null;

		if(testing)
		{
			this.gui_method += GUI_menu;
		}

		GameObject game = GameObject.FindGameObjectWithTag("Game_Init");
		if(game != null)
		{
			Debug.LogWarning("A game init is already in the scene. Using that one.");
		}
		else 
		{
			game = Instantiate(load_game_objects, Vector3.zero, Quaternion.identity) as GameObject;
		}

		//this.gui_method += GUI_menu; 
		GM.instance.Init(num_of_players, RandomFirstPlayer(num_of_players), resource_limit, GetComponent<RecruitSystem>().unit_cost);
		
		FindWorldCamera();
		//wcm.ChangeCamera();
		
		// Turn on gui fog of war
		fow_terrain = game.GetComponentInChildren<Terrain>();
		fow_terrain.materialTemplate = fow_material;
	}
	//HACK
	bool showDebug = true;

	void OnLeftRoom(){
		PhotonNetwork.LoadLevel(1);
	}
	
	// Update is called once per frame
	void Update () 
	{

		//HACK
		if (Input.GetKeyDown(KeyCode.Q))
			showDebug = !showDebug;
		if(Input.GetKeyDown(KeyCode.Backspace)){
			PhotonNetwork.LeaveRoom();
		}
		if(GM.instance.IsOn)
		{
			if(GM.instance.IsNextPlayersTurn())
			{
				GM.instance.NextPlayersTurn();
			}

			if(wcm != null)
			{
				if(Input.GetMouseButtonDown(0) && wcm.MainCamera != null)
				{
					// Reset timer for display the resource text
					if(GM.instance.CurrentFocus == null )//&& GM.instance.IsItMyTurn())
					{
						Ray ray = wcm.MainCamera.camera.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;
						if(Physics.Raycast(ray, out hit, 100))
						{ 
							// Get correct, unit
							string tag = hit.transform.tag;
							
							if(tag == "Unit" || tag == "Leader")
							{
								GameObject obj = hit.transform.gameObject;
								GM.instance.SetUnitControllerActiveOn(ref obj);
							}
							else
							{
								//GM.instance.SetUnitControllerActiveOff();
							}
						}
					}
				}
			}
		}
	}


	void OnGUI()
	{
		if (showDebug)
		{
			GUILayout.Label("isMasterClient: " + PhotonNetwork.isMasterClient);
			GUILayout.Label("Players: " + PhotonNetwork.playerList.Length);
			GUILayout.Label("Ping: " + PhotonNetwork.GetPing());
			GUILayout.Label ("Press Backspace to return to Preloader");
			//for(int i=1;i<6;++i)
				//GUILayout.Label (string.Format("ViewID{0}: {1}",i, PhotonView.);
		}
		if(this.gui_method != null)
		{
			this.gui_method();
		}
	}
	
	void GUI_menu()
	{
		float half = 0;
		
		if(GM.instance.IsOn)
		{
			if(MakeButton(half, 150, "End turn"))
			{
				GM.instance.NextPlayersTurn();
			}
			
			else if(MakeButton(half, 210, string.Format("Add 1000 resource pts")))
			{
				GM.instance.AddResourcesToCurrentPlayer(1000);
			}
		}
	}
	
	bool MakeButton(float left, float top, string name)
	{
		return GUI.Button(new Rect(left,top+20, 150,20), name);
	}
	
	int RandomFirstPlayer(int number_of_players)
	{
		return Random.Range(1,number_of_players+1);
	}

	void FindWorldCamera ()
	{
		wcm = GameObject.Find("WorldCamera").GetComponent<WorldCamera>();
		if(wcm == null)
		{
			Debug.LogError("Cannot find WorldCamera");
		}
	}
	
	void Reset ()
	{
		num_of_players = 2;
		resource_limit = 1000000;
		testing = false;
	}

	[RPC]
	void ChangeTurn()
	{
		GM.instance.NextPlayersTurn();
	} 
}
