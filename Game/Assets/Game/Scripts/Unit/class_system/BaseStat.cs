using UnityEngine;
using System.Collections;

/*
// Not sure if this is needed.
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
*/

public class BaseStat
{
	private byte _current_val;

	public BaseStat()
	{
		_current_val = 0;
	}

	public BaseStat(byte val)
	{
		_current_val = val;
	}

	public byte Value
	{
		get
		{
			if(_current_val < 0)
				return 0;
			else
				return _current_val;
		}

		set { _current_val = value; }
	}

	public enum StatName
	{
		Stamina = 0,
		Strength,
		Intellect, 
		Recovery,
		Physical_Defense,
		Magical_Defense
	}
}
/*
public class BaseStat 
{
	private byte _stamina;
	private byte _strength;
	private byte _intellect;
	private byte _recovery;
	private byte _physical_def;
	private byte _magical_def;

	public BaseStat(byte stamina, byte strength, byte intellect, byte recovery, byte p_def, byte m_def)
	{
		this.Stamina      = stamina;
		this.Strength     = strength;
		this.Intellect    = intellect;
		this.Recovery     = recovery;
		this.Physical_def = p_def;
		this.Magical_def  = m_def;
	}

	// Getters and Setters
	public byte Stamina
	{
		get	{ return _stamina;}
		set { _stamina = value;}
	}

	public byte Strength
	{
		get	{ return _strength;}
		set { _strength = value;}
	}

	public byte Intellect
	{
		get	{ return _intellect;}
		set { _intellect = value;}
	}

	public byte Recovery
	{
		get	{ return _recovery;}
		set { _recovery = value;}
	}

	public byte Physical_def
	{
		get	{ return _physical_def;}
		set { _physical_def = value;}
	}

	public byte Magical_def
	{
		get	{ return _magical_def;}
		set { _magical_def = value;}
	}
}
*/