using UnityEngine;
using System.Collections;

[System.Serializable]
public class Vital
{
	private float _current_val;
	private float _max_val;

	public Vital()
	{
		this._current_val = 0;
		this._max_val     = 0.0f;
	}

	public Vital(float max)
	{
		this._current_val = 0.0f;
		this._max_val     = max;
	}

	public Vital(Vital v)
	{
		this._current_val = v._current_val;
		this._max_val     = v._max_val;
	}

	public float Value
	{
		get 
		{
			// Lowest is 0
			if(this._current_val < 0)
				return 0;
			// Most is _max_hp
			else if(this._current_val > this._max_val)
				return this._max_val;
			else
				return this._current_val;
		}
		
		set { this._current_val = value; }
	}

	public float Max
	{
		get { return this._max_val; }
		set { this._max_val = value; }
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