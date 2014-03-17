using UnityEngine;
using System.Collections;

[System.Serializable]
public class Leader: Entity
{
	public Leader(int hp, float exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range)
	{
	}
}

[RequireComponent (typeof(UnitStatus))]
public class LeaderClass : MonoBehaviour {

	public Leader leader;
	public UnitType unit_type;

	// Use this for initialization
	void Start () {
		unit_type = UnitType.Leader;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
