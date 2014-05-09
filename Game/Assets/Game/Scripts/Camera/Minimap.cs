/*
 * UnitController.cs
 * 
 * Christopher Randin
 */
using UnityEngine;
using System.Collections;

public class Minimap: MonoBehaviour 
{
	// The target we are following
	private GameObject target;
	private float y_axis;

	private UnitGUI _unit_gui;

	void Awake () 
	{
		target = Camera.main.gameObject;
		_unit_gui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UnitGUI>();
		y_axis = target.transform.position.y;
	}
	
	void LateUpdate () 
	{
		//Debug.Log(GM.instance.CurrentFocus);

		target = _unit_gui.focus_object;
		//Debug.Log(string.Format("Minimap: Target {0}", (target == null)?"NULL":target.name));
		if (target == null)
		{
			target = Camera.main.gameObject;
		}

		// Set the position of the camera on the x-z plane to:
		transform.position = new Vector3(target.transform.position.x, y_axis, target.transform.position.z);

		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, target.transform.eulerAngles.y, transform.eulerAngles.z);
	}
}