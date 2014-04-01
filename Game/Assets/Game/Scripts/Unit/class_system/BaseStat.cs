using System.Collections;

[System.Serializable]
public class ByteStats
{
	public byte current;
	public byte max;

	public ByteStats(byte current, byte max)
	{
		this.current = current;
		this.max     = max;
	}
}

[System.Serializable]
public class BaseStat 
{
	// Some basic stats for stuff
	public ByteStats _stamina;
	public ByteStats _strength;
	public ByteStats _intellect;
	public ByteStats _recovery;
	public ByteStats _physical_def;
	public ByteStats _magical_def;
	
	public BaseStat(byte stamina, byte strength, byte intellect, byte recovery, byte p_def, byte m_def, byte max=byte.MaxValue)
	{
		this._stamina      = new ByteStats(stamina, max);
		this._strength     = new ByteStats(strength, max);
		this._intellect    = new ByteStats(intellect, max);
		this._recovery     = new ByteStats(recovery, max);
		this._physical_def = new ByteStats(p_def, max);
		this._magical_def  = new ByteStats(m_def, max);
	}
	
	// Getters and Setters
	public ByteStats Stamina
	{
		get	
		{
			if(_stamina.current < 0)
				_stamina.current = 0;
			else if (_stamina.current > _stamina.max)
				_stamina.current = _stamina.max;
			return _stamina;
		}

		set { _stamina = value;}
	}
	
	public ByteStats Strength
	{
		get	
		{
			if(_strength.current < 0)
				_strength.current = 0;
			else if (_strength.current > _strength.max)
				_strength.current = _strength.max;
			return _strength;
		}

		set { _strength = value;}
	}
	
	public ByteStats Intellect
	{
		get	
		{
			if(_intellect.current < 0)
				_intellect.current = 0;
			else if (_intellect.current > _intellect.max)
				_intellect.current = _intellect.max;
			return _intellect;
		}

		set { _intellect = value;}
	}
	
	public ByteStats Recovery
	{
		get	
		{
			if(_recovery.current < 0)
				_recovery.current = 0;
			else if (_recovery.current > _recovery.max)
				_recovery.current = _recovery.max;
			return _recovery;
		}

		set { _recovery = value;}
	}
	
	public ByteStats Physical_def
	{
		get	
		{
			if(_physical_def.current < 0)
				_physical_def.current = 0;
			else if (_physical_def.current > _physical_def.max)
				_physical_def.current = _physical_def.max;
			return _physical_def;
		}

		set { _physical_def = value;}
	}
	
	public ByteStats Magical_def
	{
		get	
		{
			if(_magical_def.current < 0)
				_magical_def.current = 0;
			else if (_strength.current > _magical_def.max)
				_magical_def.current = _magical_def.max;
			return _magical_def;
		}

		set { _magical_def = value;}
	}
}

/*
[System.Serializable]
public class BaseStat
{
	private ByteStats stats;

	public BaseStat()
	{
		this.stats.current = 0;
		this.stats.max     = 0;
	}

	public BaseStat(byte max)
	{
		this.stats.current = 0;
		this.stats.max     = max;
	}

	public BaseStat(BaseStat bs)
	{
		this.stats.current = bs.stats.current;
		this.stats.max     = bs.stats.max;
	}

	public byte Value
	{
		get
		{
			if(this.stats.current < 0)
				return 0;
			else
				return this.stats.current;
		}

		set { this.stats.current = value; }
	}

	public byte Max
	{
		get { return this.stats.max; }
		set { this.stats.max = value; }
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
*/