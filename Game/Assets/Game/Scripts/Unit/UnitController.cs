/*
 * UnitController.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MOVEMENT_AFFECT
{
	NORMAL,
	SLOWED,
	HASTED
}

public class UnitController : Photon.MonoBehaviour
{
	/*
	 * Public methods for Unit Controller
	 */
	// How fast unit can move
	[SerializeField]
	private float _speed;

	[SerializeField]
	private float rotation_speed;

	// How far the unit should be able to travel
	// Close to ~ in meters perse.
	[SerializeField]
	private float max_travel_distance;

	// Unit allowed to jump
	private bool can_jump;

	// Amount of air-jumps allowed, 0 - only capable of jumping once, 1 - "double" jump (init jump then mid-air jump)
	private int air_jumps;

	// How high the unit can jump
	private float jump_height;

	// aka. gravity, but can also be used to allow certain units to fall more slowly
	private float fall_speed;

	// If true, player will stop when distance traveled is met
	[SerializeField]
	private bool enforce_distance;
	/*
	 * Private methods for Unit Controller
	 */

	// Move direction allow is the X and Z axis. Y is only affected by incline or jumping
	private Vector3 _move_direction;

	// Speed along the Y-axis. Falling or jumping
	private float _vertical_speed;

	// Know when unit is jumping
	//HACK private bool _is_jumping;

	// Keep track how many times allow to jump in air
	private int _air_jump_count;

	// Check to see if the unit should be allow to move, disabled when unit has done its actions
	private bool _is_controllable;

	// This is the unit of focus that is moving
	private CharacterController _unit_focus_cc;

	// Keep track of the projector for how far a unit can move
	private DistanceProjection _distance_proj;

	// Keep track of units' movement stat script
	private MovementStat _unit_focus_movement;

	private Vector3 _start;

	// How far the player has ran in terms of path traveled
	private float _steps;

	// path player has taken to record location
	private List<Vector3> _path;

	// This is how much distance is required till we get a new position when moving, compares using sqrmagnitude
	[Range(0.1f, 1.0f)]
	public float distance_till_next_sample;

	// How often to check for a new position
	[Range(0.1f, 1.0f)]
	public float sample_rate;

	private float _movement_mod;

	void Awake()
	{	
		_distance_proj = GameObject.FindObjectOfType<DistanceProjection>();
	}

	// Use this for initialization
	void Start() 
	{
		// Forward is the +Z axis
		_move_direction  	= Vector3.zero; //transform.TransformDirection(Vector3.forward);
		ShutDown();
		ClearFocusUnit();
		_steps = 0.0f;
		mod_movement(MOVEMENT_AFFECT.NORMAL);
		_path  = new List<Vector3>();
	}

	IEnumerator CollectPositions()
	{
		float delta = sample_rate;

		for(;;)
		{
			if(_unit_focus_cc != null)
			{
				Vector3 cur = _unit_focus_cc.transform.position;
				if(_path.Count == 0)
				{
					_path.Add(cur);
				}
				else
				{
					if((cur - _path[_path.Count-1]).sqrMagnitude >= distance_till_next_sample * 2)
					{
						_path.Add(cur);
					}
				}
			}

			yield return new WaitForSeconds(delta);
		}
	}

	float DistanceTraveledAlongPath()
	{
		float distance = 0.0f;
		Vector3 prev = _path[0];

		for(int i=1;i<_path.Count;++i)
		{
			distance += (_path[i] - prev).sqrMagnitude;
			prev = _path[i];
		}

		return distance/_speed;
	}

	/// <summary>
	/// Returns movement left amount within 0 and 1. This is the before step when x100 to get the %
	/// </summary>
	/// <returns>The left.</returns>
	public float MovementLeft()
	{
		return 1.0f - Mathf.Clamp(DistanceTraveledAlongPath()/max_travel_distance,0.0f, 1.0f);
	}

	public float MovementScalar()
	{
		Vector3 horizontalVelocity = new Vector3(_unit_focus_cc.velocity.x, 0, _unit_focus_cc.velocity.z);
		return horizontalVelocity.magnitude;
	}
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		if(!GM.instance.IsItMyTurn())
			return;

		// Controller is found, then use UnitController
		if(_unit_focus_cc != null)
		{
			Vector3 dif = _unit_focus_cc.transform.position - _start;

			if (!_is_controllable || DistanceTraveledAlongPath() >= max_travel_distance)
			{
				Input.ResetInputAxes();
			}

			// Move
			Move();

			// Gravity
			ApplyGravity();

			// Jump
			Jump();

			// Add up all vectors to result in the actions that took place, moving, gravity(i.e. falling), jumping
			Vector3 movement = _move_direction * _speed * _movement_mod + new Vector3(0, _vertical_speed, 0);
			movement *= Time.deltaTime;

			_unit_focus_cc.Move(movement);

			_steps += _unit_focus_cc.velocity.magnitude/_speed;
		}

		if(GM.instance.IsNextPlayersTurn())
			_unit_focus_cc = null;
	}

	void Move()
	{
		if(!_is_controllable)
		{
			return;
		}

		float v    = Input.GetAxisRaw("Unit_Vertical");
		float turn = Input.GetAxisRaw("Unit_Horizontal");

		if(v < 0.0)
			v = 0.0f;

		Vector3 target_direction = v * _unit_focus_cc.transform.forward;
		if(_unit_focus_cc.GetComponent<BaseClass>().unit_status.status.Move)
			_unit_focus_cc.transform.Rotate(0, turn * rotation_speed * Time.deltaTime, 0);

		if(IsGrounded())
		{
			//HACK _is_jumping = false;
			target_direction.Normalize();
			_move_direction = target_direction;
		}
	}

	void ApplyGravity()
	{
		//_vertical_speed -= fall_speed * Time.deltaTime;
		//*
		if (IsGrounded())
			_vertical_speed = 0.0f;
		else
			_vertical_speed -= fall_speed * Time.deltaTime;
		//	*/
	}

	void Jump()
	{
		//bool jumping = can_jump && Input.GetAxis("Jump") > 0.9f;
		bool jumping = can_jump && Input.GetKeyDown(KeyCode.Space);
		
		if(jumping)
		{
			// Allow jump off ground
			if(IsGrounded())
			{
				_vertical_speed = CalculateJumpVerticalSpeed(jump_height);
				//HACK _is_jumping = true;
				_air_jump_count = 0;
			}

			// Mid-air jump
			else if(air_jumps > 0 &&_air_jump_count < air_jumps) 
			{
				++_air_jump_count;
				_vertical_speed += CalculateJumpVerticalSpeed(jump_height);
			}
		}
	}

	bool IsGrounded()
	{
		return _unit_focus_cc.isGrounded;
	}

	float CalculateJumpVerticalSpeed(float height)
	{
		return Mathf.Sqrt(2 * height * fall_speed);
	}

	public bool IsMoving()
	{
		return Mathf.Abs(Input.GetAxis("Horizontal")) > 0.05f  || Mathf.Abs(Input.GetAxis("Vertical")) > 0.05f;
	}

	public bool IsMovingForward()
	{
		return Input.GetAxisRaw("Vertical") > 0;
	}

	public bool GetIsControllable()
	{
		return _is_controllable;
	}

	public void SetIsControllable(bool v)
	{
		_is_controllable = v;
		if(_is_controllable)
		{
			// Set distance projector to focus unit
			_distance_proj.SetProjectionOn(_unit_focus_cc.gameObject);
		}
		else
		{
			// Set distance projector to unfocus
			_distance_proj.SetProjectionOff();
		}
	}

	public float GetMaxDistance()
	{
		return max_travel_distance;
	}

	public float travel_distance
	{
		get { return Vector3.Distance(_unit_focus_cc.transform.position, _start); }
	}

	public void SetFocusOnUnit(ref GameObject unit)
	{
		// Move valid unit
		if(unit.tag == "Unit" || unit.tag == "Leader")
		{
			// Get controller
			_unit_focus_cc = unit.GetComponent<CharacterController>();

			// If unit is mising controller
			if(_unit_focus_cc == null)
			{
				Debug.LogWarning(string.Format("{0} unit is missing a CharacterController! Putting one on it now.", unit.name));
				return;
			}

			// Get movementstat
			BaseClass bc = unit.GetComponent<BaseClass>();

			// If unit movement script missing
			if(bc == null)
			{
				Debug.LogWarning(string.Format("{0} unit is missing a BaseClass! Put it on now.", unit.name));
				return;
			}

			_unit_focus_movement = bc.movement;

			// Setup UnitController's variables with selected unit's movement info
			Setup();

			// Assume we got what we need now.
			_start = _unit_focus_cc.gameObject.transform.position;
			_steps = 0.0f;
			_path.Clear();
			StopCoroutine("CollectPositions");
			StartCoroutine("CollectPositions");

			//SetIsControllable(true);
		}
		else
		{
			Debug.LogWarning(string.Format("{0} object s trying to be moved by UnitController and SHOULDN'T", unit.name));
		}
	}

	public void ClearFocusUnit()
	{
		_unit_focus_cc = null;

		// Reset UnitController's variables to 0
		ShutDown();

		// Set distance projector to unfocus
		_distance_proj.SetProjectionOff();
	}

	public GameObject UnitControllerFocus
	{
		get 
		{
			if(_unit_focus_cc != null)
			{
				return _unit_focus_cc.gameObject;
			}
			else
			{
				return null;
			}
		}
	}

	void Setup()
	{		
		max_travel_distance = _unit_focus_movement.max_distance;
		_speed              = _unit_focus_movement.speed;
		can_jump            = _unit_focus_movement.can_jump;
		jump_height         = _unit_focus_movement.jump_height;
		rotation_speed      = _unit_focus_movement.rotation_speed;

		if(_unit_focus_movement.fall_speed == 0)
		{
			Debug.LogWarning(string.Format("{0}'s MovementState fall_speed is 0, thus won't be falling and the UnitController will break. Putting the default 60.", _unit_focus_cc.gameObject.name));
			fall_speed 		= 60.0f;
		}
		else
		{
			fall_speed          = _unit_focus_movement.fall_speed;
		}

		air_jumps 			= _unit_focus_movement.air_jumps;
	}

	public void mod_movement(MOVEMENT_AFFECT affect)
	{
		switch(affect)
		{
		case MOVEMENT_AFFECT.NORMAL:
			_movement_mod = 1.0f;
			break;
		case MOVEMENT_AFFECT.HASTED:
			_movement_mod = 1.5f;
			break;
		case MOVEMENT_AFFECT.SLOWED:
			_movement_mod = 0.5f;
			break;
		}
	}

	/// <summary>
	/// Sets the speed for both the unit controller and movementstat's speed
	/// </summary>
	/// <value>The speed.</value>
	public float speed
	{
		set	{ _speed = value; }
	}

	void ShutDown()
	{
		_unit_focus_movement = null;

		max_travel_distance = 0;
		_speed              = 0;
		can_jump            = false; 
		jump_height         = 0;
		fall_speed          = 0;
		air_jumps 			= 0;
	}

	/*
	void Reset ()
	{
		// Testing numbers that had a "nice" feel
		speed               = 10.0f;
		can_jump            = true;
		jump_height         = 6.0f;
		fall_speed          = 60.0f;
	}
	*/
}
