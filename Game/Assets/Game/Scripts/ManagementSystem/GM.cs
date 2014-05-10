/*
(* GameManager.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
/* GameManager - keeps track of the game state and control
 * Controlling the game 
 *  X Check when next player's turn should be or when current player passes their turn
 *  X Keep track of rounds, turn order, timer, player's units resources, leaders, each player's units, who won, etc
 *  X Control recruit position, next to leader
 *  - Should pause game or unpause
 *  -(MAYBE) Keep track of network between players, if we add *networking* capabilities
 * 
 *  -(MAYBE) Keep track of buffs, debuff, passive that should be received from other units
 * 
 *  X Record stats at the end of the game
 *   	# of kills, resource collected/spent, rounds, timer?, # of units recruited,(?ranking algorithm?)
 */

// Player turn enum
public enum Player : byte
{
		Player1 = 0,
		Player2,
		Player3,
		Player4,
		NONE
}

[RequireComponent(typeof(PhotonView))]
public class GM : Photon.MonoBehaviour 
{
	private static GM _instance;
	//private static GameObject _GM_obj;

	// Determine whether the GameManager is active or not
	private bool 				_game_init;
	
	// Keep track of player turn order and number of players
	private Player[]  			_player_turn_order;
	private int 				_current_player_turn;
	private int 				_total_players;

	// References to script that are needed for the GameManager to keep track of
	private UnitCost            _unit_cost;
	private RecruitSystem       _recruit_system;
	private UnitController      _unit_controller;
	private WorldCamera 		_world_camera;
	
	// Winning conditions
	//
	// Current amount of resource for each player
	private int[] 				_resource_count;
	private int   				_max_resource;

	// Keep track of each player's unit accordingly
	//HACK private GameObject[] 		_leaders;
	
	// Reference to containers, units will be rooted to them
	// The gameobject will contain the objects of that player's units and leader as its children nodes
	private GameObject[] 		_player_container;

	// Who has won, starts off as Player.NONE
	private Player _winner;
	
	// Recording scores
	private int[] _resources_obtained;
	private int[] _resource_spent;
	private int[] _units_obtained;
	private int[] _units_killed;
	private  int  _round_num;
	

	private	PhotonHashTable turn_order;

	// Keep track of base time, which we use as a base in which time continues from that point and onwards
	private  float _base_time;

	/*
	 * HACKY
	 */
//	private static Game _game_gui;
	/*
	 * 
	 */

	//HACK 
	private GameObject __leader;

	//PhotonView GMpv = PhotonView.Get(this);

	public void Awake()
	{
		Destroy();
	}

	#region Initialization type methods
	/// <summary>
	/// Similiar to that of a constructor. Initializes the GameManager with all needed functionality already provided in the back end.
	/// Should only be called once!
	/// </summary>
	/// <param name="num_of_players">Num_of_players.</param>
	/// <param name="who_goes_first">Who_goes_first.</param>
	/// <param name="resource_win_count">Resource_win_count.</param>
	/// <param name="unit_cost">Unit_cost.</param>
	public void Init(int num_of_players, int who_goes_first, int resource_win_count, UnitCost unit_cost)
	{
		Debug.Log("Start up GM");
		
		// Valid player limit
		if((who_goes_first < 1) ||(who_goes_first > num_of_players) ||(num_of_players > 4)) 
		{
			Debug.LogError("Player limit reached!\nProblem in GameManager.cs");
			return;
		}
		
		_total_players = num_of_players;
		
		_recruit_system = GameObject.FindGameObjectWithTag("GameController").GetComponent<RecruitSystem>();
		if(_recruit_system == null)
		{
			Debug.LogWarning("Recruit System missing reference");
		}
		
		_unit_controller = GameObject.FindGameObjectWithTag("UnitController").GetComponent<UnitController>();
		if(_unit_controller == null)
		{
			Debug.LogWarning("Unit Controller missing reference");
		}
		
		_world_camera = GameObject.Find("WorldCamera").GetComponent<WorldCamera>();
		if(_world_camera == null)
		{
			Debug.LogWarning("World Camera missing reference");
		}

		/*
		 * HACKY
		 */
		//_game_gui = GameObject.Find("GameController").GetComponent<Game>();
		/*
		 * 
		 */

		_winner = Player.NONE;
		
		// Keep track of cost
		_unit_cost = unit_cost;

		// What is the max number of resources required to win
		_max_resource = resource_win_count;
		
		_current_player_turn = 0; 
		
		Allocate();

		// Get player units on the screen, for now assuming leaders are there before the game starts
		InitPlayerContainers();
		InitPlayersLeader();
		
		ResetGameState();
		
		// Set camera
		//_world_camera.ChangeCamera();
		
		StartTimer();

		_game_init = true;
		
		
	}

	// Allocate memory blocks for things needed in the game
	private void Allocate()
	{
		// One time calls of basic init.
		_player_turn_order  = new Player[_total_players];

		// Allocate correct number of resource counters
		_resource_count     = new int[_total_players];
		_resource_spent     = new int[_total_players];
		_resource_count     = new int[_total_players];
		_resources_obtained = new int[_total_players];
		
		// Allocate correct number of leaders
		//HACK _leaders 			= new GameObject[_total_players];
		
		// Allocate correct number of player containers for their units/leaders
		_player_container 	= new GameObject[_total_players];
		
		// How many units each player purchased & killed
		_units_obtained 	= new int[_total_players];
		_units_killed       = new int[_total_players];
	}

	// Point to player container or create one if needed, based on the number of players
	private void InitPlayerContainers()
	{
		GameObject obj;
		
		for(int i=1; i<=_total_players; ++i) 
		{
			// Find player
			obj = GameObject.FindGameObjectWithTag(string.Format("Player{0}", i));
			
			// Player container exist, point to it
			if(obj != null)
			{
				_player_container[i - 1] = obj as GameObject;
			}
			
			// Player container doesn't exist, uh-oh!
			else
			{
				Debug.LogError(string.Format("Container for player{0} is missing! Stuff will break. Add it you fool!", i));
			}
		}
	}

	// Keep track of each player's leader, making sure who has lost the game if their leader has died
	private void InitPlayersLeader()
	{
		int id = PhotonNetwork.player.ID;
		GameObject spawn_locations = GameObject.Find(string.Format("_leader_spawn{0}", id));

		__leader = null;

		GameObject[] available_leaders = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>().available_leaders;

		if ((string)PhotonNetwork.player.customProperties["Leader"] == "Altier_Seita")
			__leader = available_leaders[0];
		else if ((string)PhotonNetwork.player.customProperties["Leader"] == "Captain_Mena")
			__leader = available_leaders[1];

		if(__leader == null)
		{
			Debug.LogError("Leader failed to be instantiated!");
			ForceQuit();
		}

		// The appropiate leader should be picked now.
		// Give it to the respective player
		if (PhotonNetwork.inRoom)
			__leader = PhotonNetwork.Instantiate(__leader.name, spawn_locations.transform.position, spawn_locations.transform.rotation, 0) as GameObject;
		else
			__leader = Instantiate(__leader, spawn_locations.transform.position, spawn_locations.transform.rotation) as GameObject;

		// So at this point. The local player's leader has been created
		//__leader.transform.parent = _player_container[id-1].transform; // put it in the player's container in the scene
		StartCoroutine(SetupPlayerContainer());

		// Set camera focus on leader
		WorldCamera.instance.LeaderFocus();

		//Debug.Log(string.Format("There are a total of {0} leaders", Get_Leaders.Length));
	}

	// Get all leaders in the room and puts them in their respective local container
	IEnumerator SetupPlayerContainer()
	{
		while(_total_players != Get_Leaders.Length){
			Debug.Log ("Photon PlayerList Length: " + Get_Leaders.Length);
			Debug.Log ("Our Game PlayerList Length: " + _total_players);
			yield return new WaitForSeconds(0.5f);
		}
	
		foreach(GameObject leader in Get_Leaders)
		{
			Debug.Log(string.Format("Leader {0} is owned by {1}", leader.name, leader.GetPhotonView().owner.name));
			leader.transform.parent = _player_container[leader.GetPhotonView().owner.ID-1].transform;
			
			UpdateFogOfWarComponents(leader);
			
		}

		// Distinguish which leader belongs to which player
		// Make sure there is a player container prepared already.
		if(__leader.transform.parent == null)
		{
			Debug.LogError(string.Format("Missing parent object for {0}. Parent object should be tagged \"Player#\"", __leader.name));
		}

		Debug.Log(string.Format("{0} owns this room: {1}", (Player)PhotonNetwork.masterClient.ID-1, PhotonNetwork.room.name.ToString()));

		//Generate Turn Sequence
		//If you are the host send the turn order to the other player
		if(PhotonNetwork.player.isMasterClient)
		{
			GenerateTurnSequence();

			//PhotonHashTable reuse_hash = PhotonNetwork.room.customProperties;

			//for(int i=0;i < Get_Leaders.Length;++i)
			//{
			//HACK MARK HACK
			//reuse_hash.Add("TurnOrder",  _player_turn_order);	
			//Debug.Log("Add "+ string.Format("Turn{0}",i) +  " this value" + (int)_player_turn_order[i]);
			//	reuse_hash.Add(string.Format("Turn{0}",i),  _player_turn_order[i]);
			//	Debug.Log("Add "+ string.Format("Turn{0}",i) +  " this value" + (int)_player_turn_order[i]);
			//}
				
			
			//PhotonNetwork.room.SetCustomProperties(reuse_hash);
			
			this.photonView.RPC("SendTurnOrder", PhotonTargets.All, _player_turn_order);
		}
				
		//__leader.GetPhotonView().owner.customProperties.Add("current_player_turn", _current_player_turn);
	}
	
	[RPC]
	void SendTurnOrder(Player[] playerTurnOrder, PhotonMessageInfo mi)
	{
		for (int i = 0; i < playerTurnOrder.Length; ++i){
			_player_turn_order[i] = playerTurnOrder[i];
		}

	}
	
	/// <summary>
	/// Updates the fog of war components per unit
	/// </summary>
	/// <param name="unit">Unit.</param>
	private void UpdateFogOfWarComponents(GameObject unit){
	
		if( !unit.GetPhotonView().isMine  ){
			
			unit.GetComponent<FOWRenderers>().enabled = true;
			unit.GetComponent<FOWRevealer>().enabled = false;
			
		}
		else{
			unit.GetComponent<FOWRenderers>().enabled = false;
			unit.GetComponent<FOWRevealer>().enabled = true;
			unit.GetComponent<FOWRevealer>().isActive = true;
		}
	}
	
	

	


	// Currently, shuffles player's turn order
	private void GenerateTurnSequence()
	{
		// Sloppy way of doing this
		List<int> tmp = new List<int>();
		int i = 0;

		Random.seed = 42;
		Random.seed = (int)Random.value;
		Random.seed = (int)(Random.value+1)/(Random.seed+1);

		while(i<_total_players) 
		{
			int t = Random.Range(0, _total_players);
			if(!tmp.Contains(t)) 
			{
				tmp.Add(t);
				_player_turn_order[i] =(Player)t;
				++i;
			}
		}
	}
	#endregion

	#region Deinitialization type methods
	/// <summary>
	/// Removes the instance of the GameManager
	/// </summary>
	public void Destroy()
	{
		Debug.Log("Destroy GM");
		_instance  = null;
		_game_init = false;
	}

	/// <summary>
	/// Deallocate values that were used
	/// </summary>
	public void ResetGameManager()
	{
		Destroy();
		ResetGameState();
		Deallocate();
	}

	// Restroy and remove pointers to the objects.
	private void Deallocate()
	{
		_player_turn_order 	= null;
		_resource_count    	= null;
		_resource_spent    	= null;
		_resources_obtained = null;
		//HACK _leaders			= null;
		_player_container	= null;
		_units_obtained		= null;
		_units_killed  		= null;
		_recruit_system     = null;
		_unit_controller    = null;
		_world_camera       = null;
	}

	// Reset variables that are required to keep track of info during the game
	/// <summary>
	/// Resets the values in the game. Typically used for when the game is over or restart.
	/// </summary>
	public void ResetGameState()
	{
		// Reset values that are used for recording players numbers
		ResetRecordings();
		
		// Reset leaders to be alive
		ResetLeaders();
		
		// Player order	
		//GenerateTurnSequence();
		
		_winner = Player.NONE;
	}

	// Blank resource counts
	private void ResetRecordings()
	{
		// Reset round number
		_round_num = 1;
		
		// Reset 
		//_timer = 0.0f;
		
		for(int i=0;i<_resource_count.Length;++i)
		{
			_resource_count[i]     = 0;
			_resource_spent[i]     = 0;
			_resources_obtained[i] = 0;
			_units_obtained[i]     = 0;
			_units_killed[i]	   = 0;
		}
		StartTimer();
	}

	// All leaders are alive
	private void ResetLeaders()
	{
		/*HACK for(int i=0;i<_leaders.Length;++i)
		{
			_leaders[i].GetComponent<BaseClass>().unit_status.Clean();
		}*/
	}

	private void StartTimer()
	{
		_base_time = Time.time;
	}
	#endregion

	#region Getter methods
	/// <summary>
	/// Gets the instance of the Game Manager. Will only return the single instance and that's it.
	/// </summary>
	/// <value>The instance.</value>
	public static GM instance
	{
		get 
		{
			if(_instance == null)
			{
				_instance = GameObject.FindGameObjectWithTag("GameManager").AddComponent<GM>();
				//PhotonView photonView = GameObject.Find("GameManager").GetPhotonView();
				//PhotonView
				//_instance = GameObject.FindGameObjectWithTag("GameController").AddComponent<GM>();
				/*
				_GM_obj = Instantiate(Resources.Load("GM", typeof(GameObject))) as GameObject;
				_GM_obj.AddComponent<GM>();
				_instance = _GM_obj.GetComponent<GM>();
				*/
			}

			return _instance;
		}
	}

	/// <summary>
	/// Gets the GM as a GameObject
	/// </summary>
	/// <value>The G m_obj.</value>
	/*public static GameObject GM_obj
	{
		get { return _GM_obj; }
	}*/

	public Player[] TurnOrder
	{
		get{
		
			return _player_turn_order;
		}
	}

	/// <summary>
	/// Gets the photon_ leader.
	/// </summary>
	/// <value>The photon_ leader.</value>
	public GameObject Photon_Leader
	{
		get 
		{ 
			return __leader; 
		}
	}

	public PhotonView GM_Photon_View
	{
		get
		{
			return __leader.GetPhotonView();
		}
	}
 	public GameObject[] Get_Leaders
	{
		get
		{
			return GameObject.FindGameObjectsWithTag("Leader");
		}
	}

	public Player WhichPlayerAmI
	{
		get
		{
			return (Player)__leader.GetPhotonView().owner.ID-1;
		}
	}

	/// <summary>
	/// Gets the transformation of the instance
	/// </summary>
	/// <value>The object_instance.</value>
	public Transform object_instance
	{
		get { return _instance.transform; }
	}

	/// <summary>
	/// Gets a value indicating whether this instance is on.
	/// </summary>
	/// <value><c>true</c> if this instance is on; otherwise, <c>false</c>.</value>
	public bool IsOn
	{
		get { return _game_init; }
	}

	/// <summary>
	/// Gets the total number of players
	/// </summary>
	/// <returns>The of players.</returns>
	public int NumberOfPlayers
	{
		get { return _total_players; }
	}

	/// <summary>
	/// Returns the current player's turn as an enum of type Player
	/// </summary>
	/// <returns>The current player.</returns>
	public Player CurrentPlayer
	{
		get { return(Player)_current_player_turn; }
	}

	/// <summary>
	/// Returns the player given an input int. If none match, returns Player.NONE
	/// </summary>
	/// <returns>The player.</returns>
	/// <param name="p">P.</param>
	public Player GetPlayer(int p)
	{
		if(p>5)
		{
			return Player.NONE;
		}
		return (Player)p;
	}
	// Get resource amount from a player
	/// <summary>
	/// Gets the resource from a player.
	/// </summary>
	/// <returns>The resource from.</returns>
	/// <param name="player">Player.</param>
	public int GetResourceFrom(Player player)
	{
		return _resource_count[(int)player];
	}

	/// <summary>
	/// Get the max number of resources needed to be aquired for this match
	/// </summary>
	/// <returns>The max resource limit.</returns>
	public int MaxResourceLimit
	{
		get { return _max_resource; }
	}

	/// <summary>
	/// Gets the winner of the match.
	/// </summary>
	/// <returns>The winner.</returns>
	public Player Winner
	{
		get { return _winner; }
	}

	/// <summary>
	/// Get all units(including leader) from a chosen player.
	/// </summary>
	/// <returns>The units from player.</returns>
	/// <param name="player">Player.</param>
	public GameObject[] GetUnitsFromPlayer(Player player)
	{
		BaseClass[] base_class = _player_container[(int)player].GetComponentsInChildren<BaseClass>();
		GameObject[] units = new GameObject[base_class.Length];
		for(int i=0;i<units.Length;++i)
		{
			units[i] = base_class[i].gameObject;
		}
		
		return units;
	}

	/// <summary>
	/// Gets the current time.
	/// </summary>
	/// <returns>The current time.</returns>
	public float CurrentTime
	{
		get { return Time.time - _base_time; }
	}

	/// <summary>
	/// Gets the current focused unit in which a player has clicked on, awaiting an action.
	/// </summary>
	/// <returns>The current focus.</returns>
	public GameObject CurrentFocus
	{
		get { return _unit_controller.UnitControllerFocus; }
	}

	/// <summary>
	/// Gets the current focused camera.
	/// </summary>
	/// <value>The current camera.</value>
	public Camera CurrentFocusCamera
	{
		get { return _world_camera.MainCamera.GetComponent<Camera>(); }
	}

	/// <summary>
	/// Gets all units near player. Including friendly and enemy units.
	/// </summary>
	/// <returns>The all units near player.</returns>
	/// <param name="distance">Distance.</param>
	public List<GameObject> GetAllUnitsNearPlayer(GameObject target, float distance)
	{
		// Get all units that have a base class, they all should have one.
		BaseClass[] units_bc = GameObject.FindObjectsOfType<BaseClass>();
		
		List<GameObject> units = new List<GameObject>();
		
		foreach(BaseClass unit in units_bc)
		{
			if(Vector3.Distance(unit.transform.position, target.transform.position) < distance)
			{
				units.Add(unit.gameObject);
			}
		}
		
		return units;
	}

	/// <summary>
	/// Gets the friendly units near player.
	/// </summary>
	/// <returns>The friendly units near player.</returns>
	/// <param name="distance">Distance.</param>
	public List<GameObject> GetFriendlyUnitsNearPlayer(GameObject target, float distance)
	{
		// Get all of the current player's units that are in their unique player container.
		BaseClass[] units_bc = _player_container[_current_player_turn].GetComponentsInChildren<BaseClass>();
		
		List<GameObject> units = new List<GameObject>();
		
		foreach(BaseClass unit in units_bc)
		{
			if((unit.transform.position - target.transform.position).sqrMagnitude < distance * distance)
			{
				units.Add(unit.gameObject);
			}
		}
		
		return units;
	}

	/// <summary>
	/// Gets the enemy units near player.
	/// </summary>
	/// <returns>The enemy units near player.</returns>
	/// <param name="distance">Distance.</param>
	public List<GameObject> GetEnemyUnitsNearPlayer(GameObject target, float distance)
	{
		List<GameObject> enemies = new List<GameObject>();
		for(int i=0;i<_total_players;++i)
		{
			if(i == _current_player_turn)
			{
				// skip own units, we want enmies
			}
			else
			{
				foreach(BaseClass unit in _player_container[i].GetComponentsInChildren<BaseClass>())
				{
					if(Vector3.Distance(unit.transform.position, target.transform.position) < distance)
					{
						enemies.Add(unit.gameObject);
					}
				}
			}
		}
		
		return enemies;
	}
	#endregion

	#region Winner related methods
	// Get who ever is winning currently in terms of resources
	/// <summary>
	/// Gets the lead in terms of resources. Returns a Player type.
	/// </summary>
	/// <returns>The lead in resources.</returns>
	public Player LeadInResources
	{
		get 
		{
			if( _resource_count != null)
			{
				int most = 0;
				for(int i=1; i<_total_players; ++i) 
				{
					if(_resource_count[i] > _resource_count[most])
						most = i;
				}
				
				return _player_turn_order[most];
			}
			else
			{
				return Player.NONE;
			}

		}
	}

	// Get a count for how many leaders are currently still alive
	private int SurvivingLeaderCount
	{
		get
		{
			int alive = 0;
//HACK			foreach(GameObject leader in _leaders)
//			{
//				if(!leader.GetComponent<BaseClass>().unit_status.status.Dead)
//				{
//					alive += 1;
//				}
//			}

			return alive;
		}
	}

	// Check winning conditions. See if any player won
	/// <summary>
	/// Determines if is there A winner.
	/// </summary>
	/// <returns><c>true</c> if is there A winner; otherwise, <c>false</c>.</returns>
	public bool IsThereAWinner()
	{
		Player lead = LeadInResources;
		bool is_winner = false;

		if(_resource_count != null)
		{
			// Win by resource
			if(_resource_count[(int)lead] >= _max_resource) 
			{
				_winner = lead;
				is_winner = true;
			}
			
			// Win by having 1 leader survive/killing off other leaders
			/*HACK else if(SurvivingLeaderCount == 1) 
			{
				for(int i=0; i<=_total_players; ++i) 
				{
					if(!_leaders[i].GetComponent<BaseClass>().unit_status.status.Dead)
					{
						_winner =(Player)i;
						is_winner = true;
						i = _leaders.Length;
					}
				}
			}*/
		}

		return is_winner;
	}
	#endregion

	#region Game interaction methods
	// Add X amount of points to resource counter array, according to player
	/// <summary>
	/// Adds the resources according to the amount, determined by a Player type passed.
	/// </summary>
	/// <param name="player_turn">Player_turn.</param>
	/// <param name="amount">Amount.</param>
	public void AddResources(Player player_turn, int amount)
	{
		_resource_count[(int)player_turn]     += amount;
		_resources_obtained[(int)player_turn] += amount;
		
		if(_resource_count[(int)player_turn] < 0)
		{
			_resource_count[(int)player_turn] = 0;
		}
	}

	// Add X amount of points to resource counter array, according to player
	/// <summary>
	/// Adds the resources to current player by a amount.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void AddResourcesToCurrentPlayer(int amount)
	{
		_resource_count[_current_player_turn]     += amount;
		_resources_obtained[_current_player_turn] += amount;
		
		if(_resource_count[_current_player_turn] < 0)
		{
			_resource_count[_current_player_turn] = 0;
		}
	}

	// Return bool if player can purchase unit, if so do purchase
	// Based on unit type passed
	/// <summary>
	/// Purchases a unit, if the amount of resource is met then purchase as well as return true
	/// , else cannot purchase as well as return false. Then place unit into the scene into its 
	/// proper Player container with appropriate name, tag, and components.
	/// </summary>
	/// <returns><c>true</c>, if unit was recruited, <c>false</c> otherwise.</returns>
	/// <param name="player">Player.</param>
	/// <param name="unit_type">Unit_type.</param>
	public void RecruitUnit(Player player, UnitType unit_type)
	{
		int cost;
		switch(unit_type) 
		{
		case UnitType.Arcane:
			cost = _unit_cost.arcane;
			break;
		case UnitType.Braver:
			cost = _unit_cost.braver;
			break;
		case UnitType.Scout:
			cost = _unit_cost.scout;
			break;
		case UnitType.Sniper:
			cost = _unit_cost.sniper;
			break;
		case UnitType.Titan:
			cost = _unit_cost.titan;
			break;
		case UnitType.Vangaurd:
			cost = _unit_cost.vangaurd;
			break;
		default:
			Debug.LogError(string.Format("Unit type: {0} does not have an associated cost to it!", unit_type));
			cost = -1;
			break;
		}
		
		// Can current player afford unit
		if(_resource_count[(int)player] >= cost) 
		{
			// Record keeping
			_resource_count[(int)player] -= cost;
			_resource_spent[(int)player] += cost;
			_units_obtained[(int)player] += 1; 
				
			// Signal spawner and to approiate players container
			GameObject unit = _recruit_system.SpawnUnit(unit_type);

			unit.GetPhotonView().RPC("ParentUnitToCurrentPlayerContainer", PhotonTargets.AllBuffered);	

			// Put unit into appropriate player's container
			//this.photonView.RPC ("AddUnitToCurrentPlayersContainer", PhotonTargets.All, (int)unit_type);
		} 
	}

	// Return bool if player can purchase unit, if so do purchase
	// Based on unit type passed
	/// <summary>
	/// Purchases a unit, if the amount of resource is met then purchase as well as return true
	/// , else cannot purchase as well as return false. Then place unit into the scene into its 
	/// proper Player container with appropriate name, tag, and components.
	/// </summary>
	/// <returns><c>true</c>, if unit was recruited, <c>false</c> otherwise.</returns>
	/// <param name="player">Player.</param>
	/// <param name="unit_type">Unit_type.</param>
	public void RecruitUnitOnCurrentPlayer(UnitType unit_type)
	{
		RecruitUnit((Player)_current_player_turn, unit_type);
	}

	public void AddUnitToCurrentPlayerContainer(GameObject unit)
	{
		Debug.Log(string.Format("Got unit: {0}", unit.name));
		unit.transform.parent = _player_container[_current_player_turn].transform;
		UpdateFogOfWarComponents(unit);
	}

	/// <summary>
	/// Add unit into GameManager pool. It will distinguish whose turn it is and put them accoringly into a container.
	/// </summary>
	/// <param name="unit">Unit.</param>
	[RPC]
	void AddUnitToCurrentPlayersContainer(int unit_type, PhotonMessageInfo mi){
		// Signal spawner and to approiate players container
		GameObject unit = _recruit_system.SpawnUnit((UnitType)unit_type);

		if(_player_container[_current_player_turn].transform == null)
		{
			Debug.Log(string.Format("Player container for {0} doesn't exist!", (Player)_current_player_turn));
		}
		else
		{
			unit.transform.parent = _player_container[_current_player_turn].transform;
			Debug.Log(string.Format("_current_player_turn: {0}   _player_container[{0}]: {1}", _current_player_turn, _player_container[_current_player_turn]));
		}

		UpdateFogOfWarComponents(unit);
	}
	
	/// <summary>
	/// Determines if it is next players turn by checking if all units and leaders have used up all of their exhuast.
	/// Next player's turn can be interupted by an "End turn" type button,
	/// </summary>
	/// <returns><c>true</c> if is next players turn; otherwise, <c>false</c>.</returns>
	public bool IsNextPlayersTurn()
	{
		bool next = true;

		// Player should be in the scene. So it exist
		// Check if leader can not longer perform action
		// Leader still can do actions
		if(!__leader.GetComponent<BaseClass>().unit_status.status.Rest)
		{
			next = false;
		}
		// Leader is still active so don't need to check other units
		if(next)
		{
			// If units exist for current player, check if they are able to move
			BaseClass[] units_base_class = _player_container[__leader.GetPhotonView().owner.ID-1].GetComponentsInChildren<BaseClass>();
			for(int i=0;i<units_base_class.Length;++i)
			{	
				if(!units_base_class[i].unit_status.status.Rest)
				{
					next = false;
					i    = units_base_class.Length;
				}
			}
		}

		return next;
	}

	// Check with the current player using Photon to see if it is their turn
	public bool IsItMyTurn()
	{
		return (_current_player_turn == __leader.GetPhotonView().owner.ID-1);
	}

	// Method for allowing other player to take turn
	// This should enable all options for the next player in the queue
	// Disable the player's actions when they are done with their turn
	/// <summary>
	/// Nexts the players turn. As well as other stuff...
	/// </summary>
	public void NextPlayersTurn()
	{
		GameObject[] units = GetUnitsFromPlayer(CurrentPlayer);
		foreach(GameObject unit in units)
		{
			unit.GetComponent<BaseClass>().unit_status.Clean();
		}


		// Next player's turn
		_current_player_turn = (_current_player_turn + 1) % _total_players;
		this.photonView.RPC("ChangeTurn", PhotonTargets.AllBuffered, _current_player_turn);

		// Change camera accoring to player
		//_world_camera.ChangeCamera();
	}

	[RPC]
	void ChangeTurn(int cur)
	{
		_current_player_turn = cur;
		// When all player's have had their turn increment round number counter
		if(_current_player_turn == 0)
		{
			_round_num += 1;
		}
	}

	/// <summary>
	/// Get a unit and trys to apply the UnitController to it if possible. Already does at check if unit belongs to the appropriate player.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void SetUnitControllerActiveOn(ref GameObject unit)
	{
		_unit_controller.SetFocusOnUnit(ref unit);
	}

	/// <summary>
	/// Sets the unit controller active off. Killing input to move for the previous unit selected.
	/// </summary>
	public void SetUnitControllerActiveOff()
	{
		_unit_controller.ClearFocusUnit();
	}

	// Get current round
	/// <summary>
	/// Gets the current round.
	/// </summary>
	/// <returns>The current round.</returns>
	public int CurrentRound
	{
		get { return _round_num; }
	}

	// Get leader gameobjects in the scene
	/// <summary>
	/// Gets a desired player leader.
	/// </summary>
	/// <returns>The player leader.</returns>
	/// <param name="player">Player.</param>
	public GameObject GetPlayerLeader(Player player)
	{
		return __leader;
	}

	/// <summary>
	/// Sets the current focused unit's controller to on/off, which prevent the user from moving the unit.
	/// </summary>
	/// <param name="v">If set to <c>true</c> v.</param>
	public void SetFocusController (bool v)
	{
		// Get the unit controler object to allow if to move
		_unit_controller.GetComponent<UnitController>().SetIsControllable(true);
	}

	/// <summary>
	/// When a unit has died, the GameManager will record it, handle other functionality and dispose of the unit
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void UnitDied(GameObject unit)
	{
		if(!unit.GetComponent<BaseClass>().unit_status.status.Dead)
		{
			Debug.LogWarning(string.Format("{0} is not dead!", unit));
			return;
		}

		//_units_killed[_current_player_turn] += 1;

		//unit.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 90));

		// Currently, doesn't work since unit is not part of the pooling system initially
		//PoolingSystem.instance.Destroy(unit);
	}

	#endregion

	#region Record keeping
	// Return recap of the current match
	// Record stats at the end of the game
	// # of kills, resource collected/spent, rounds, timer?, # of units recruited, ranking algorithm
	/// <summary>
	/// Get all recorded scores that were kept track during this game's progress. Return as a single string.
	/// </summary>
	/// <returns>The recorded scores.</returns>
	public string[] GetRecordedScores()
	{
		string[] player_score = new string[_total_players + 1];
		player_score[0] = string.Format ("Total rounds:{0}\n" +
		                               "Time:{1}\n\n",
		                               _round_num, CurrentTime);

		/*HACK for (int i=1; i<_total_players + 1; ++i) 
		{
			player_score[i] = string.Format (
				"Player{0} Score:\n" +
				"Total resources obtained:{1}\n" +
				"Resources spent:{2}\n" +
				"Leader alive:{3}\n" +
				"Units obtained:{4}\n" +
				"Enemy units killed:{5}\n\n", 
				i + 1, _resources_obtained[i], _resource_spent[i], _leaders[i].GetComponent<BaseClass>().unit_status.status, _units_obtained[i], _units_killed[i]);
		}*/
		
		return player_score;
	}
	#endregion

	/// <summary>
	/// Set _game_init = false. Preventing GameManager from working. To show that something is missing that is necessary for the GameManager to know
	/// Also display warning to fix error
	/// </summary>
	private void ForceQuit()
	{
		Destroy();
		Debug.LogError("Quiting GameManager. Above message provides the error.");
	}
}
