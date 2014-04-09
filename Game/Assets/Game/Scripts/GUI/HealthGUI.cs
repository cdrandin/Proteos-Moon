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

	private Camera _current_camera;

	// Where to parent out GUIs
	private GameObject _root;

	private bool gui_on;

	private float _timer;

	// Use to offset the position of the GUI bar from the start
	private Vector3 _base_pos;

	// Use this for initialization
	void Start () 
	{
		_ratio    = remaining_health.GetComponent<GUITexture>().pixelInset.height/remaining_health.GetComponent<GUITexture>().pixelInset.width;
		_timer    = refresh_time;
		_base_pos = remaining_health.transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GameObject obj = GameObject.Find("Altier_Seita");
			obj.GetComponent<BaseClass>().vital.HP.current -= (obj.GetComponent<BaseClass>().vital.HP.max * 0.1f);
		}

		if(GM.instance.IsOn)
		{
			TurnOn(); // called once

			_timer += Time.deltaTime;
			// Update health bar info based on the refresh rate
			if(_timer >= refresh_time)
			{
				Debug.Log("Refreshing");
				Reset();

				foreach(GameObject unit in GM.instance.GetAllUnitsNearPlayer(GM.instance.CurrentFocusCamera.gameObject, rendering_distance))
				{
					AddTarget(unit);
				}
				_timer = 0;
			}

			// Display the GUITexutres
			for(int i=0;i<_targets.Count;++i)
			{
				DisplayGUITexture(_targets_gui[i].GetComponent<GUITexture>(), _targets[i].transform);
			}
		}

	}

	// Add target to list also with a GUITexture associated with it
	void AddTarget(GameObject obj)
	{
		_targets.Add(obj);

		_targets_gui.Add(Instantiate(remaining_health) as GameObject);
		
		_targets_gui[_targets_gui.Count-1].name = string.Format("{0} GameObject ID: {1}", obj.name, obj.GetInstanceID());
		_targets_gui[_targets_gui.Count-1].transform.parent = _root.transform; // parent to root

		Vital vital = obj.GetComponent<BaseClass>().vital;
		_targets_hp_length.Add((vital.HP.current/vital.HP.max)*100.0f);

		/*
		// copy health_remaining GUITexute into gui_t's GUITexture
		_targets_gui[_targets_gui.Count-1].guiTexture.texture    = remaining_health.guiTexture.texture;
		_targets_gui[_targets_gui.Count-1].guiTexture.color      = remaining_health.guiTexture.color;
		_targets_gui[_targets_gui.Count-1].guiTexture.pixelInset = remaining_health.guiTexture.pixelInset;
		*/
	}

	void TurnOn()
	{
		if(gui_on)
			return;

		_current_camera = GM.instance.CurrentFocusCamera;
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

		_current_camera = null;
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

	void DisplayGUITexture(GUITexture cur_texture, Transform focus)
	{
		RescaleGUITexture(cur_texture, focus);
		RepositionGUITexture(cur_texture, focus);
	}

	void RescaleGUITexture(GUITexture cur_texture, Transform focus)
	{
		_distance = (_current_camera.transform.position - focus.transform.position).magnitude;
		
		cur_texture.pixelInset = new Rect(remaining_health.GetComponent<GUITexture>().pixelInset.x,
		                                  remaining_health.GetComponent<GUITexture>().pixelInset.y,
		                                               (1/_distance)*distance_factor,
		                                               _ratio*(1/_distance)*distance_factor);
	}
	
	void RepositionGUITexture(GUITexture cur_texture, Transform focus)
	{
		Vector3 relativePosition = _current_camera.transform.InverseTransformPoint(focus.position);
		relativePosition.z =  Mathf.Max(relativePosition.z, 1.0f);
		
		Transform gui_transform;
		
		gui_transform = cur_texture.transform;
		
		gui_transform.position = _current_camera.WorldToViewportPoint(_current_camera.transform.TransformPoint(relativePosition + offset));
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
