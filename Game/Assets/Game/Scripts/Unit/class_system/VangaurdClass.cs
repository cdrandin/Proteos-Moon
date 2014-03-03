using UnityEngine;
using System.Collections;

[System.Serializable]
public class Vangaurd: Entity
{
	public Vangaurd(int hp, int exhaust, int damage, float distance, float attack_range):
		base(hp, exhaust, damage, distance, attack_range, UnitType.Vangaurd)
	{
	}
}

public class VangaurdClass : MonoBehaviour {

	public Vangaurd vangaurd;

	// Use this for initialization
	void Start () {
		vangaurd.unit_type = UnitType.Vangaurd;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
