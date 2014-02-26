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
	// Circle range in which units can be summoned
	public float summoning_radius;

	// Like pi/12, creates 12 possible spots throughout a circle
	public int steps = 12;

	public UnitCost unit_cost;
	
	private float _interval;
	
	// Use this for initialization
	void Start ()
	{
		_interval = 360.0f/steps;
		++steps; // Just works for now

		if(summoning_radius <= 0)
			Debug.LogWarning("Summoning radius is less than or equal to 0. May perform weird artifacts.");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	// Bring unit onto the field
	// Horrible way of doing this, just for now
	// Does not use appropriate models, but logic is there
	public GameObject SpawnUnit(UnitType unit_type)
	{
		Vector3 position = GameManager.GetPlayerLeader(GameManager.GetCurrentPlayer()).transform.position;
		string name = "ERROR";

		// For now have it spawn immediately
		if(unit_type == UnitType.Arcane)
			name = "Arcane";
		
		else if(unit_type == UnitType.Braver)
			name = "Braver";
		
		else if(unit_type == UnitType.Scout)
			name = "Scout";
		
		else if(unit_type == UnitType.Sniper)
			name = "Sniper";
		
		else if(unit_type == UnitType.Titan)
			name = "Titan";
		else if(unit_type == UnitType.Vangaurd)
			name = "Vangaurd";
		else
			Debug.LogError("Spawn unit went to default switch. ERROR");

		// Spawn behind leader
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		obj.transform.position = position + 2*Vector3.back;
		obj.transform.rotation = Quaternion.identity;
		obj.name = name;
		obj.tag  = "Unit";
		obj.layer = LayerMask.NameToLayer(obj.tag);

		return obj;

		/*// Summon in a ring of units around Leader
		foreach(Vector3 location in SpawnLocations(position))
		{
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			obj.transform.position = location;
			obj.transform.rotation = Quaternion.identity;
			obj.name = name;
		}
		*/
	}

	// Possible locations for a unit to be spawned. Later implement filter that checks if tht spot is filled, then it 
	//   is not a possile spawn location.
	Vector3[] SpawnLocations(Vector3 origin)
	{
		Vector3[] locations = new Vector3[steps];

		// Traverse through each step
		for(int i=0;i<steps;++i)
		{
			// Equation of circle
			// All that is carred for ix X-Z position
			locations[i] = new Vector3(summoning_radius * Mathf.Cos(i*_interval) + origin.x, 
			                             origin.y, 
			                             summoning_radius * Mathf.Sin(i*_interval) + origin.z);
		}
		return locations;
	}

	void Reset ()
	{
		unit_cost = new UnitCost(100, 80, 50, 120, 200, 300);
	}
}
