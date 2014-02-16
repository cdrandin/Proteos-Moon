using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour 
{
	public int num_of_players;
	public int resource_limit;

	private bool change;
	void Awake() 
	{
		GameManager.Init(num_of_players, RandomPlayerFirst(num_of_players), resource_limit);
	}

	// Use this for initialization
	
	void Start()
	{
		change = true;
	}

	// Update is called once per frame
	void Update() 
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

	int RandomPlayerFirst(int number_of_players)
	{
		return Random.Range(1,number_of_players+1);
	}

	void Reset ()
	{
		num_of_players = 2;
		resource_limit = 500;
	}
}
