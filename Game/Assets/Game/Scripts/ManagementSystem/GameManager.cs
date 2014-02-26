using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* GameManager - keeps track of the game state and control
 * Controlling the game 
 *  X Check when next player's turn should be or when current player passes their turn
 *  X Keep track of rounds, turn order, timer, player's units resources, leader alive?, who won, etc
 *  - Control recruit position, next to leader
 *  - Should pause game or unpause
 *  -(MAYBE) Keep track of network between players, if we add *networking* capabilities
 * 
 *  -(MAYBE) Keep track of buffs, debuff, passive that should be received from other units
 * 
 * Record stats at the end of the game
 * 	# of kills, resource collected/spent, rounds, timer?, # of units recruited, ranking algorithm
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

	private static bool _game_init;

	// Keep track of player turn order and number of players
	public static Player[] _player_turn_order;
	private static int _current_player_turn;
	public static int total_players;

	// Pointer to script that contains unit specific cost
	private static UnitCost _unit_cost;
	private static RecruitSystem _rs;

	// Winning conditions
	private static int[] _resource_count;
	private static int _max_resource;
	private static bool[] _leaders_alive; // Find better way

	private static GameObject[] _leaders;

	// Maybe good for now, might keep classtype for other stuff as needed
	private static List<GameObject>[] _player_units; // Keep track each unit with respect to each player's unit

	// Who has won
	private static Player _winner;

	// Recording scores
	private static int[] _resources_obtained;
	private static int[] _resource_spent;
	private static int[] _units_obtained;
	private static int _round_num;
	//HACK private static float _timer;
	
	private static float _base_time;

	private static void Awake ()
	{
		_game_init = false;
	}

	public static bool IsOn()
	{
		return _game_init;
	}

	// Constructor
	public static void Init(int num_of_players, int who_goes_first, int resource_win_count, UnitCost unit_cost)
	{
		_game_init = true;

		// Valid player limit
		if(who_goes_first == 0 || (who_goes_first > num_of_players) || num_of_players > 4)
		{
			Debug.LogError("Player limit reached!\nProblem in GameManager.cs");
			return;
		}

		total_players = num_of_players;

		// How many lists is needed, based on player total
		_player_units = new List<GameObject>[total_players];

		_rs = GameObject.FindGameObjectWithTag("GameController").GetComponent<RecruitSystem>();

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

		// How many units ech player purchased
		_units_obtained = new int[total_players];

		// Pointer to the leader script, to keep track of hp
		// _leader_script = new ^LeaderScript^[total_players];

		// Get player units on the screen, for now assuming leaders are there before the game starts
		InitPlayersLeader();
		//InitPlayersUnits();

		ResetGameState();

		StartTimer();
	}

	// Get which player's is taking there turn currently
	public static Player GetCurrentPlayer()
	{
		return _player_turn_order[_current_player_turn]; 
	}

	public static int GetResourceFrom(Player player)
	{
		return _resource_count[(int)player];
	}

	/*
	 * Win Conditions/helper functions
	 */
	// Get who ever is winning currently in terms of resources
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

	private static int GetSurvivingLeaderCount()
	{
		int alive = 0;

		foreach(bool leader in _leaders_alive)
		{
			if(leader)
				++alive;
		}
		return alive;
	}

	// Check winning conditions. See if any player won
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
			// Search for the one leader that is alive associated with the player
			for(int i=0;i<_leaders_alive.Length;++i)
			{
				if(_leaders_alive[i])
				{
					_winner = (Player)i;
					return true;
				}
			}
		}
		return false;
	}

	public static Player GetWinner()
	{
		return _winner;
	}

	/*
	 * End Win Conditions/helper functions
	 */

	// Add X amount of points to resource counter array, according to player
	public static void AddResources(Player player_turn, int amount)
	{
		_resource_count[(int)player_turn] += amount;
		_resources_obtained[(int)player_turn] += amount;
		
		if(_resource_count[(int)player_turn]<0)
			_resource_count[(int)player_turn] = 0;
	}

	// Add X amount of points to resource counter array, according to player
	public static void AddResourcesToCurrentPlayer(int amount)
	{
		_resource_count[_current_player_turn] += amount;
		_resources_obtained[_current_player_turn] += amount;
		
		if(_resource_count[_current_player_turn]<0)
			_resource_count[_current_player_turn] = 0;
	}

	// Return bool if player can purchase unit, if so do purchase
	// Based on unit type passed
	public static bool RecruitUnit(Player player, UnitType unit_type)
	{
		int cost;

		switch(unit_type)
		{
		case UnitType.Arcane:
			cost = _unit_cost.Arcane;
			break;
		case UnitType.Braver:
			cost = _unit_cost.Braver;
			break;
		case UnitType.Scout:
			cost = _unit_cost.Scout;
			break;
		case UnitType.Sniper:
			cost = _unit_cost.Sniper;
			break;
		case UnitType.Titan:
			cost = _unit_cost.Titan;
			break;
		case UnitType.Vangaurd:
			cost = _unit_cost.Vangaurd;
			break;
		default:
			Debug.LogError(string.Format("Unit type: {0} does not have an associated cost to it!", unit_type));
			cost = -1;
			break;
		}

		if(_resource_count[(int)player] >= cost)
		{
			_resource_count[(int)player] -= cost;
			_resource_spent[(int)player] += cost;
			++_units_obtained[(int)player]; 
			_rs.SpawnUnit(unit_type);
			return true;
		}
		else
			return false;
	}
	// Method for allowing other player to take turn
	// This should enable all options for the next player in the queue
	// Disable the player's actions when they are done with their turn
	public static void NextPlayersTurn()
	{
		// Disable unit selection for current player
		
		// Enable Fog of War for other player's perspective
		
		// Change player's camera perspective 

		
		// Next player's turn
		_current_player_turn = (_current_player_turn+1)%total_players;

		// When all player's have had their turn increment round number counter
		if((_current_player_turn+1)%total_players == 0)
			++_round_num;
	}

	// Get current round
	public static int GetCurrentRound()
	{
		return _round_num;
	}

	// Get leader gameobjects in the scene
	public static GameObject GetPlayerLeader(Player player)
	{
		return _leaders[(int)player];
	}

	// Keep track of each player's leader, making sure who has lost the game if their leader has died
	private static void InitPlayersLeader()
	{
		// Get leaders
		GameObject[] _all_leaders = GameObject.FindGameObjectsWithTag("Leader"); 

		// Number of allocated leaders and number of leaders found should be the same, else a leader is missing
		if(_leaders.Length != _all_leaders.Length)
		{
			Debug.LogError(string.Format("A leader is missing!! Should be a total of {0} leaders.", total_players));
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

	// Keep track of units with their corresponding player
	// Not really needed, but it is there
	/*
	private static void InitPlayersUnits()
	{
		// This point, all units are obtained
		GameObject[] all_units = GameObject.FindGameObjectsWithTag("Unit");

		// Sperate based on which player owns which unit
		foreach(GameObject unit in all_units)
		{
			if(unit.transform.parent.tag == "Player1")
				_player_units[0].Add(unit);

			else if(unit.transform.parent.tag == "Player2")
				_player_units[1].Add(unit);

			else if(unit.transform.parent.tag == "Player3")
				_player_units[2].Add(unit);

			else if(unit.transform.parent.tag == "Player4")
				_player_units[3].Add(unit);

			else
				Debug.LogError(string.Format("Unknown player tag! >> {0} <<", unit.transform.tag));
		}
	}
	*/

	private static void StartTimer()
	{
		_base_time = Time.time;
	}

	public static float GetCurrentTime()
	{
		return Time.time - _base_time;
	}

	// Reset variables that are required to keep track of info during the game
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
	/// Set _game_init = false. Preventing GameManager from working. To show that something is missing that is necessary for the GameManager to know
	/// Also display warning to fix error
	/// </summary>
	private static void ForceQuit()
	{
		_game_init = false;
		Debug.LogError("Quiting GameManager. Above message provides the error.");
	}
}