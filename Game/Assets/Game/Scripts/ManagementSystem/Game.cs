using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour 
{
	public int num_of_players;
	public int resource_limit;

	private bool change;

	/* 
	 * Variables used for testing GameManager
	 */
	public bool testing;
	private bool _manager_init;

	void Awake() 
	{
		_manager_init = false;

		if(!testing)
		{
			GameManager.Init(num_of_players, RandomFirstPlayer(num_of_players), resource_limit);
			_manager_init = true;
		}
	}

	// Use this for initialization

	void Start()
	{
		change = true;
	}

	// Update is called once per frame
	void Update() 
	{
		// Manager is initialized
		if(_manager_init)
		{
			// Next players turn
			if(Input.GetKeyDown(KeyCode.Space))
			{
				GameManager.NextPlayersTurn();
				change = true;
			}

			if(change)
			{
				Debug.Log(string.Format("It is now {0}'s turn!", GameManager.GetCurrentPlayer()));
				change = false;
			}
		}
	}


	int RandomFirstPlayer(int number_of_players)
	{
		return Random.Range(1,number_of_players+1);
	}

	void Reset ()
	{
		num_of_players = 2;
		resource_limit = 500;
	}


	/*
	 * Only used for testing GameManager
	 */

	void OnGUI()
	{
		// Only display when testing
		if(testing)
		{
			if(MakeButton(0,0,"Start GameManager") && !_manager_init)
			{
				GameManager.Init(num_of_players, RandomFirstPlayer(num_of_players), resource_limit);
				_manager_init = true;
			}

			else if(MakeButton(0,50,"End GameManager"))
			{
				GameManager.ResetGameState();
				_manager_init = false;
			}

			// Manager is on, put up some options
			if(_manager_init)
			{	
				float half = Screen.width/2;

				if(MakeButton(half, 0,"Round #"))
					Debug.Log(string.Format("Current round: {0}", GameManager.GetCurrentRound()));
				else if(MakeButton(half, 50, "Timer"))
					Debug.Log(string.Format("Current time: {0}", GameManager.GetCurrentTime()));
			}
		}
	}

	bool MakeButton(float left, float top, string name)
	{
		return GUI.Button(new Rect(left,top, 150,50), name);
	}
	
}
