/*
 * HealthGUI.cs
 * 
 * CHristopher Randin
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthGUI : MonoBehaviour
{
	// GUI Texture to display
	public GameObject remaining_health;

	// Bound what is needed to be rendered within this distance
	public float rendering_distance;

	// How often to display new health bar
	public float refresh_time;

	public float x_render_offset = 0.3f;
	public float y_render_offset = 0.3f;

	// List of targets
	[SerializeField]
	private List<GameObject> _targets;

	// List of GUITextures to go along with these targets
	private List<GameObject> _targets_gui;

	// Used to keep track of the % change of the length of the texture horizontally
	private List<float>      _targets_hp_length;

	// Units in world space to offset; 1 unit above object by default
	public Vector3 offset = Vector3.up;
	public bool clampToScreen = false;  // If true, label will be visible even if object is off screen
	public float clampBorderSize = 0.05f;  // How much viewport space to leave at the borders when a label is being clamped

	// To keep the bar looking nice
	private float _ratio;

	// How far we currently are
	private float _distance;

	public float distance_factor = 1000.0f;

	// Where to parent out GUIs
	private GameObject _root;

	private bool gui_on;

	private float _timer;

	// Use to offset the position of the GUI bar from the start
	private Vector3 _base_pos;

	private float gui_width_offset = -55.78f;

	private Rect screen_rect_w_offsets;

	// Use this for initialization
	void Start () 
	{
		_ratio    = remaining_health.GetComponent<GUITexture>().pixelInset.height/remaining_health.GetComponent<GUITexture>().pixelInset.width;
		_timer    = refresh_time;
		_base_pos = remaining_health.transform.position;

		screen_rect_w_offsets = new Rect(0.0f - x_render_offset, 0.0f - y_render_offset, Screen.width + x_render_offset, Screen.height + y_render_offset);
	}

	// Update is called once per frame
	void Update () 
	{
		if(GM.instance.IsOn)
		{
			TurnOn(); // called once

			_timer += Time.deltaTime;

			// Update health bar info based on the refresh rate
			if(_timer >= refresh_time)
			{
				Reset();

				foreach(GameObject unit in GM.instance.GetAllUnitsNearPlayer(GM.instance.CurrentFocusCamera.gameObject, rendering_distance))
				{
					Vector3 unit_screen_pos = GM.instance.CurrentFocusCamera.WorldToViewportPoint(unit.transform.position);

					if(screen_rect_w_offsets.Contains(new Vector2(unit_screen_pos.x, unit_screen_pos.y)))
					{
						AddTarget(unit);
					}
				}
				_timer = 0;
			}

			// Display the GUITexutres
			for(int i=0;i<_targets.Count;++i)
			{
				DisplayGUITexture(_targets_gui[i].GetComponent<GUITexture>(), _targets[i].transform, _targets_hp_length[i]);
			}
		}	}

	// Add target to list also with a GUITexture associated with it
	void AddTarget(GameObject obj)
	{
		_targets.Add(obj);

		_targets_gui.Add(Instantiate(remaining_health) as GameObject);
		
		_targets_gui[_targets_gui.Count-1].name = string.Format("{0} GameObject ID: {1}", obj.name, obj.GetInstanceID());
		_targets_gui[_targets_gui.Count-1].transform.parent = _root.transform; // parent to root

		Vital vital = obj.GetComponent<BaseClass>().vital;
		_targets_hp_length.Add((vital.HP.current/vital.HP.max)*100.0f);
	}

	void TurnOn()
	{
		if(gui_on)
			return;

		_root           = this.gameObject;
		gui_on 			= true;
		_timer  		= refresh_time;

		if(GM.instance.IsOn)
		{
			_targets           = new List<GameObject>();
			_targets_gui       = new List<GameObject>();
			_targets_hp_length = new List<float>();
		}
	}
	
	void TurnOff()
	{
		if(!gui_on)
			return;

		Reset();
		gui_on = false;
	}

	void Reset()
	{
		foreach(GameObject unit in _targets_gui)
		{
			Destroy(unit);
		}
		_targets.Clear();
		_targets_gui.Clear();
		_targets_hp_length.Clear();
	}

	void DisplayGUITexture(GUITexture cur_texture, Transform focus, float ratio)
	{
		RescaleGUITexture(cur_texture, focus, ratio);
		RepositionGUITexture(cur_texture, focus);
	}

	void RescaleGUITexture(GUITexture cur_texture, Transform focus, float ratio)
	{
		_distance = (GM.instance.CurrentFocusCamera.transform.position - focus.transform.position).magnitude;

		cur_texture.pixelInset = new Rect(remaining_health.GetComponent<GUITexture>().pixelInset.x,
		                                  remaining_health.GetComponent<GUITexture>().pixelInset.y,
		                                  gui_width_offset + ratio,
		                                  _ratio*(1/_distance)*distance_factor);
	}
	
	void RepositionGUITexture(GUITexture cur_texture, Transform focus)
	{
		Vector3 relativePosition = GM.instance.CurrentFocusCamera.transform.InverseTransformPoint(focus.position);
		relativePosition.z =  Mathf.Max(relativePosition.z, 1.0f);
		
		Transform gui_transform;
		
		gui_transform = cur_texture.transform;
		
		gui_transform.position = GM.instance.CurrentFocusCamera.WorldToViewportPoint(GM.instance.CurrentFocusCamera.transform.TransformPoint(relativePosition + offset));
		gui_transform.position = new Vector3(Mathf.Clamp(gui_transform.position.x, clampBorderSize, 1.0f - clampBorderSize),
		                                     Mathf.Clamp(gui_transform.position.y, clampBorderSize, 1.0f - clampBorderSize),
		                                     gui_transform.position.z);
	}

	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.green;
		for(int i=1;i<=GameObject.FindObjectsOfType<Camera>().Length;++i)
		{
			Gizmos.DrawWireSphere(GameObject.Find(string.Format("camera_player{0}", i)).transform.position, rendering_distance);
		}
	}
}
