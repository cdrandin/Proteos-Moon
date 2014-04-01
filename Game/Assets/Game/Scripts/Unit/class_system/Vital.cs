using System.Collections;

[System.Serializable]
public class FloatStats
{
	public float current;
	public float max;
}

[System.Serializable]
public class Vital
{
	// Vitals each unit will have
	public FloatStats _health_point;
	public FloatStats _exhaust;
	
	public Vital(float max_hp, float max_exhaust)
	{
		_health_point.current = max_hp;
		_exhaust.current      = max_exhaust;
	}

	public Vital(FloatStats hp, FloatStats exhaust)
	{
		this._health_point = hp;
		this._exhaust      = exhaust;
	}

	// Setters and Getters
	public FloatStats HP
	{
		get 
		{
			// Lowest hp is 0
			if(_health_point.current < 0)
				_health_point.current = 0;
			// Most hp is _max_hp
			else if(_health_point.current > _health_point.max)
				_health_point.current = _health_point.max;
			return _health_point;
		}
		
		set { _health_point = value;}
	}
	
	public FloatStats Exhaust
	{
		get
		{
			// Lowest exhaust is 0
			if(_exhaust.current < 0)
				_exhaust.current = 0;
			// Most exhaust is _max_exhaust
			else if(_exhaust.current > _exhaust.max)
				_exhaust.current = _exhaust.max;
			return _exhaust;
		}

		set { _exhaust = value; }
	}
}

/*
[System.Serializable]
public class Vital
{
	private FloatStats stats;

	public Vital()
	{
		this.stats.current = 0.0f;
		this.stats.max     = 0.0f;
	}

	public Vital(float max)
	{
		this.stats.current = 0.0f;
		this.stats.max     = max;
	}

	public Vital(Vital v)
	{
		this.stats.current = v.stats.current;
		this.stats.max     = v.stats.max;
	}

	public float Value
	{
		get 
		{
			// Lowest is 0
			if(this.stats.current < 0)
				return 0;
			// Most is _max_hp
			else if(this.stats.current > this.stats.max)
				return this.stats.max;
			else
				return this.stats.current;
		}
		
		set { this.stats.current = value; }
	}

	public float Max
	{
		get { return this.stats.max; }
		set { this.stats.max = value; }
	}

	public enum VitalName
	{
		Hp = 0,
		Exhaust
	}
}
*/