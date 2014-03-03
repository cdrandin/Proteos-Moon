using UnityEngine;
using System.Collections;

[System.Serializable]
public class Scout: Entity
{
	public Scout(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Scout)
	{
	}
}

public class ScoutClass : MonoBehaviour {

	public Scout scout;

	// Use this for initialization
	void Start () {
		scout.unit_type = UnitType.Scout;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
