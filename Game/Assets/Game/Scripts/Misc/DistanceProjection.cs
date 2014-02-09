using UnityEngine;
using System.Collections;

/*
 * Usuage:
 * Projection display over a unit, if ABLE to move still.
 * Call 'SetProjectorFocus' with 1 arg, GameObject 'target'
 *      This will project a ring over the unit's starting position displaying
 *      how far the unit can move with respect to its distance
 */

public class DistanceProjection : MonoBehaviour
{
	// What is the possible distance of travel for the unit
	private float _distance;

	// Keep track of which unit is being focus
	private GameObject _focus;

	// The ratio of how many units in world space to convert to get 1 ortho size for the projection
	// By testing, it is ((10.0f/12.0f) units/1 ortho_size)
	// new ortho size is calculated when we got focus, then sizes projection according to that unit's distance
	private float _new_ortho_size;

	// Array of projectors
	private Projector[] projectors;

	void Awake ()
	{
		projectors = new Projector[2];
	}

	// Use this for initialization
	void Start () 
	{
		_focus    = null;
		_distance = 0.0f;
		projectors = GetComponentsInChildren<Projector>();
		SetProjectionOn(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.T))
		{
			SetProjectionOn(true);
			SetProjectorFocus(GameObject.FindGameObjectWithTag("Unit") as GameObject);
		}

		if(_focus != null)
		{ 
			// There is a target
			//_focus.GetComponent<UnitController>();

			// Get target's capable travel distance
			//_distance = _focus.GetDistance();
		}
	}
	 
	// Focus to target, get distance, calculate new ortho size for projections
	// If target does not have the UnitController script, pass over.
	// This function should only be called for units and leader characters ONLY
	public void SetProjectorFocus(GameObject target)
	{
		UnitController uc = target.GetComponent<UnitController>() as UnitController;

		if(uc == null)
		{
			Debug.LogError(string.Format("Trying to project onto {0}, but it does not contain the UnitController script.", target.name));
			return;
		}

		_focus            = target;
		_distance         = uc.GetMaxDistance();
		_new_ortho_size   = _distance/(10.0f/12.0f); // ~ value

		print (_new_ortho_size);

		AdjustProjection();
	}

	void AdjustProjection()
	{
		Vector3 new_position = _focus.transform.position;
		new_position.y       = transform.position.y;
		transform.position = new_position;

		foreach(Projector p in projectors)
			p.orthographicSize = _new_ortho_size;
	}

	void SetProjectionOn(bool v)
	{
		foreach(Projector p in projectors)
			p.enabled = v;
	}
}
