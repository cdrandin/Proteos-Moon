using UnityEngine;
using System.Collections;

[System.Serializable]
public class Arcane: Entity
{
	public Arcane(int hp, float exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range)
	{
	}
}

[RequireComponent (typeof(UnitStatus))]
public class ArcaneClass : MonoBehaviour {

	public Arcane arcane;
	public UnitType unit_type;

	// Use this for initialization
	void Start () {
		unit_type = UnitType.Arcane;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
