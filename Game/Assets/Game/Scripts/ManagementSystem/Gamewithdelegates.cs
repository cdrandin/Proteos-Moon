using UnityEngine;
using System.Collections;

public class Gamewithdelegates : MonoBehaviour
{
	public int num_of_players;
	public int resource_limit;
	
	private bool change;

	/* 
	 * Variables used for testing GameManager
	 */
	private bool _manager_init;
	private GUIText _resource;
	private GUIText _recruit;

	private delegate void GUIMethod();
	private GUIMethod gui_method;
	private bool recruit_gui_on;

	void Awake() 
	{
		_resource = GameObject.Find("Resource_GUIText").GetComponent<GUIText>();
		_recruit  = GameObject.Find("Recruit_GUIText").GetComponent<GUIText>();
	}

	// Use this for initialization
	void Start () 
	{
		this.gui_method = GUI_init;
		recruit_gui_on = true;
		_manager_init = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnGUI()
	{
		if(this.gui_method != null)
			this.gui_method();
	}
	
	void GUI_init()
	{
		if(MakeButton(0,0,"Start GameManager") /*&& !_manager_init*/)
		{
			this.gui_method += GUI_menu;

			GameManager.Init(num_of_players, RandomFirstPlayer(num_of_players), resource_limit);

			//_manager_init = true;

			_resource.transform.position = new Vector3(0.05f, 0.05f, 0.0f);
			_resource.fontSize = 16;
			_resource.text = string.Format("Current player: {0} at {1} Resources", 
			                               GameManager.GetCurrentPlayer(), GameManager.GetResourceFrom(GameManager.GetCurrentPlayer()).ToString());
		}
		
		if(MakeButton(0, 50, "End GameManager"))
		{
			this.gui_method -= GUI_menu;

			GameManager.ResetGameState();
			//_manager_init = false;
		}
	}

	void GUI_menu()
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
		else if(MakeButton(half, 200, "Recruit Menu"))
		{
			print (recruit_gui_on);

			if(recruit_gui_on)
				this.gui_method += GUI_recruit;
			else
				this.gui_method -= GUI_recruit;

			recruit_gui_on = !recruit_gui_on;
		}
	}

	void GUI_recruit()
	{
		float half = Screen.width/2;
		string recruit_text = "Recently purchased";
		string recruit_fail = "Could not purchase";

		if(MakeButton(half + half/3, 0, "Arcane Cost: 70"))
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

	bool MakeButton(float left, float top, string name)
	{
		return GUI.Button(new Rect(left,top, 150,50), name);
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
}
