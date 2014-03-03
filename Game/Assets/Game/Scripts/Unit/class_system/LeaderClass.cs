using UnityEngine;
using System.Collections;

[System.Serializable]
public class Leader: Entity
{
	public Leader(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Leader)
	{
	}
}

public class LeaderClass : MonoBehaviour {

	public Leader leader;

	// Use this for initialization
	void Start () {
		leader.unit_type = UnitType.Leader;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
