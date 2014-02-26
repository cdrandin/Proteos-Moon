using UnityEngine;
using System.Collections;

[RequireComponent (typeof(RecruitSystem))]
public class Game : MonoBehaviour
{
	public int num_of_players;
	public int resource_limit;
	
	public bool testing;

	/* 
	 * Variables used for testing GameManager
	 */
	private delegate void GUIMethod();
	private GUIMethod gui_method;

	private bool recruit_gui_on;
	private GUIText _game_manager_gui;

	private UnitCost _unit_cost;

	private float waitingTime;
	private float timer;


	void Awake() 
	{
		_game_manager_gui = GameObject.Find("GameManagerStatus").GetComponent<GUIText>();
		_game_manager_gui.text = "";

		_unit_cost = GetComponent<RecruitSystem>().unit_cost;
	}

	// Use this for initialization
	void Start () 
	{
		if(testing)
		{
			this.gui_method = GUI_init;
			recruit_gui_on = true;
			waitingTime = 5.0f;
			timer = 0.0f;
		}

		_game_manager_gui.transform.position = new Vector3(0.18f, 0.95f, 0.0f);
		_game_manager_gui.fontSize = 16;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Reset timer for display the resource text
		if(Input.GetMouseButtonDown(0))
			timer = 0;

		//Only run when GameManager is active
		if(GameManager.IsOn())
		{
			timer += Time.deltaTime;
			if(timer > waitingTime){
				//Action
				_game_manager_gui.text = string.Format("Current player: {0} at {1} Resources", 
				                                       GameManager.GetCurrentPlayer(), (GameManager.GetResourceFrom(GameManager.GetCurrentPlayer())).ToString());
				timer = 0;
			}
		}
	}

	void OnGUI()
	{
		if(this.gui_method != null)
			this.gui_method();
	}
	
	void GUI_init()
	{
		if(MakeButton(0,0,"Start GameManager"))
		{
			this.gui_method += GUI_menu;

			GameManager.Init(num_of_players, RandomFirstPlayer(num_of_players), resource_limit, GetComponent<RecruitSystem>().unit_cost);

			_game_manager_gui.text = "Game Manager enabled";
		}
		
		else if(MakeButton(0, 50, "End GameManager"))
		{
			this.gui_method -= GUI_menu;

			GameManager.ResetGameState();

			_game_manager_gui.text = "Game Manager disabled";
		}
	}

	void GUI_menu()
	{
		float half = Screen.width/2;
		
		if(MakeButton(half, 0, "Next player's turn"))
		{
			GameManager.NextPlayersTurn();

			_game_manager_gui.text = string.Format("Next player's turn\n" + 
			                                       "Current player: {0}\n",
			                                       GameManager.GetCurrentPlayer());
		}
		
		else if(MakeButton(half, 50, "Current round #"))
			_game_manager_gui.text = string.Format("Current round: {0}",
			                                       GameManager.GetCurrentRound());
		
		else if(MakeButton(half, 100, "Timer"))
			_game_manager_gui.text = string.Format("Current time: {0}", GameManager.GetCurrentTime());
		
		else if(MakeButton(half, 150, string.Format("Add 50 resource pts\n to {0}", GameManager.GetCurrentPlayer())))
		{
			GameManager.AddResources(GameManager.GetCurrentPlayer(),50);
			_game_manager_gui.text = string.Format("Current player: {0} at {1} Resources", 
			                               GameManager.GetCurrentPlayer(), (GameManager.GetResourceFrom(GameManager.GetCurrentPlayer())).ToString());
		}

		else if(MakeButton(half, 200, "Recruit Menu"))
		{
			if(recruit_gui_on)
			{
				this.gui_method += GUI_recruit;
				_game_manager_gui.text = "Recruit Menu opened";
			}
			else
			{	
				this.gui_method -= GUI_recruit;
				_game_manager_gui.text = "Recruit Menu closed";
			}

			recruit_gui_on = !recruit_gui_on;
		}
	}

	void GUI_recruit()
	{
		float half = Screen.width/2;
		string recruit_text = "Recently purchased";
		string recruit_fail = "Could not purchase";

		if(MakeButton(half + half/3, 0, string.Format("Arcane Cost: {0}", _unit_cost.Arcane)))
		{
			if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), UnitType.Arcane))
			{
				_game_manager_gui.text = string.Format("{0} Arcane", recruit_text);
			}
			else
				_game_manager_gui.text = string.Format("{0} Arcane", recruit_fail);
		}
		
		else if(MakeButton(half + half/3, 50, string.Format("Braver Cost: {0}", _unit_cost.Braver)))
		{
			if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), UnitType.Braver))
			{
				_game_manager_gui.text = string.Format("{0} Braver", recruit_text);
			}
			else
				_game_manager_gui.text = string.Format("{0} Braver", recruit_fail);
		}
		
		else if(MakeButton(half + half/3, 100, string.Format("Scout Cost: {0}", _unit_cost.Scout)))
		{
			if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), UnitType.Scout))
			{
				_game_manager_gui.text = string.Format("{0} Scout", recruit_text);
			}
			else
				_game_manager_gui.text = string.Format("{0} Scout", recruit_fail);
		}
		
		else if(MakeButton(half + half/3, 150, string.Format("Sniper Cost: {0}", _unit_cost.Sniper)))
		{
			if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), UnitType.Sniper))
			{
				_game_manager_gui.text = string.Format("{0} Sniper", recruit_text);
			}
			else
				_game_manager_gui.text = string.Format("{0} Sniper", recruit_fail);
		}
		
		else if(MakeButton(half + half/3, 200, string.Format("Titan Cost: {0}", _unit_cost.Titan)))
		{
			if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), UnitType.Titan))
			{
				_game_manager_gui.text = string.Format("{0} Titan", recruit_text);
			}
			else
				_game_manager_gui.text = string.Format("{0} Titan", recruit_fail);
		}
		
		else if(MakeButton(half + half/3, 250, string.Format("Vangaurd Cost: {0}", _unit_cost.Vangaurd)))
		{
			if(GameManager.RecruitUnit(GameManager.GetCurrentPlayer(), UnitType.Vangaurd))
			{
				_game_manager_gui.text = string.Format("{0} Vanguard", recruit_text);
			}
			else
				_game_manager_gui.text = string.Format("{0} Vanguard", recruit_fail);
		}
	}

	bool MakeButton(float left, float top, string name)
	{
		return GUI.Button(new Rect(left,top+50, 150,50), name);
	}

	int RandomFirstPlayer(int number_of_players)
	{
		return Random.Range(1,number_of_players+1);
	}
	
	void Reset ()
	{
		num_of_players = 2;
		resource_limit = 500;
		testing = false;
	}
}
