using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sniper: Entity
{
	public Sniper(int hp, float exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range)
	{
	}
}

[RequireComponent (typeof(UnitStatus))]
public class SniperClass : MonoBehaviour {

	public Sniper sniper;
	public UnitType unit_type;

	// Use this for initialization
	void Start () {
		unit_type = UnitType.Sniper;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
