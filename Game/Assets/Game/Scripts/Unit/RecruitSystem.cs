using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitCost
{
	public int Arcane;
	public int Braver;
	public int Scout;
	public int Sniper;
	public int Titan;
	public int Vangaurd;

	public UnitCost(int arcane, int braver, int scout, int sniper, int titan, int vanguard)
	{
		Arcane   = arcane;
		Braver   = braver;
		Scout    = scout;
		Sniper   = sniper;
		Titan    = titan;
		Vangaurd = vanguard;
	}
}

public class RecruitSystem : MonoBehaviour 
{
	public UnitCost unit_cost;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void Reset ()
	{
		unit_cost = new UnitCost(100, 80, 50, 120, 200, 300);
	}
}
