using UnityEngine;
using System.Collections;

[System.Serializable]
public class Scout: Entity
{
	public Scout(int hp, float exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range)
	{
	}
}

[RequireComponent (typeof(UnitStatus))]
public class ScoutClass : MonoBehaviour {

	public Scout scout;
	public UnitType unit_type;

	// Use this for initialization
	void Start () {
		unit_type = UnitType.Scout;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
