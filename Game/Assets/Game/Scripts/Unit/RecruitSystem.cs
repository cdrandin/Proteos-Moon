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
	public GameObject summoning_particle;

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

	private bool _ready_to_summon; // Use when to exactly summon a unit, used for placing a unit to spawn at location
	private GameObject _obj_to_summon;
	private GameObject _particle;
	private float _timer;

	// Use this for initialization
	void Awake()
	{
		_interval        = 360.0f/steps;
		++steps; // Just works for now
		_ready_to_summon = false;
		_obj_to_summon   = null;
		_particle        = null;
		_timer           = 0.0f;

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

	void Update ()
	{
		// Read to spawn unit. Display circle of where they can summon.
		if(GM.instance.IsOn && _ready_to_summon)
		{
			if (Input.GetMouseButtonDown(0)) 
			{
				RaycastHit hit;
				Ray ray = GM.instance.CurrentFocusCamera.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit))
				{
					// Difference between leader and where user clicks
					Vector3 dif = GM.instance.GetPlayerLeader(GM.instance.CurrentPlayer).transform.position - hit.point;

					if(dif.sqrMagnitude < summoning_radius*summoning_radius)
					{
						// Make unit visble now
						_obj_to_summon.SetActive(true);
						_obj_to_summon.transform.position = hit.point;
						_ready_to_summon = false;

						// Create summon particle
						_particle = PoolingSystem.instance.PS_Instantiate(summoning_particle, hit.point, Quaternion.identity);
						_timer = 0;
					}
				}
			}
		}
		// Return particle to pool when done
		if(_particle != null)
		{
			_timer += Time.deltaTime;
			
			// Update health bar info based on the refresh rate
			if(_timer >= 1.5f)
			{
				PoolingSystem.instance.PS_Destroy(_particle);
				_particle = null;
				_timer = 0;
			}
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
			GameObject obj = PoolingSystem.instance.PS_Instantiate(unit, leader.position + summoning_radius *(-1 * leader.forward), leader.rotation);
			_ready_to_summon = true;
			_obj_to_summon = obj;
			obj.SetActive(false);
			obj.name = unit.name;

			return obj;
		}
		else
		{
			return null;
		}
	}

	void Reset ()
	{
		unit_cost = new UnitCost(100, 80, 50, 120, 200, 300);
	}

	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(GameObject.Find(string.Format("_leader_spawn{0}", PhotonNetwork.player.ID)).transform.position, summoning_radius);
	}
}
