using UnityEngine;
using System.Collections;

/*
 * Still needs some work. Doesn't feel good enough. May just do Entity-Coexhaustonent System
 */
// Exhuast and distance should be on of the same
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

[System.Serializable]
public class Entity
{
	public int _hp;
	public float _exhaust;
	public int _damage;
	public float _distance_cost;
	public float _attack_range;
	private UnitType _unit_type;

	public Entity(int hp, float exhaust, int damage, float distance, float attack_range, UnitType type)
	{
		_hp            = hp;
		_exhaust       = exhaust;
		_damage        = damage; 
		_distance_cost = distance; 
		_attack_range  = attack_range;
		_unit_type     = type;
	}
	
	public int hp 
	{ 
		get
		{
			return _hp;
		}
		set
		{
			_hp = value;
		}
	}
	
	public float exhaust 
	{ 
		get
		{
			return _exhaust;
		}
		set
		{
			_exhaust = value;
		}
	}
	
	public int damage 
	{ 
		get
		{
			return _damage;
		}
		set
		{
			_damage = value;
		}
	}
	
	public float distance 
	{ 
		get
		{
			return _distance_cost;
		}
		set
		{
			_distance_cost = value;
		}
	}
	
	public float attack_range 
	{ 
		get
		{
			return _attack_range;
		}
		set
		{
			_attack_range = value;
		}
	}

	public UnitType unit_type
	{
		get
		{
			return _unit_type;
		}
		set
		{
			_unit_type = value;
		}
	}
}
/*
[System.Serializable]
public class Leader: Entity
{
	public Leader(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Leader)
	{
	}
}

[System.Serializable]
public class Arcane: Entity
{
	public Arcane(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Arcane)
	{
	}
}

[System.Serializable]
public class Braver: Entity
{
	public Braver(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Braver)
	{
	}
}

[System.Serializable]
public class Scout: Entity
{
	public Scout(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Scout)
	{
	}
}

[System.Serializable]
public class Sniper: Entity
{
	public Sniper(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Sniper)
	{
	}
}

[System.Serializable]
public class Titan: Entity
{
	public Titan(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Titan)
	{
	}
}

[System.Serializable]
public class Vangaurd: Entity
{
	public Vangaurd(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Vangaurd)
	{
	}
}
*/