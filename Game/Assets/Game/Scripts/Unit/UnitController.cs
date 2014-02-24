using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animation))]
public class UnitController : MonoBehaviour
{
	/*
	 * Public methods for Unit Controller
	 */
	//HACK to get rid of warnings
	// Animations needed for each unit
	/*public AnimationClip idle_animation;
	public AnimationClip run_animation;
	public AnimationClip jump_animation;*/

	// How fast unit can move
	public float speed;

	// How far the unit should be able to travel
	// Close to ~ in meters perse.
	public float max_travel_distance;

	// Unit allowed to jump
	public bool can_jump;

	// Amount of air-jumps allowed, 0 - only capable of jumping once, 1 - "double" jump (init jump then mid-air jump)
	public int air_jumps;

	// How high the unit can jump
	public float jump_height;

	// aka. gravity, but can also be used to allow certain units to fall more slowly
	public float fall_speed;

	// How fast the unit can rotate
	public float rotation_speed;

	/*
	 * Private methods for Unit Controller
	 */

	// Method for accessing the animation component
	//HACK private Animation _animation;

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

	// Keep track how far the unit has traveled
	private float _travel_distance;

	private CharacterController _cc;

	/*
	// State of the unit
	private enum CharacterState : byte
	{
		Idle = 0,
		Running,
		Jumping,
		Attacking,
		Mining
	};

	private CharacterState character_state;
	*/

	void Awake()
	{	
		//HACK to get rid of warnings
		/*_animation = GetComponent<Animation>();
		if (!idle_animation)
		{
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if (!run_animation)
		{
			_animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if (!jump_animation && can_jump) 
		{
			_animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}*/

		_cc = GetComponent<CharacterController>();
	}

	// Use this for initialization
	void Start() 
	{
		// Forward is the +Z axis
		_move_direction  = Vector3.zero; //transform.TransformDirection(Vector3.forward);
		//HACK _is_jumping      = false;
		_is_controllable = true;
		_vertical_speed  = 0.0f;
		_air_jump_count  = 0;
		_travel_distance = 0.0f;
		//character_state_ = CharacterState.Idle;
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (!_is_controllable /*|| _travel_distance >= max_travel_distance*/)
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
			_travel_distance += (_move_direction * speed).normalized.magnitude * Time.deltaTime;

		_cc.Move(movement);
	}

	void Move()
	{
		if(!_is_controllable)
			return;

		float v = Input.GetAxisRaw("Vertical");//Input.GetAxis("Vertical");
		float h = Input.GetAxisRaw("Horizontal");//Input.GetAxis("Horizontal");

		Vector3 target_direction = h * Vector3.right + v * Vector3.forward;

		if(IsGrounded())
		{
			//HACK _is_jumping = false;
			target_direction.Normalize();
			_move_direction = target_direction;
		}
	}

	void ApplyGravity()
	{
		if (IsGrounded())
			_vertical_speed = 0.0f;
		else
			_vertical_speed -= fall_speed * Time.deltaTime;
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
		return _cc.isGrounded;
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

	void Reset ()
	{
		// Testing numbers that had a "nice" feel
		speed               = 10.0f;
		max_travel_distance = 4.0f;
		can_jump            = true;
		jump_height         = 6.0f;
		fall_speed          = 60.0f;
	}
}
