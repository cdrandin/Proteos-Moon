using UnityEngine;
using System.Collections;

[System.Serializable]
public class MovementStat 
{
	// How fast unit can move
	[Range(0, 10)]
	public float speed;

	// How much exhaust moving cost per frame
	public float movement_cost_per_frame;

	// How much the unit has traveled so far
	public float current_distance;
	 
	// How far the unit should be able to travel
	// Close to ~ in meters perse.
	public float max_travel_distance;
	
	// Unit allowed to jump
	public bool can_jump;
	
	// Amount of air-jumps allowed, 0 - only capable of jumping once, 1 - "double" jump (init jump then mid-air jump)
	[Range(0, 2)]
	public int air_jumps;
	
	// How high the unit can jump
	public float jump_height;
}
