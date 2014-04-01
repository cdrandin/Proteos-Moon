using UnityEngine;
using System.Collections;

public class Vital
{
	private float _max_val;
	private float _current_val;

	public Vital()
	{
		_current_val = 0;
		_max_val     = 100.0f;
	}

	public Vital(float max)
	{
		_current_val = _max_val = max;
	}

	public float Value
	{
		get 
		{
			// Lowest is 0
			if(_current_val < 0)
				return 0;
			// Most is _max_hp
			else if(_current_val > _max_val)
				return _max_val;
			else
				return _current_val;
		}
		
		set { _current_val = value; }
	}

	public enum VitalName
	{
		Hp = 0,
		Exhaust
	}
}

/*
public class Vital
{
	private byte _max_hp;
	private byte _hp;

	private float _exhaust_max;
	private float _exhaust;

	public Vital(byte max_hp, float max_exhaust)
	{
		_hp      = _max_hp = max_hp;
		_exhaust = _exhaust_max = max_exhaust;
	}

	// Setters and Getters
	public byte Hp
	{
		get 
		{
			// Lowest hp is 0
			if(_hp < 0)
				return 0;
			// Most hp is _max_hp
			else if(_hp > _max_hp)
				return _max_hp;
			else
				return _hp;
		}

		set { _hp = value;}
	}

	public float Exhaust
	{
		get { return Mathf.Clamp(_exhaust, 0.0f, _exhaust_max); }
		set { _exhaust = value; }
	}
}
*/