/*
 * UnitStatus.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
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

[System.Serializable]
public enum Status
{
	Clean = 0, // Has not moved, all actions avaliable
	Clicked,   // Mouse has the unit on focus
	Movement,  // Unit is moving
	Combat,    // Unit is in combat
	Ability,   // Unit has used ability last
	Gathering,    // UNit is gathering a resource
	Resting,   // Unit has exhuasted its exhuast bar or cannot perform any more actions
	Dead       // Unit is dead (Not sure if needed)
}

[System.Serializable]
public class UnitStatus 
{
	[SerializeField] 
	private Status _status;

	[SerializeField] 
	private UnitType _unit_type;

	// Use this for initialization
	void Start () 
	{
		_status = Status.Clean;
	}

	public Status status
	{
		get { return _status; }
		set { _status = value; }
	}

	public UnitType unit_type
	{
		get { return _unit_type ; }
		set { _unit_type = value; }
	}
}