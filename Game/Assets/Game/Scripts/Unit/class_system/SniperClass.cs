using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sniper: Entity
{
	public Sniper(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Sniper)
	{
	}
}

public class SniperClass : MonoBehaviour {

	public Sniper sniper;

	// Use this for initialization
	void Start () {
		sniper.unit_type = UnitType.Sniper;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
