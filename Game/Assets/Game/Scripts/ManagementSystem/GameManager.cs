using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
 *   	# of kills, resource collected/spent, rounds, timer?, # of units recruited, (?ranking algorithm?)
 */

public static class GameManager 
{
	// Player turn enum
	public enum Player
	{
		Player1 = 0,
		Player2,
		Player3,
		Player4,
		NONE
	}

	// Determine whether the GameManager is active or not.
	private static bool _game_init;

	// Keep track of player turn order and number of players
	public static Player[] _player_turn_order;
	private static int _current_player_turn;
	public static int total_players;

	// Pointer to script that contains unit specific scripts
	private static UnitCost _unit_cost;
	private static RecruitSystem _rs;
	private static UnitController _uc;

	// Winning conditions
	private static int[] _resource_count;
	private static int _max_resource;
	private static bool[] _leaders_alive; // Find better way

	// Keep track of each player's unit accordingly
	private static GameObject[] _leaders;
	
	// Pointer to containers, units will be rooted to them
	// The gameobject will contain the objects of that player's units and leader as its children nodes
	private static GameObject[] _player_container;

	// Who has won
	private static Player _winner;

	// Recording scores
	private static int[] _resources_obtained;
	private static int[] _resource_spent;
	private static int[] _units_obtained;
	private static int _round_num;
	//HACK private static float _timer;
	
	private static float _base_time;

	private static WorldCameraModified _wcm;

	private static void Awake ()
	{
		_game_init = false;
	}

	/// <summary>
	/// Determines if the GameManager is on.
	/// </summary>
	/// <returns><c>true</c> if is on; otherwise, <c>false</c>.</returns>
	public static bool IsOn()
	{
		return _game_init;
	}

	// Constructor
	/// <summary>
	/// Similiar to that of a constructor. Initializes the GameManager with all needed functionality already provided in the back end.
	/// Should only be called once!
	/// </summary>
	/// <param name="num_of_players">Num_of_players.</param>
	/// <param name="who_goes_first">Who_goes_first.</param>
	/// <param name="resource_win_count">Resource_win_count.</param>
	/// <param name="unit_cost">Unit_cost.</param>
	public static void Init(int num_of_players, int who_goes_first, int resource_win_count, UnitCost unit_cost)
	{
		if(IsOn())
			return;

		_game_init = true;

		// Valid player limit
		if((who_goes_first < 1) || (who_goes_first > num_of_players) || (num_of_players > 4))
		{
			_game_init = false;
			Debug.LogError("Player limit reached!\nProblem in GameManager.cs");
			return;
		}

		total_players = num_of_players;

		_rs = GameObject.FindGameObjectWithTag("GameController").GetComponent<RecruitSystem>();
		if(_rs == null)
			Debug.LogWarning("Recruit System missing reference");

		_uc = GameObject.FindGameObjectWithTag("UnitController").GetComponent<UnitController>();
		if(_uc == null)
			Debug.LogWarning("Unit Controller missing reference");

		_wcm = GameObject.Find("WorldCamera").GetComponent<WorldCameraModified>();
		if(_wcm == null)
			Debug.LogWarning("World Camera missing reference");

		_winner = Player.NONE;

		// Keep track of cost
		_unit_cost = unit_cost;

		// What is the max number of resources required to win
		_max_resource = resource_win_count;

		// One time calls of basic init.
		_player_turn_order  = new Player[total_players];

		// Allocate correct number of resource counters
		_resource_count     = new int[total_players];
		_resource_spent     = new int[total_players];
		_resources_obtained = new int[total_players];

		// Allocated correct number of leader counters
		_leaders_alive = new bool[total_players];

		// Allocate correct number of leaders
		_leaders = new GameObject[total_players];

		// Allocate correct number of player containers for their units/leaders
		_player_container = new GameObject[total_players];

		// How many units ech player purchased
		_units_obtained = new int[total_players];

		// Pointer to the leader script, to keep track of hp
		// _leader_script = new ^LeaderScript^[total_players];

		// Get player units on the screen, for now assuming leaders are there before the game starts
		InitPlayerContainers();
		InitPlayersLeader();
		//InitPlayersUnits();

		ResetGameState();
	
		StartTimer();

		// Set camera
		_wcm.ChangeCamera();
	}

	// Get which player's is taking there turn currently
	/// <summary>
	/// Returns the current player's turn as an enum of type Player
	/// Example. Player.Player1
	/// </summary>
	/// <returns>The current player.</returns>
	public static Player GetCurrentPlayer()
	{
		return _player_turn_order[_current_player_turn]; 
	}

	public static Player GetPlayer(int player)
	{
		return (Player)player;
	}

	// Get resource amount from a player
	/// <summary>
	/// Gets the resource from a player.
	/// </summary>
	/// <returns>The resource from.</returns>
	/// <param name="player">Player.</param>
	public static int GetResourceFrom(Player player)
	{
		return _resource_count[(int)player];
	}

	/// <summary>
	/// Get the max number of resources needed to be aquired for this match
	/// </summary>
	/// <returns>The max resource limit.</returns>
	public static int GetMaxResourceLimit()
	{
		return _max_resource;
	}

	/* ###
	 * Win Conditions
	 * ###
	 */
	// Get who ever is winning currently in terms of resources
	/// <summary>
	/// Gets the lead in terms of resources. Returns a Player type.
	/// </summary>
	/// <returns>The lead in resources.</returns>
	public static Player GetLeadInResources()
	{
		int most = 0;

		for(int i=1;i<total_players;++i)
		{
			if(_resource_count[i] > _resource_count[most])
				most = i;
		}

		return _player_turn_order[most];
	}

	// Get a count for how many leaders are currently still alive
	private static int GetSurvivingLeaderCount()
	{
		int alive = 0;

		foreach(GameObject leader in _leaders)
		{
			if(leader.GetComponent<LeaderClass>().leader.hp != 0)
				++alive;
		}
		return alive;
	}

	// Check winning conditions. See if any player won
	/// <summary>
	/// Determines if is there A winner.
	/// </summary>
	/// <returns><c>true</c> if is there A winner; otherwise, <c>false</c>.</returns>
	public static bool IsThereAWinner()
	{
		Player lead = GetLeadInResources();

		// Win by resource
		if(_resource_count[(int)lead] >= _max_resource)
		{
			_winner = lead;
			return true;
		}

		// Win by having 1 leader survive/killing off other leaders
		else if(GetSurvivingLeaderCount() == 1)
		{
			for(int i=1;i<=_leaders.Length;++i)
			{
				if(_leaders[i].GetComponent<UnitStatus>().status != Status.Dead)
				{
					_winner = (Player)i;
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Gets the winner of the match.
	/// </summary>
	/// <returns>The winner.</returns>
	public static Player GetWinner()
	{
		return _winner;
	}

	/* ###
	 * End Win Conditions/helper functions
	 * ###
	 */

	// Add X amount of points to resource counter array, according to player
	/// <summary>
	/// Adds the resources according to the amount, determined by a Player type passed.
	/// </summary>
	/// <param name="player_turn">Player_turn.</param>
	/// <param name="amount">Amount.</param>
	public static void AddResources(Player player_turn, int amount)
	{
		_resource_count[(int)player_turn] += amount;
		_resources_obtained[(int)player_turn] += amount;
		
		if(_resource_count[(int)player_turn]<0)
			_resource_count[(int)player_turn] = 0;
	}

	// Add X amount of points to resource counter array, according to player
	/// <summary>
	/// Adds the resources to current player by a amount.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public static void AddResourcesToCurrentPlayer(int amount)
	{
		_resource_count[(int)GetCurrentPlayer()] += amount;
		_resources_obtained[(int)GetCurrentPlayer()] += amount;
		
		if(_resource_count[(int)GetCurrentPlayer()]<0)
			_resource_count[(int)GetCurrentPlayer()] = 0;
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
	public static bool RecruitUnit(Player player, UnitType unit_type)
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
			++_units_obtained[(int)player]; 

			// Signal spawner and to approiate players container
			GameObject unit = _rs.SpawnUnit(unit_type);

			// Put unit into appropriate player's container
			AddUnitToCurrentPlayersContainer(unit);

			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// Determines if it is next players turn by checking if all units and leaders have used up all of their exhuast.
	/// Next player's turn can be interupted by an "End turn" type button,
	/// </summary>
	/// <returns><c>true</c> if is next players turn; otherwise, <c>false</c>.</returns>
	public static bool IsNextPlayersTurn()
	{
		// Player should be in the scene. So it exist
		// Check if leader can not longer perform action
		if(_leaders[_current_player_turn].GetComponent<UnitStatus>().status != Status.Resting)
			return false;

		// If units exist for current player, check if they are able to move
		UnitStatus[] units = _player_container[_current_player_turn].GetComponentsInChildren<UnitStatus>();
		foreach(UnitStatus unit in units)
		{
			if(unit.status != Status.Resting)
				return false;
		}
		return true;
	}

	// Method for allowing other player to take turn
	// This should enable all options for the next player in the queue
	// Disable the player's actions when they are done with their turn
	/// <summary>
	/// Nexts the players turn. As well as other stuff...
	/// </summary>
	public static void NextPlayersTurn()
	{
		// Disable unit selection for current player
		
		// Enable Fog of War for other player's perspective
		
		// Change player's camera perspective 

		
		// Next player's turn
		_current_player_turn = (_current_player_turn+1)%total_players;

		// Unfocus any unit
		GameManager.SetUnitControllerActiveOff();

		// When all player's have had their turn increment round number counter
		if((_current_player_turn+1)%total_players == 0)
			++_round_num;

		// Change camera accoring to player
		_wcm.ChangeCamera();
	}

	// Get current round
	/// <summary>
	/// Gets the current round.
	/// </summary>
	/// <returns>The current round.</returns>
	public static int GetCurrentRound()
	{
		return _round_num;
	}

	// Get leader gameobjects in the scene
	/// <summary>
	/// Gets a desired player leader.
	/// </summary>
	/// <returns>The player leader.</returns>
	/// <param name="player">Player.</param>
	public static GameObject GetPlayerLeader(Player player)
	{
		return _leaders[(int)player];
	}

	/// <summary>
	/// Get a unit and trys to apply the UnitController to it if possible. Already does at check if unit belongs to the appropriate player.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public static void SetUnitControllerActiveOn(ref GameObject unit)
	{
		_uc.SetFocusOnUnit(ref unit);
	}

	/// <summary>
	/// Sets the unit controller active off. Killing input to move for the previous unit selected.
	/// </summary>
	public static void SetUnitControllerActiveOff()
	{
		_uc.ClearFocusUnit();
	}

	// Point to player container or create one if needed, based on the number of players
	private static void InitPlayerContainers()
	{
		GameObject obj;

		for(int i=1;i<=total_players;++i)
		{
			// Find player
			obj = GameObject.FindGameObjectWithTag(string.Format("Player{0}", i));

			// Player container exist, point to it
			if(obj != null)
				_player_container[i-1] = obj as GameObject;

			// Player container doesn't exist, uh-oh!
			else
				Debug.LogError(string.Format("Container for player{0} is missing! Stuff will break. Add it you fool!", i));
		}
	}

	// Keep track of each player's leader, making sure who has lost the game if their leader has died
	private static void InitPlayersLeader()
	{
		// Get leaders
		GameObject[] _all_leaders = GameObject.FindGameObjectsWithTag("Leader"); 

		// Number of allocated leaders and number of leaders found should be the same, else a leader is missing
		if(_leaders.Length != _all_leaders.Length)
		{
			Debug.LogError(string.Format("A leader is missing!! Should be a total of {0} leaders currently there is {1}.", total_players, _all_leaders.Length));
			ForceQuit();
			return;
		}

		// Distinguish which leader belongs to which player
		foreach(GameObject leader in _all_leaders)
		{
			if(leader.transform.parent == null)
				Debug.LogError(string.Format("Missing parent object for {0}. Parent object should be tagged \"Player#\"", leader.name));

			if(leader.transform.parent.tag == "Player1")
				_leaders[0] = leader;

			else if(leader.transform.parent.tag == "Player2")
				_leaders[1] = leader;
			
			else if(leader.transform.parent.tag == "Player3")
				_leaders[2] = leader;
			
			else if(leader.transform.parent.tag == "Player4")
				_leaders[3] = leader;

			else
				Debug.LogError(string.Format("Unknown player tag! >> {0} <<", leader.transform.tag));
		}
	}
	
	/// <summary>
	/// Add unit into GameManager pool. It will distinguish whose turn it is and put them accoringly into a container.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public static void AddUnitToCurrentPlayersContainer(GameObject unit)
	{
		unit.transform.parent = _player_container[(int)GetCurrentPlayer()].transform;
	}

	private static void StartTimer()
	{
		_base_time = Time.time;
	}

	/// <summary>
	/// Gets the current time.
	/// </summary>
	/// <returns>The current time.</returns>
	public static float GetCurrentTime()
	{
		return Time.time - _base_time;
	}

	// Reset variables that are required to keep track of info during the game
	/// <summary>
	/// Resets the state of the game. Typically used for when the game is over or restart.
	/// </summary>
	public static void ResetGameState()
	{
		// Associate a player 
		for(int i=0;i<total_players;++i)
		{
			_player_turn_order[i] = (Player)i;
		}

		// Reset values that are used for recording players numbers
		ResetRecordings();

		// Reset leaders to be alive
		ResetLeaders();

		// Player order	
		GenerateTurnSequence();

		_winner = Player.NONE;
	}

	// Blank resource counts
	private static void ResetRecordings()
	{
		// Reset round number
		_round_num = 0;

		// Reset timer
		//HACK _timer = 0.0f;

		for(int i=0;i<_resource_count.Length;++i)
		{
			_resource_count[i]     = 0;
			_resource_spent[i]     = 0;
			_resources_obtained[i] = 0;
			_units_obtained[i]     = 0;
		}
	}

	// All leaders are alive
	private static void ResetLeaders()
	{
		for(int i=0;i<_leaders_alive.Length;++i)
			_leaders_alive[i] = true;
	}
	
	// Currently, shuffles player's turn order
	private static void GenerateTurnSequence()
	{// Sloppy way of doing this
		
		List<int> tmp = new List<int>();
		int i = 0;
		
		while(i<total_players)
		{
			int t = Random.Range(0,total_players);
			if(!tmp.Contains(t))
			{
				tmp.Add(t);
				_player_turn_order[i] = (Player)t;
				++i;
			}
		}
	}
	
	// Return recap of the current match
	// Record stats at the end of the game
	// # of kills, resource collected/spent, rounds, timer?, # of units recruited, ranking algorithm
	/// <summary>
	/// Get all recorded scores that were kept track during this game's progress. Return as a single string.
	/// </summary>
	/// <returns>The recorded scores.</returns>
	public static string GetRecordedScores()
	{
		string[] player_score = new string[total_players];
		for(int i=0;i<total_players;++i)
		{
			player_score[i] = string.Format(
				"Player{0} Score:\n" +
				"Total resources obtained:{1}\n" +
				"Resources spent:{2}\n" +
				"Leader alive:{3}\n" +
				"Units obtained:{4}\n" +
				"Enemy units killed:{5}\n\n", 
				i+1, _resources_obtained[i], _resource_spent[i], _leaders_alive[i], _units_obtained[i], -1);
		}

		string scores = string.Format("Total rounds:{0}\n" +
		                              "Time:{1}\n\n",
		                              _round_num, GetCurrentTime());
		                              
		// Stich recorded scores into string
		foreach(string play in player_score)
			scores += play;

		return scores;
	}

	/// <summary>
	/// Gets the current focused unit in which a player has clicked on, awaiting an action.
	/// </summary>
	/// <returns>The current focus.</returns>
	public static GameObject GetCurrentFocus()
	{
		_uc.GetUnitControllerFocus();
	}

	/// <summary>
	/// Set _game_init = false. Preventing GameManager from working. To show that something is missing that is necessary for the GameManager to know
	/// Also display warning to fix error
	/// </summary>
	private static void ForceQuit()
	{
		_game_init = false;
		Debug.LogError("Quiting GameManager. Above message provides the error.");
	}
}