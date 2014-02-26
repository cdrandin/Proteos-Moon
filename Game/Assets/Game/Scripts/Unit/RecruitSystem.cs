using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitCost
{
	public int Arcane;
	public int Braver;
	public int Scout;
	public int Sniper;
	public int Titan;
	public int Vangaurd;

	public UnitCost(int arcane, int braver, int scout, int sniper, int titan, int vanguard)
	{
		Arcane   = arcane;
		Braver   = braver;
		Scout    = scout;
		Sniper   = sniper;
		Titan    = titan;
		Vangaurd = vanguard;
	}
}

public class RecruitSystem : MonoBehaviour 
{
	public UnitCost unit_cost;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	// Bring unit onto the field
	// Horrible way of doing this, just for now
	public void SpawnUnit(UnitType unit_type)
	{
		Vector3 position = GameManager.GetPlayerLeader(GameManager.GetCurrentPlayer()).transform.position;
		string name;

		// For now have it spawn immediately
		switch(unit_type)
		{
		case UnitType.Arcane:
			name = "Arcane";
			break;
		case UnitType.Braver:
			name = "Braver";
			break;
		case UnitType.Scout:
			name = "Scout";
			break;
		case UnitType.Sniper:
			name = "Sniper";
			break;
		case UnitType.Titan:
			name = "Titan";
			break;
		case UnitType.Vangaurd:
			name = "Vangaurd";
			break;
		default:
			name = "BROKEN";
			Debug.LogError("Spawn unit went to default switch. ERROR");
			return;
		}

		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		obj.transform.position = position;
		obj.transform.rotation = Quaternion.identity;
		obj.name = name;
	}

	void Reset ()
	{
		unit_cost = new UnitCost(100, 80, 50, 120, 200, 300);
	}
}
