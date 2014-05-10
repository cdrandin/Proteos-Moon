/*
 * UnitStatus.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

public enum Leader
{
	NOT_AVAILABLE,
	Altier_Seita,
	Captain_Mena
}

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
public struct Status
{
public bool Clean,  // Has not moved, all actions avaliable
			Move,   // Unit is moving
			Action, // Unit has performed an action
			Gather, // Unit is gathering a resource
			Rest,   // Unit has exhuasted its exhuast bar or cannot perform any more actions
			Dead ;  // Unit is dead (Not sure if needed)
}

[System.Serializable]
public class UnitStatus 
{
	[SerializeField]
	private Status _status;

	[SerializeField]
	private Leader _leader;

	[SerializeField]
	private UnitType _unit_type;

	// Use this for initialization
	void Start () 
	{
		Clean();
	}

	public Status status
	{
		get { return _status; }
		set { _status = value; }
	}

	public Leader leader
	{
		get { return _leader; }
	}

	public UnitType unit_type
	{
		get { return _unit_type ; }
		set { _unit_type = value; }
	}

	public void Clean()
	{
		this._status.Clean  = true;
		this._status.Move   = false;
		this._status.Action = false;
		this._status.Gather = false;
		this._status.Rest   = false;
		this._status.Dead   = false;
	}
	
	public void Gather()
	{
		this._status.Gather = true;
	}

	public void Move()
	{
		this._status.Move = true;
	}

	public void Action()
	{
		this._status.Action = true;
	}

	public void Rest()
	{
		this._status.Rest = true;
	}
	
	public void Dead()
	{
		this._status.Clean  = false;
		this._status.Move   = false;
		this._status.Action = false;
		this._status.Gather = false;
		this._status.Rest   = true;
		this._status.Dead   = true;
	}
}