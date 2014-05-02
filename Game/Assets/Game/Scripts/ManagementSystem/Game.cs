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

	private GUIText _game_manager_gui;
	
	private UnitCost _unit_cost;

	private float waitingTime;
	private float timer;
	private bool init;

	private WorldCamera wcm;
	
	void Awake() 
	{
		_game_manager_gui = GameObject.Find("GameManagerStatus").GetComponent<GUIText>();
		_game_manager_gui.text = "";
	
		_unit_cost = GetComponent<RecruitSystem>().unit_cost;
	}
	
	// Use this for initialization
	void Start () 
	{
		if(testing)
		{
			this.gui_method += GUI_init;
			_game_manager_gui.enabled = true;
			_game_manager_gui.transform.position = new Vector3(0.18f, 0.95f, 0.0f);
			_game_manager_gui.fontSize = 16;
			//FindWorldCamera();
			waitingTime = 5.0f;
			timer = 0.0f;
			
			init = false;
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
		wcm.ChangeCamera();
		
		// Turn on gui fog of war
		fow_terrain = game.GetComponentInChildren<Terrain>();
		fow_terrain.materialTemplate = fow_material;
	}
	//HACK
	bool showDebug = false;
	// Update is called once per frame
	void Update () 
	{
		//HACK
		if (Input.GetKeyDown(KeyCode.Q))
			showDebug = !showDebug;
		if(GM.instance.IsOn)
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				GM.instance.AddResourcesToCurrentPlayer(1000);
				Debug.Log(string.Format("{0} has {1} resources", GM.instance.CurrentPlayer, GM.instance.GetResourceFrom(GM.instance.CurrentPlayer)));
			}

			if(GM.instance.IsThereAWinner())
			{
				_game_manager_gui.text = string.Format("The winner is {0}!", GM.instance.Winner);
			}

			if(GM.instance.IsNextPlayersTurn())
			{
				GM.instance.NextPlayersTurn();
				_game_manager_gui.text = string.Format("It is now {0}'s turn", GM.instance.CurrentPlayer);
			}

			if(wcm != null)
			{
				if(Input.GetMouseButtonDown(0) && wcm.MainCamera != null)
				{
					// Reset timer for display the resource text
					timer = 0;
					if(GM.instance.CurrentFocus == null)
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

			if(testing)
			{
				timer += Time.deltaTime;
				if(timer > waitingTime)
				{
					//Action
					_game_manager_gui.text = string.Format("Current player: {0} at {1}/{2} Resources", 
					                                       GM.instance.CurrentPlayer, 
					                                       GM.instance.GetResourceFrom(GM.instance.CurrentPlayer).ToString(),
					                                       GM.instance.MaxResourceLimit.ToString());
					timer = 0;
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
			//for(int i=1;i<6;++i)
				//GUILayout.Label (string.Format("ViewID{0}: {1}",i, PhotonView.);
		}
		if(this.gui_method != null)
		{
			this.gui_method();
		}
	}
	
	void GUI_init()
	{
		if(MakeButton(0,80,"Start GameManager"))
		{
			if(init)
			{
				return;
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

			this.gui_method += GUI_menu;
			init = true;
			_game_manager_gui.text = "Game Manager enabled";
			
			GM.instance.Init(num_of_players, RandomFirstPlayer(num_of_players), resource_limit, GetComponent<RecruitSystem>().unit_cost);

			FindWorldCamera();
			wcm.ChangeCamera();

			// Turn on gui fog of war
			fow_terrain = game.GetComponentInChildren<Terrain>();
			fow_terrain.materialTemplate = fow_material;
		}
		
		else if(MakeButton(0, 100, "End GameManager"))
		{
			if(!init)
			{
				return;
			}
			
			this.gui_method -= GUI_menu;
			init = false;
			_game_manager_gui.text = "Game Manager disabled";
			
			GM.instance.ResetGameState();
		}
	}
	
	void GUI_menu()
	{
		float half = 0;
		
		if(GM.instance.IsOn)
		{
			if(MakeButton(half, 150, "Next player's turn"))
			{
				GM.instance.NextPlayersTurn();
				
				_game_manager_gui.text = string.Format("Next player's turn\n" + 
				                                       "Current player: {0}\n",
				                                       GM.instance.CurrentPlayer);
			}
			
			else if(MakeButton(half, 170, "Current round #"))
			{
				_game_manager_gui.text = string.Format("Current round: {0}",
				                                       GM.instance.CurrentRound);
			}
			
			else if(MakeButton(half, 190, "Timer"))
			{
							_game_manager_gui.text = string.Format("Current time: {0}", GM.instance.CurrentTime);
			}
			
			else if(MakeButton(half, 210, string.Format("Add 1000 resource pts")))
			{
				GM.instance.AddResourcesToCurrentPlayer(1000);
				_game_manager_gui.text = string.Format("Current player: {0} at {1}/{2} Resources", 
				                                       GM.instance.CurrentPlayer, 
				                                       (GM.instance.GetResourceFrom(GM.instance.CurrentPlayer)).ToString(),
				                                       GM.instance.MaxResourceLimit.ToString());
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

	public void ResetGUIState()
	{
		init = false;
		this.gui_method -= GUI_init;
		this.gui_method -= GUI_menu;
	}
	
	void Reset ()
	{
		num_of_players = 2;
		resource_limit = 1000000;
		testing = false;
		_game_manager_gui.text = "";
	}
}
