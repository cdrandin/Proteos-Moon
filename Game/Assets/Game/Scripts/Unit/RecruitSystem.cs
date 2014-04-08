/*
 * RecruitSystem.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitCost
{
	public int arcane;
	public int braver;
	public int scout;
	public int sniper;
	public int titan;
	public int vangaurd;

	public UnitCost(int arcane, int braver, int scout, int sniper, int titan, int vanguard)
	{
		this.arcane   = arcane;
		this.braver   = braver;
		this.scout    = scout;
		this.sniper   = sniper;
		this.titan    = titan;
		this.vangaurd = vanguard;
	}
}

public class RecruitSystem : MonoBehaviour 
{
	// Prefabs of the objects to summon
	public GameObject arcane;
	public GameObject braver;
	public GameObject scout;
	public GameObject sniper;
	public GameObject titan;
	public GameObject vangaurd;	

	// Circle range in which units can be summoned
	public float summoning_radius;

	// Like pi/12, creates 12 possible spots throughout a circle
	public int steps = 12;

	public UnitCost unit_cost;
	
	private float _interval;
	
	// Use this for initialization
	void Awake()
	{
		_interval = 360.0f/steps;
		++steps; // Just works for now

		if(summoning_radius <= 0)
		{
			Debug.LogWarning("Summoning radius is less than or equal to 0. May perform weird artifacts.");
		}

		if(this.arcane == null)
		{
			Debug.LogWarning("Missing Arcane GameObject reference");
		}

		if(this.braver == null)
		{
			Debug.LogWarning("Missing Braver GameObject reference");
		}

		if(this.scout == null)
		{
			Debug.LogWarning("Missing Scout GameObject reference");
		}

		if(this.sniper == null)
		{
			Debug.LogWarning("Missing Sniper GameObject reference");
		}

		if(this.titan == null)
		{
			Debug.LogWarning("Missing Titan GameObject reference");
		}

		if(this.vangaurd == null)
		{
			Debug.LogWarning("Missing Vangaurd GameObject reference");
		}
	}

	// Bring unit onto the field
	// Horrible way of doing this, just for now
	// Does not use appropriate models, but logic is there
	public GameObject SpawnUnit(UnitType unit_type)
	{
		Transform leader = GM.instance.GetPlayerLeader(GM.instance.CurrentPlayer).transform;
		GameObject unit;

		// For now have it spawn immediately
		if(unit_type == UnitType.Arcane)
		{
			unit = this.arcane;
		}
		else if(unit_type == UnitType.Braver)
		{
			unit = this.braver;
		}
		else if(unit_type == UnitType.Scout)
		{
			unit = this.scout;
		}
		else if(unit_type == UnitType.Sniper)
		{
			unit = this.sniper;
		}
		else if(unit_type == UnitType.Titan)
		{
			unit = this.titan;
		}
		else if(unit_type == UnitType.Vangaurd)
		{
			unit = this.vangaurd;
		}
		else
		{
			Debug.LogError("Spawn unit went to default switch. ERROR");
			unit = null;
		}

		if(unit != null)
		{
			// Spawn behind leader
			GameObject obj = GameObject.Instantiate(unit, leader.position + summoning_radius *(-1 * leader.forward), leader.rotation) as GameObject;

			obj.name = unit.name;
			//obj.tag  = "Unit";
			//obj.layer = LayerMask.NameToLayer(obj.tag);
			return obj;
		}
		else
		{
			return null;
		}

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
