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
	private GUIText _resource;
	private GUIText _recruit;

	void Awake() 
	{
		_manager_init = false;
				
		_resource = GameObject.Find("Resource_GUIText").GetComponent<GUIText>();
		_recruit  = GameObject.Find("Recruit_GUIText").GetComponent<GUIText>();

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

				_resource.transform.position = new Vector3(0.05f, 0.05f, 0.0f);
				_resource.fontSize = 16;
				_resource.text = string.Format("Current player: {0} at {1} Resources", 
				                               GameManager.GetCurrentPlayer(), GameManager.GetResourceFrom(GameManager.GetCurrentPlayer()).ToString());
			}

			else if(MakeButton(0, 50, "End GameManager"))
			{
				GameManager.ResetGameState();
				_manager_init = false;
			}

			// Manager is on, put up some options
			if(_manager_init)
			{	
				float half = Screen.width/2;
				string recruit_text = "Recently purchased";
				string recruit_fail = "Could not purchase";

				if(MakeButton(half, 0, "Next player's turn"))
				{
					GameManager.NextPlayersTurn();
					_resource.text = string.Format("Current player: {0} at {1} Resources", 
					                               GameManager.GetCurrentPlayer(), GameManager.GetResourceFrom(GameManager.GetCurrentPlayer()).ToString());
				}

				else if(MakeButton(half, 50,"Round\t #"))
					Debug.Log(string.Format("Current round: {0}", GameManager.GetCurrentRound()));

				else if(MakeButton(half, 100, "Timer"))
					Debug.Log(string.Format("Current time: {0}", GameManager.GetCurrentTime()));

				else if(MakeButton(half, 150, string.Format("Add 50 resource pts\n to {0}", GameManager.GetCurrentPlayer())))
				{
					GameManager.AddResources(GameManager.GetCurrentPlayer(),50);
					_resource.text = string.Format("Current player: {0} at {1} Resources", 
					                               GameManager.GetCurrentPlayer(), GameManager.GetResourceFrom(GameManager.GetCurrentPlayer()).ToString());
				}

				else if(MakeButton(half + half/3, 0, "Arcane Cost: 70"))
				{
					if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), 70))
					{
						_recruit.text = string.Format("{0} Arcane", recruit_text);
					}
					else
						_recruit.text = string.Format("{0} Arcane", recruit_fail);
				}

				else if(MakeButton(half + half/3, 50, "Braver Cost: 50"))
				{
					if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), 50))
					{
						_recruit.text = string.Format("{0} Braver", recruit_text);
					}
					else
						_recruit.text = string.Format("{0} Braver", recruit_fail);
				}

				else if(MakeButton(half + half/3, 100, "Scout Cost: 20"))
				{
					if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), 20))
					{
						_recruit.text = string.Format("{0} Scout", recruit_text);
					}
					else
						_recruit.text = string.Format("{0} Scout", recruit_fail);
				}

				else if(MakeButton(half + half/3, 150, "Sniper Cost: 80"))
				{
					if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), 80))
					{
						_recruit.text = string.Format("{0} Sniper", recruit_text);
					}
					else
						_recruit.text = string.Format("{0} Sniper", recruit_fail);
				}

				else if(MakeButton(half + half/3, 200, "Titan Cost: 150"))
				{
					if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), 150))
					{
						_recruit.text = string.Format("{0} Titan", recruit_text);
					}
					else
						_recruit.text = string.Format("{0} Titan", recruit_fail);
				}

				else if(MakeButton(half + half/3, 250, "Vangaurd Cost: 100"))
				{
					if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), 100))
					{
						_recruit.text = string.Format("{0} Vanguard", recruit_text);
					}
					else
						_recruit.text = string.Format("{0} Vanguard", recruit_fail);
				}
			}
		}
	}

	bool MakeButton(float left, float top, string name)
	{
		return GUI.Button(new Rect(left,top, 150,50), name);
	}
	
}
