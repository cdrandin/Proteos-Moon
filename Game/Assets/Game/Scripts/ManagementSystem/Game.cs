﻿/*
 * Game.cs
 * 
 * Christopher Randin
 */

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
	private bool init;

	private WorldCameraModified wcm;
	
	void Awake() 
	{
		_game_manager_gui = GameObject.Find("GameManagerStatus").GetComponent<GUIText>();
		_game_manager_gui.text = "";
		wcm = GameObject.Find("WorldCamera").GetComponent<WorldCameraModified>();
		if(wcm == null)
		{
			Debug.LogError("Cannot find WorldCamera");
		}
		_unit_cost = GetComponent<RecruitSystem>().unit_cost;
	}
	
	// Use this for initialization
	void Start () 
	{
		if(testing)
		{
			this.gui_method += GUI_init;
			_game_manager_gui.enabled = true;
			recruit_gui_on = true;
			_game_manager_gui.transform.position = new Vector3(0.18f, 0.95f, 0.0f);
			_game_manager_gui.fontSize = 16;
			
			waitingTime = 5.0f;
			timer = 0.0f;
			
			init = false;
			print ("testing is on");
		}
		else
		{
			print ("testing is off");
			//this.gui_method += GUI_menu;
			GM.instance.Init(num_of_players, RandomFirstPlayer(num_of_players), resource_limit, GetComponent<RecruitSystem>().unit_cost);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GM.instance.IsOn)
		{
			if(Input.GetMouseButtonDown(0) && wcm.MainCamera != null)
			{
				// Reset timer for display the resource text
				timer = 0;
				Ray ray = wcm.MainCamera.camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, 100))
				{
					// Get correct, unit
					string tag = hit.transform.tag;
					
					if(tag == "Unit" || tag == "Leader")
					{
						GameObject obj = hit.transform.gameObject;
						GM.instance.SetUnitControllerActiveOn(ref obj);
					}
					else
					{
						GM.instance.SetUnitControllerActiveOff();
					}
				}
			}
			if(GM.instance.IsThereAWinner())
			{
				_game_manager_gui.text = string.Format("The winner is {0}!", GM.instance.Winner);
			}
			if(testing)
			{
				timer += Time.deltaTime;
				if(timer > waitingTime)
				{
					//Action
					_game_manager_gui.text = string.Format("Current player: {0} at {1}/{2} Resources", 
					                                       GM.instance.CurrentPlayer, 
					                                       GM.instance.GetResourceFrom(GM.instance.CurrentPlayer).ToString(),
					                                       GM.instance.MaxResourceLimit.ToString());
					timer = 0;
				}
			}
		}
	}
	
	void OnGUI()
	{
		if(this.gui_method != null)
		{
			this.gui_method();
		}
	}
	
	void GUI_init()
	{
		if(MakeButton(0,80,"Start GameManager"))
		{
			if(init)
			{
				return;
			}
			
			this.gui_method += GUI_menu;
			init = true;
			_game_manager_gui.text = "Game Manager enabled";
			
			GM.instance.Init(num_of_players, RandomFirstPlayer(num_of_players), resource_limit, GetComponent<RecruitSystem>().unit_cost);
		}
		
		else if(MakeButton(0, 100, "End GameManager"))
		{
			if(!init)
			{
				return;
			}
			
			this.gui_method -= GUI_menu;
			if(!recruit_gui_on)
			{
				this.gui_method -= GUI_recruit;
				recruit_gui_on = !recruit_gui_on;
			}
			init = false;
			_game_manager_gui.text = "Game Manager disabled";
			
			GM.instance.ResetGameState();
		}
	}
	
	void GUI_menu()
	{
		float half = 0; //Screen.width/2;
		
		if(GM.instance.IsOn)
		{
			if(MakeButton(half, 150, "Next player's turn"))
			{
				GM.instance.NextPlayersTurn();
				this.gui_method -= GUI_recruit;
				recruit_gui_on = !recruit_gui_on;
				
				_game_manager_gui.text = string.Format("Next player's turn\n" + 
				                                       "Current player: {0}\n",
				                                       GM.instance.CurrentPlayer);
			}
			
			else if(MakeButton(half, 170, "Current round #"))
			{
				_game_manager_gui.text = string.Format("Current round: {0}",
				                                       GM.instance.CurrentRound);
			}
			
			else if(MakeButton(half, 190, "Timer"))
			{
							_game_manager_gui.text = string.Format("Current time: {0}", GM.instance.CurrentTime);
			}
			
			else if(MakeButton(half, 210, string.Format("Add 50 resource pts")))
			{
				GM.instance.AddResourcesToCurrentPlayer(50);
				_game_manager_gui.text = string.Format("Current player: {0} at {1}/{2} Resources", 
				                                       GM.instance.CurrentPlayer, 
				                                       (GM.instance.GetResourceFrom(GM.instance.CurrentPlayer)).ToString(),
				                                       GM.instance.MaxResourceLimit.ToString());
			}
			
			else if(MakeButton(half, 260, "Recruit Menu"))
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
	}
	
	void GUI_recruit()
	{
		float half = 0;//Screen.width/2;
		string recruit_text = "Recently purchased";
		string recruit_fail = "Could not purchase";
		
		if(MakeButton(half /*+ half/3*/, 280, string.Format("Arcane Cost: {0}", _unit_cost.arcane)))
		{
			if(GM.instance.RecruitUnit(GM.instance.CurrentPlayer, UnitType.Arcane))
			{
				_game_manager_gui.text = string.Format("{0} Arcane", recruit_text);
			}
			else
			{
				_game_manager_gui.text = string.Format("{0} Arcane", recruit_fail);
			}
		}
		
		else if(MakeButton(half /*+ half/3*/, 300, string.Format("Braver Cost: {0}", _unit_cost.braver)))
		{
			if(GM.instance.RecruitUnit(GM.instance.CurrentPlayer, UnitType.Braver))
			{
				_game_manager_gui.text = string.Format("{0} Braver", recruit_text);
			}
			else
			{
				_game_manager_gui.text = string.Format("{0} Braver", recruit_fail);
			}
		}
		
		else if(MakeButton(half /*+ half/3*/, 320, string.Format("Scout Cost: {0}", _unit_cost.scout)))
		{
			if(GM.instance.RecruitUnit(GM.instance.CurrentPlayer, UnitType.Scout))
			{
				_game_manager_gui.text = string.Format("{0} Scout", recruit_text);
			}
			else
			{
				_game_manager_gui.text = string.Format("{0} Scout", recruit_fail);
			}
		}
		
		else if(MakeButton(half /*+ half/3*/, 340, string.Format("Sniper Cost: {0}", _unit_cost.sniper)))
		{
			if(GM.instance.RecruitUnit(GM.instance.CurrentPlayer, UnitType.Sniper))
			{
				_game_manager_gui.text = string.Format("{0} Sniper", recruit_text);
			}
			else
			{
				_game_manager_gui.text = string.Format("{0} Sniper", recruit_fail);
			}
		}
		
		else if(MakeButton(half /*+ half/3*/, 360, string.Format("Titan Cost: {0}", _unit_cost.titan)))
		{
			if(GM.instance.RecruitUnit(GM.instance.CurrentPlayer, UnitType.Titan))
			{
				_game_manager_gui.text = string.Format("{0} Titan", recruit_text);
			}
			else
			{
				_game_manager_gui.text = string.Format("{0} Titan", recruit_fail);
			}
		}
		
		else if(MakeButton(half /*+ half/3*/, 380, string.Format("Vangaurd Cost: {0}", _unit_cost.vangaurd)))
		{
			if(GM.instance.RecruitUnit(GM.instance.CurrentPlayer, UnitType.Vangaurd))
			{
				_game_manager_gui.text = string.Format("{0} Vanguard", recruit_text);
			}
			else
			{
				_game_manager_gui.text = string.Format("{0} Vanguard", recruit_fail);
			}
		}
	}
	
	bool MakeButton(float left, float top, string name)
	{
		return GUI.Button(new Rect(left,top+20, 150,20), name);
	}
	
	int RandomFirstPlayer(int number_of_players)
	{
		return Random.Range(1,number_of_players+1);
	}
	
	public void InitGUIState()
	{
		//this.gui_method += GUI_init;
	}
	
	public void ResetGUIState()
	{
		init = false;
		this.gui_method -= GUI_init;
		this.gui_method -= GUI_menu;
		this.gui_method -= GUI_recruit;
		recruit_gui_on = true;
	}
	
	
	void Reset ()
	{
		num_of_players = 2;
		resource_limit = 500;
		testing = false;
		_game_manager_gui.text = "";
	}
}
