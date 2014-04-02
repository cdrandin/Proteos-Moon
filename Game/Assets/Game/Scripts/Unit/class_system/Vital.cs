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
	[SerializeField] 
	private FloatStats _health_point;

	[SerializeField] 
	private FloatStats _exhaust;

	/*
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
	*/

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