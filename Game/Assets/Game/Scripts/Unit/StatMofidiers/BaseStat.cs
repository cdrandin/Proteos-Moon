﻿/*
 * BaseStat.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
public class IntStats
{
	public int current;
	public int max;

	public IntStats(int current, int max)
	{
		this.current = current;
		this.max     = max;
	}
}

public enum BaseStatID:byte
{
	STAMINA,
	STRENGTH,
	AGILITY,
	INTELLECT,
	RECOVERY,
	PHYSICAL_DEFENSE,
	MAGICAL_DEFENSE
}

[System.Serializable]
public class BaseStat 
{
	// Some basic stats for stuff
	[SerializeField] 
	private IntStats _stamina;

	[SerializeField] 
	private IntStats _strength;

	[SerializeField]
	private IntStats _agility;

	[SerializeField] 
	private IntStats _intellect;

	[SerializeField] 
	private IntStats _recovery;

	[SerializeField] 
	private IntStats _physical_def;

	[SerializeField] 
	private IntStats _magical_def;

	public BaseStat(byte stamina, byte strength, byte intellect, byte recovery, byte p_def, byte m_def, byte max=byte.MaxValue)
	{
		this._stamina      = new IntStats(stamina, max);
		this._strength     = new IntStats(strength, max);
		this._intellect    = new IntStats(intellect, max);
		this._recovery     = new IntStats(recovery, max);
		this._physical_def = new IntStats(p_def, max);
		this._magical_def  = new IntStats(m_def, max);
	}

	// Getters and Setters
	public IntStats Stamina
	{
		get	
		{
			if(_stamina.current < 0)
			{
				_stamina.current = 0;
			}
			else if (_stamina.current > _stamina.max)
			{
				_stamina.current = _stamina.max;
			}

			return _stamina;
		}

		set { _stamina = value;}
	}
	

	public IntStats Strength
	{
		get	
		{
			if(_strength.current < 0)
			{
				_strength.current = 0;
			}
			else if (_strength.current > _strength.max)
			{
				_strength.current = _strength.max;
			}

			return _strength;
		}
		
		set { _strength = value;}
	}

	public IntStats Agility
	{
		get	
		{
			if(_agility.current < 0)
			{
				_agility.current = 0;
			}
			else if (_agility.current > _agility.max)
			{
				_agility.current = _agility.max;
			}

			return _agility;
		}
		
		set { _strength = value;}
	}
	
	public IntStats Intellect
	{
		get	
		{
			if(_intellect.current < 0)
			{
				_intellect.current = 0;
			}
			else if (_intellect.current > _intellect.max)
			{
				_intellect.current = _intellect.max;
			}

			return _intellect;
		}

		set { _intellect = value;}
	}
	
	public IntStats Recovery
	{
		get	
		{
			if(_recovery.current < 0)
			{
				_recovery.current = 0;
			}
			else if (_recovery.current > _recovery.max)
			{
				_recovery.current = _recovery.max;
			}

			return _recovery;
		}

		set { _recovery = value;}
	}
	
	public IntStats Physical_def
	{
		get	
		{
			if(_physical_def.current < 0)
			{
				_physical_def.current = 0;
			}
			else if (_physical_def.current > _physical_def.max)
			{
				_physical_def.current = _physical_def.max;
			}

			return _physical_def;
		}

		set { _physical_def = value;}
	}
	
	public IntStats Magical_def
	{
		get	
		{
			if(_magical_def.current < 0)
			{
				_magical_def.current = 0;
			}
			else if (_strength.current > _magical_def.max)
			{
				_magical_def.current = _magical_def.max;
			}

			return _magical_def;
		}

		set { _magical_def = value;}
	}
}