using UnityEngine;
using System.Collections;

public class GatherGUI : MonoBehaviour 
{
	public Texture gather_gui;
	private bool _gui_on;
	private Transform _target;
	private Texture _cur_img;
	public float scalar;
	private Vector2 _screen_pos;
	private bool _once;

	void Start()
	{
		scalar = 0.5f;
		_once = true;
		_target = null;
		_cur_img = gather_gui;
		_gui_on = false;
	}

	public bool gui_on
	{
		set 
		{
			_gui_on = value; 
			_target = GM.instance.CurrentFocus.transform;
			_once   = true;
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
				gui_pos.y += _target.GetComponent<CharacterController>().height*scalar;
				Vector2 screen_pos = Camera.main.WorldToScreenPoint(gui_pos);
				if(_once)
				{
					_screen_pos = screen_pos;
					_once = false;
				}

				GUI.DrawTexture(new Rect(_screen_pos.x + _cur_img.width/2.0f, _screen_pos.y + _cur_img.height/2.0f, 30.0f, 30.0f), _cur_img);
			}
		}
	}
}