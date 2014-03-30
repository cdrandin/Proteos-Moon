using UnityEngine;
using System.Collections;

[System.Serializable]
public class Braver: Entity
{
	public Braver(int hp, float exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range)
	{
	}
}

[RequireComponent (typeof(UnitStatus))]
public class BraverClass : MonoBehaviour {

	public Braver braver;
	public UnitType unit_type;

	// Use this for initialization
	void Start () {
		unit_type = UnitType.Braver;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
