using UnityEngine;
using System.Collections;

[System.Serializable]
public class Vangaurd: Entity
{
	public Vangaurd(int hp, float exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range)
	{
	}
}

[RequireComponent (typeof(UnitStatus))]
public class VangaurdClass : MonoBehaviour {

	public Vangaurd vangaurd;
	public UnitType unit_type;
	
	// Use this for initialization
	void Start () {
		unit_type = UnitType.Vangaurd;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
