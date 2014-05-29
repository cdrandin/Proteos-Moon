using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class HorizontalSlowMovement : MonoBehaviour 
{
	public Texture move_normal;
	public Texture move_down;

	private UnitController _uc;
	private Transform _target;
	private Texture _cur_img;
	private bool _gui_on;

	void Start()
	{
		GetComponent<BoxCollider>().isTrigger = true;
		_uc = GameObject.FindGameObjectWithTag("UnitController").GetComponent<UnitController>();
		_gui_on  = false;
		_cur_img = null;
	}
	
	// Entered the movement impairing object
	void OnTriggerEnter(Collider other)
	{
		if(GM.instance.IsItMyTurn())
		{
			if(other.tag == "Unit" || other.tag == "Leader")
			{
				_target = other.transform;
				_uc.mod_movement(MOVEMENT_AFFECT.SLOWED);
				_gui_on  = true;
				_cur_img = move_down;
			}
		}
	}

	// Exit the movement impairing object
	void OnTriggerExit(Collider other)
	{
		if(GM.instance.IsItMyTurn())
		{
			if(other.tag == "Unit" || other.tag == "Leader")
			{
				_target = null;
				_uc.mod_movement(MOVEMENT_AFFECT.NORMAL);
				_gui_on  = false;
				_cur_img = move_normal;
			}
		}
	}

	void OnGUI()
	{
		// we want the gui to be up only when slowed and it is the current player's turn
		if(_gui_on && GM.instance.IsItMyTurn())
		{
			// we have our target to focus
			if(_target != null)
			{
				Vector3 gui_pos = _target.position;
				gui_pos.y += _target.GetComponent<CharacterController>().height;
				Vector2 screen_pos = Camera.main.WorldToScreenPoint(gui_pos);
				GUI.DrawTexture(new Rect(screen_pos.x + _cur_img.width/2.0f, screen_pos.y + _cur_img.height/2.0f, 30.0f, 30.0f), _cur_img);
			}
		}
	}
}
