using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour 
{
	public int num_of_players;

	void Awake() 
	{
		GameManager.Init(num_of_players, RandomPlayerFirst(num_of_players));
	}

	// Use this for initialization
	
	void Start()
	{
	}

	// Update is called once per frame
	void Update() 
	{

	}

	int RandomPlayerFirst(int number_of_players)
	{
		return Random.Range(1,number_of_players+1);
	}

	void Reset ()
	{
		num_of_players = 2;
	}
}
