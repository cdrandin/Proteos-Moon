using UnityEngine;
using System.Collections;

public class UnitHierarchy: MonoBehaviour
{
	public Arcane player1;
	public Titan player2;

	void Start()
	{
		player1 = new Arcane(100,200,50,60,300);
		player2 = new Titan(300,50,200,10,100);



		//Combat(player1, player2);
	}

	void Update()
	{
	}

	void Combat(Entity combatant1, Entity combatant2)
	{
		Debug.Log(string.Format("Combat between {0}({1}hp) and {2}({3}hp)\n", 
		                        combatant1.unit_type, combatant1.hp, combatant2.unit_type, combatant2.hp));

		combatant1.hp = Mathf.Clamp(combatant1.hp - combatant2.damage, 0, 9999);
		combatant2.hp = Mathf.Clamp(combatant2.hp - combatant1.damage, 0, 9999);

		Debug.Log(string.Format("Combat aftermath:\n {0} at {1} hp!\t {2} at {3} hp!\n", 
		                        combatant1.unit_type, combatant1.hp, combatant2.unit_type, combatant2.hp));
	}
}

public enum UnitType
{
	Arcane = 0,
	Bravers,
	Leader,
	Scout, 
	Sniper,
	Titan,
	Vangaurd
}

[System.Serializable]
public class Entity
{
	public int _hp;
	public int _mp;
	public int _damage;
	public float _distance;
	public float _attack_range;
	public UnitType _unit_type;

	public Entity(int hp, int mp, int damage, float distance, float attack_range, UnitType type)
	{
		_hp           = hp;
		_mp           = mp;
		_damage       = damage; 
		_distance     = distance; 
		_attack_range = attack_range;
		_unit_type    = type;
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
	
	public int mp 
	{ 
		get
		{
			return _mp;
		}
		set
		{
			_mp = value;
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
			return _distance;
		}
		set
		{
			_distance = value;
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

[System.Serializable]
public class Arcane: Entity
{
	public Arcane(int hp, int mp, int damage, float distance, float attack_range):
		base(hp, mp, damage, distance, attack_range, UnitType.Arcane)
	{
	}
}

[System.Serializable]
public class Braver: Entity
{
	public Braver(int hp, int mp, int damage, float distance, float attack_range):
		base(hp, mp, damage, distance, attack_range, UnitType.Bravers)
	{
	}
}

[System.Serializable]
public class Scout: Entity
{
	public Scout(int hp, int mp, int damage, float distance, float attack_range):
		base(hp, mp, damage, distance, attack_range, UnitType.Scout)
	{
	}
}

[System.Serializable]
public class Sniper: Entity
{
	public Sniper(int hp, int mp, int damage, float distance, float attack_range):
		base(hp, mp, damage, distance, attack_range, UnitType.Sniper)
	{
	}
}

[System.Serializable]
public class Titan: Entity
{
	public Titan(int hp, int mp, int damage, float distance, float attack_range):
		base(hp, mp, damage, distance, attack_range, UnitType.Titan)
	{
	}
}

[System.Serializable]
public class Vangaurd: Entity
{
	public Vangaurd(int hp, int mp, int damage, float distance, float attack_range):
		base(hp, mp, damage, distance, attack_range, UnitType.Vangaurd)
	{
	}
}