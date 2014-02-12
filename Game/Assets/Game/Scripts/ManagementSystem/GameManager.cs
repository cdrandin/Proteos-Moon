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



	// Constructor
	public static void Init(int num_of_players, int who_goes_first)
	{
		// Valid player limit
		if(who_goes_first == 0 || (who_goes_first > num_of_players) || num_of_players > 4)
		{
			Debug.LogError("Problem in GameManager.cs");
			return ;
		}

		// One time calls of basic init.
		_player_turn_order = new PlayerTurn[num_of_players];
		total_players = num_of_players;

		// Associate a player 
		for(int i=0;i<num_of_players;++i)
		{
			if((int)PlayerTurn.Player1 == i)
				_player_turn_order[i] = PlayerTurn.Player1;
			else if((int)PlayerTurn.Player2 == i)
				_player_turn_order[i] = PlayerTurn.Player2;
			else if((int)PlayerTurn.Player3 == i)
				_player_turn_order[i] = PlayerTurn.Player3;
			else if((int)PlayerTurn.Player4 == i)
				_player_turn_order[i] = PlayerTurn.Player4;
		}

		// Allocate correct number of resource counters
		_resource_count = new int[num_of_players];

		for(int i=0;i<num_of_players;++i)
			_resource_count[i] = 0;

		// Player order	
		GenerateTurnSequence();
		_resource_count[0] = 100;
		_resource_count[1] = 200;
		Debug.Log(GetLeadInResources());
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

	// Get which player's is taking there turn currently
	public static PlayerTurn GetCurrentPlayerTurn()
	{
		return _player_turn_order[_current_player_turn]; 
	}

	// Get who ever is winning currently in terms of resources
	public static PlayerTurn GetLeadInResources()
	{
		int most = _resource_count[0];
		Debug.Log(_resource_count.Length);
		for(int i=1;i<total_players;++i)
		{
			if(_resource_count[i] > _resource_count[most])
				most = i;
		}
		Debug.Log(most);

		return _player_turn_order[most];
	}

}

/*
public class GameManager : MonoBehaviour 
{
	private enum PlayerTurn
	{
		Player1 = 1,
		Player2,
		Player3,
		Player4
	}
	private PlayerTurn _player_turn;

	// private MovementSystem _movement_system; let a unit move
	// ACTIONS:
	//    private CombatSystem _combat_system; fighting, abilities(combat and terrain use)
	//    private RecruitSystem _recruit_system;
	void Awake ()
	{
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
*/