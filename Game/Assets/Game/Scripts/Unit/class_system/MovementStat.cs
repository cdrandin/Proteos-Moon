/*
 * MovementStat.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
public class MovementStat 
{
	// How fast unit can move
	[Range(0, 10)]
	[SerializeField]
	private float _speed;

	// How much exhaust moving cost per frame
	//[SerializeField]
	private float _movement_cost_per_frame;

	// How much the unit has traveled so far
	//[SerializeField]
	//private float _current_distance;
	 
	// How far the unit should be able to travel
	// Close to ~ in meters perse.
	[SerializeField]
	private float _max_travel_distance;
	
	// Unit allowed to jump
	[SerializeField]
	private bool _can_jump;
	
	// Amount of air-jumps allowed, 0 - only capable of jumping once, 1 - "double" jump (init jump then mid-air jump)
	[Range(0, 2)]
	[SerializeField]
	private int _air_jumps;
	
	// How high the unit can jump
	[SerializeField]
	private float _jump_height;

	[SerializeField]
	private float _fall_speed;

	// Keep track of unit controller for the current moved amount
	private UnitController _uc = null;

	public float speed
	{
		get { return _speed; }
	}

	public float movement_cost
	{
		get { return _movement_cost_per_frame; }
	}

	public float current_distance
	{
		get
		{
			if(_uc == null)
			{
				_uc = GameObject.Find("UnitController").GetComponent<UnitController>();
			}


			return _uc.travel_distance;
		}
	}

	public float max_distance
	{
		get { return _max_travel_distance; }
	}

	public bool can_jump
	{
		get { return _can_jump; }
	}

	public int air_jumps
	{
		get { return _air_jumps; }
	}

	public float jump_height 
	{
		get { return _jump_height; }
	}

	public float fall_speed
	{
		get { return _fall_speed; }
	}
}
