/*
 * DistanceProjection.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

/*
 * Usuage:
 * Projection display over a unit, if ABLE to move still.
 * Call 'SetProjectionOn' with 1 arg, GameObject 'target'
 *      This will project a ring over the unit's starting position displaying
 *      how far the unit can move with respect to its distance.
 * Call UpdateProjection to have it follow the focused unit
 * Call SetProjectionOff to turn it off and remove focus
 */

public class DistanceProjection : MonoBehaviour
{
	// What is the possible distance of travel for the unit
	private float _distance;

	// Keep track of which unit is being focus
	private GameObject _focus;

	// The ratio of how many units in world space to convert to get 1 ortho size for the projection
	// new ortho size is calculated when we got focus, then sizes projection according to that unit's distance
	private float _new_ortho_size;

	// Value. Based on ratio of, _travel_distance formula += (_move_direction * speed).normalized.magnitude * Time.deltaTime;
	// When _travel_distance is 1 "unit" the ortho_size should be ~12.25
	private const float _ratio = 12.25f;

	// Array of projectors
	public Projector[] projectors;

	private MovementStat _movement;

	void Awake ()
	{
		projectors = GetComponentsInChildren<Projector>();
	}

	// Use this for initialization
	void Start () 
	{
		_distance = 0.0f;
		SetProjectionOff();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_focus != null)
		{
			UpdateProjection();
		}
	}
	 
	// Focus to target, get distance, calculate new ortho size for projections
	// If target does not have the UnitController script, pass over.
	// This function should only be called for units and leader characters ONLY
	void SetProjectorFocus(GameObject target)
	{
		if(_focus == null)
		{
			_focus    		  = target;
			_movement		  = _focus.GetComponent<BaseClass>().movement;
			_distance         = _movement.max_distance;
			_new_ortho_size   = _distance*_ratio;

			// Resize
			foreach(Projector p in projectors)
			{
				p.orthographicSize = _new_ortho_size;
			}

			UpdateProjection();
		}
	}
	
	// Align projectors over selected unit
	/// <summary>
	/// Updates the projection. Keep calling this function if you want the projectin to follow the unit
	/// </summary>
	private void UpdateProjection()
	{
		Vector3 new_position      = _focus.transform.position;
		new_position.y            = this.transform.position.y;
		this.transform.position   = new_position;

		_new_ortho_size = Mathf.Clamp(_distance*_ratio - _movement.current_distance*_ratio,
		                              1.0f,
		                              _distance*_ratio);

		// Bad artifact when texture reaches size 1.0 <
		if(_new_ortho_size <= 1.0f)
		{
			_new_ortho_size = 0.0f;
		}

		foreach(Projector p in projectors)
		{
			p.orthographicSize = _new_ortho_size;
		}
	}

	// Turn on projectors
	/// <summary>
	/// Sets the projection on and focuses the desired GameObject
	/// </summary>
	/// <param name="target">Target.</param>
	public void SetProjectionOn(GameObject target)
	{
		SetProjectorFocus(target);

		foreach(Projector p in projectors)
		{
			p.enabled = true;
		}
	}
	// Loses focuses target
	/// <summary>
	/// Sets the projection off and removing the focused GameObject
	/// </summary>
	public void SetProjectionOff()
	{
		foreach(Projector p in projectors)
		{
			p.enabled = false;
		}
			
		_focus = null;
		_movement = null;
	}
}
