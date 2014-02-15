using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* GameManager - keeps track of the game state and control
 * Controlling the game 
 * Check when next player's turn should be or when current player passes their turn
 *  - Keep track of rounds, turn order, timer, player's units resources, leader alive?, who won, etc
 *  - Check if unit is dead, if so move to pool
 *  - Control recruit position, next to leader
 * Should paucs game or unpause
 * Keep track of network between players, if we add *networking* capabilities
 * 
 * Keep track of buffs, debuff, passive that should be received from other units
 * 
 * Record stats at the end of the game
 * 	# of kills, resource collected/spent, rounds, timer?, # of units recruited, ranking algorithm
 */

public static class GameManager 
{
	// Player turn enum
	public enum PlayerTurn
	{
		Player1 = 0,
		Player2,
		Player3,
		Player4
	}
	public static PlayerTurn[] _player_turn_order;
	private static int _current_player_turn;
	public static int total_players;

	private static int[] _resource_count;

	private static int _round_num;
	
	// Constructor
	public static void Init(int num_of_players, int who_goes_first)
	{
		// Valid player limit
		if(who_goes_first==0||(who_goes_first>num_of_players)||num_of_players>4)
		{
			Debug.LogError("Player limit reached!\nProblem in GameManager.cs");
			return;
		}

		total_players = num_of_players;

		// One time calls of basic init.
		_player_turn_order = new PlayerTurn[total_players];

		// Allocate correct number of resource counters
		_resource_count = new int[total_players];

		// Associate a player 
		for(int i=0;i<num_of_players;++i)
		{
			_player_turn_order[i] = (PlayerTurn)i;
		}

		// Keep track of each player's leader or have outside script signal this script to keep track

		ResetGameState();
	}

	// Get which player's is taking there turn currently
	public static PlayerTurn GetCurrentPlayerTurn()
	{
		return _player_turn_order[_current_player_turn]; 
	}

	/*
	 * Win Conditions 
	 */
	// Get who ever is winning currently in terms of resources
	public static PlayerTurn GetLeadInResources()
	{
		int most = 0;

		for(int i=1;i<total_players;++i)
		{
			if(_resource_count[i] > _resource_count[most])
				most = i;
		}

		return _player_turn_order[most];
	}

	/*
	 * End Wind Conditions
	 */

	// Add X amount of points to resource counter array, according to player
	public static void AddResources(PlayerTurn player_turn, int amount)
	{
		_resource_count[(int)player_turn] += amount;

		if(_resource_count[(int)player_turn]<0)
			_resource_count[(int)player_turn] = 0;
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

	}

	// Reset variables that are required to keep track of info during the game
	public static void ResetGameState()
	{
		// Reset round number
		_round_num = 0;

		// Blank resource counts
		ResetResourceCount();
		
		// Player order	
		GenerateTurnSequence();
	}

	// Blank resource counts
	private static void ResetResourceCount()
	{
		for(int i=0;i<_resource_count.Length;++i)
			_resource_count[i] = 0;
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
				_player_turn_order[i] = (PlayerTurn)t;
				++i;
			}
		}
	}
}