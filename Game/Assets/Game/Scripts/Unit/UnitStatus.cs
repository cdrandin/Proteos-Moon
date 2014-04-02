using UnityEngine;
using System.Collections;

public enum UnitType
{
	Arcane = 0,
	Braver,
	Leader,
	Scout, 
	Sniper,
	Titan,
	Vangaurd
}

public enum Status
{
	Clean = 0, // Has not moved, all actions avaliable
	Clicked,   // Mouse has the unit on focus
	Movement,  // Unit is moving
	Combat,    // Unit is in combat
	Ability,   // Unit has used ability last
	Resting,   // Unit has exhuasted its exhuast bar or cannot perform any more actions
	Dead       // Unit is dead (Not sure if needed)
}

[RequireComponent (typeof(UnitSelected))]
public class UnitStatus : MonoBehaviour 
{
	public Status status;
	public UnitType unit_type;

	// Use this for initialization
	void Start () 
	{
		status = Status.Clean;
	}
}
