/*
 * UnitController.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class UnitController : MonoBehaviour
{
	/*
	 * Public methods for Unit Controller
	 */
	// How fast unit can move
	[SerializeField]
	private float speed;

	// Keep track how far the unit has traveled
	[SerializeField]
	private float _travel_distance;

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

	void Awake()
	{	
		_distance_proj = GameObject.FindObjectOfType<DistanceProjection>();
	}

	// Use this for initialization
	void Start() 
	{
		// Forward is the +Z axis
		_move_direction  	= Vector3.zero; //transform.TransformDirection(Vector3.forward);
		//HACK _is_jumping      = false;
		_is_controllable 	= true;
		_vertical_speed  	= 0.0f;
		_air_jump_count  	= 0;
		_travel_distance 	= 0.0f;
		max_travel_distance = 0.0f;

		ClearFocusUnit();
	}
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		// Controller is found, then use UnitController
		if(_unit_focus_cc != null)
		{
			if (!_is_controllable || ( enforce_distance && (_travel_distance >= max_travel_distance)))
			{
				//
				// MAY CAUSE PROBLEMS IN THE FUTURE !!!
				//
				// kill all inputs if not controllable.
				Input.ResetInputAxes();
			}

			// Move
			Move();

			// Gravity
			ApplyGravity();

			// Jump
			Jump();

			// Add up all vectors to result in the actions that took place, moving, gravity(i.e. falling), jumping
			Vector3 movement = _move_direction * speed + new Vector3(0, _vertical_speed, 0);
			movement *= Time.deltaTime;

			// Just adding some numbers to get distance traveled
			if(IsMoving())
			{
				_travel_distance += (_move_direction * speed).normalized.magnitude * Time.deltaTime;
			}

			//movement = transform.TransformDirection(movement);

			_unit_focus_cc.Move(movement);
		}
	}

	void Move()
	{
		if(!_is_controllable)
		{
			return;
		}

		float v = Input.GetAxisRaw("Vertical");//Input.GetAxis("Vertical");
		float h = Input.GetAxisRaw("Horizontal");//Input.GetAxis("Horizontal");

		//Vector3 target_direction = h * Vector3.right + v * Vector3.forward;
		Vector3 target_direction = h * _unit_focus_cc.transform.right + v * _unit_focus_cc.transform.forward;

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

	bool IsMoving()
	{
		return Mathf.Abs(Input.GetAxis("Horizontal")) > 0.05f  || Mathf.Abs(Input.GetAxis("Vertical")) > 0.05f;
	}

	public bool GetIsControllable()
	{
		return _is_controllable;
	}

	public void SetIsControllable(bool v)
	{
		_is_controllable = v;
	}

	public float GetMaxDistance()
	{
		return max_travel_distance;
	}

	public float travel_distance
	{
		get { return _travel_distance; }
		set { _travel_distance = value; }
	}

	public void ResetDistanceTraveled()
	{
		_travel_distance = 0.0f;
	}

	public void SetFocusOnUnit(ref GameObject unit)
	{
		// -1 because enum starts at 0 for player1
		int player_num = int.Parse(unit.transform.parent.tag[unit.transform.parent.tag.Length-1].ToString()) - 1;

		// Check for player moving their own units
		if((Player)player_num != GM.instance.CurrentPlayer)
		{
			Debug.LogWarning("Not your turn to move that unit");
			return;
		}

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

			// Set distance projector to focus unit
			//_distance_proj.SetProjectionOn(unit);

			// Assume we got what we need now.
			_unit_focus_cc.detectCollisions = false;
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

	public GameObject GetUnitControllerFocus()
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

	void Setup()
	{		
		max_travel_distance = _unit_focus_movement.max_distance;
		speed               = _unit_focus_movement.speed;
		can_jump            = _unit_focus_movement.can_jump;
		jump_height         = _unit_focus_movement.jump_height;

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

	void ShutDown()
	{
		_unit_focus_movement = null;

		_travel_distance 	= 0;
		max_travel_distance = 0;
		speed               = 0;
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
