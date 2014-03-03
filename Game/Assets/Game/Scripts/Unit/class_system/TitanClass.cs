using UnityEngine;
using System.Collections;

[System.Serializable]
public class Titan: Entity
{
	public Titan(int hp, float exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Titan)
	{
	}
}

public class TitanClass : MonoBehaviour {

	public Titan titan;

	// Use this for initialization
	void Start () {
		titan.unit_type = UnitType.Titan;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
