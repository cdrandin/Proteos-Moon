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

	// Keep track of player turn order and number of players
	public static Player[] _player_turn_order;
	private static int _current_player_turn;
	public static int total_players;

	// Winning conditions
	private static int[] _resource_count;
	private static int _max_resource;
	private static bool[] _leaders_alive;
	//private static ^LeaderScript^[] _leaders_scripts;

	// Who has won
	private static Player _winner;

	// Recording scores
	private static int[] _resources_obtained;
	private static int[] _resource_spent;
	private static int[] _units_obtained;
	private static int _round_num;
	private static float _timer;

	private static float _base_time;

	// Constructor
	public static void Init(int num_of_players, int who_goes_first, int resource_win_count)
	{
		// Valid player limit
		if(who_goes_first==0||(who_goes_first>num_of_players)||num_of_players>4)
		{
			Debug.LogError("Player limit reached!\nProblem in GameManager.cs");
			return;
		}

		total_players = num_of_players;

		_winner = Player.NONE;

		// What is the max number of resources required to win
		_max_resource = resource_win_count;

		// One time calls of basic init.
		_player_turn_order = new Player[total_players];

		// Allocate correct number of resource counters
		_resource_count = new int[total_players];
		_resource_spent = new int[total_players];
		_resources_obtained = new int[total_players];

		// Allocated correct number of leader counters
		_leaders_alive = new bool[total_players];

		// How many units ech player purchased
		_units_obtained = new int[total_players];

		// Pointer to the leader script, to keep track of hp
		// _leader_script = new ^LeaderScript^[total_players];

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

	// Updated leader status for all players
	public static void UpdateStatusOfAllLeaders()
	{
		foreach(Player player in _player_turn_order)
			UpdateStatusOfLeaderFrom(player);
	}

	// Pass player enum and find leader, get status
	public static void UpdateStatusOfLeaderFrom(Player player)
	{
		// if(_leaders_scripts[(int}player.GetHP == 0)
		//     _leaders_alive[(int)player] = false;
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

	public static Player GetWiner()
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

	// Return bool if player can purchase unit, if so do purchase
	public static bool RecruitUnit(Player player, int amount)
	{
		if(_resource_count[(int)player] >= amount)
		{
			_resource_count[(int)player] -= amount;
			_resource_spent[(int)player] += amount;
			++_units_obtained[(int)player]; 
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

	// Keep track of each player's leader, making sure who has lost the game if their leader has died
	public static void InitPlayersLeader()
	{
		// Find each player's leader. All leaders alive.
		// _leader_script = GameObject.FindGameObjectsWithType(typeof(^LeaderScript^));
	}

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
		_timer = 0.0f;

		for(int i=0;i<_resource_count.Length;++i)
		{
			_resource_count[i] = 0;
			_resource_spent[i] = 0;
			_resources_obtained[i] = 0;
			_units_obtained[i] = 0;
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
				i+1, _resources_obtained[i], _resource_spent[i], _leaders_alive[i], -1, -1);
		}

		string scores = string.Format("Total rounds:{0}\n" +
		                              "Time:{1}\n",
		                              _round_num, GetCurrentTime() );
		                              

		foreach(string play in player_score)
			scores += play;

		return scores;
	}
}