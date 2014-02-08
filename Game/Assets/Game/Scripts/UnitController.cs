using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animation))]
public class UnitController : MonoBehaviour
{
	/*
	 * Public methods for Unit Controller
	 */

	// Animations needed for each unit
	public AnimationClip idle_animation;
	public AnimationClip run_animation;
	public AnimationClip jump_animation;

	// How far the unit should be able to travel
	public float travel_distance;

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
	private Animation _animation;

	// Move direction allow is the X and Z axis. Y is only affected by incline or jumping
	private Vector3 _move_direction;

	// Speed along the Y-axis. Falling or jumping
	private float _vertical_speed;

	// Know when unit is jumping
	private bool _is_jumping;

	// Check to see if the unit should be allow to move, disabled when unit has done its actions
	private bool _is_controllable;

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
		_animation = GetComponent<Animation>();

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
		}

		_cc = GetComponent<CharacterController>();
	}

	// Use this for initialization
	void Start() 
	{
		// Forward is the +Z axis
		_move_direction  = transform.TransformDirection(Vector3.forward);
		_is_jumping      = false;
		_is_controllable = false;
		_vertical_speed  = 0.0f;

		//character_state_ = CharacterState.Idle;
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (!_is_controllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}

		// Move
		Move();
		// Gravity
		ApplyGravity();

		// Jump

		// Add up all vectors to result in the actions that took place, moving, gravity(i.e. falling), jumping
		Vector3 movement = _move_direction + new Vector3(0, _vertical_speed, 0);
		movement *= Time.deltaTime;

		_cc.Move(movement);
	}

	void Move()
	{
	}

	void ApplyGravity()
	{
		if (IsGrounded())
			_vertical_speed = 0.0f;
		else
			_vertical_speed -= fall_speed * Time.deltaTime;
	}

	bool IsGrounded()
	{
		return _cc.isGrounded;
	}
}
