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
	private Transform target;
	private float y_axis;

	void Awake () 
	{
		y_axis = target.position.y;
	}
	
	void LateUpdate () 
	{
		Debug.Log("*********************************");
		target = GM.instance.CurrentFocus.transform;
		Debug.Log(string.Format("Minimap: Target {0}", target));
		if (target == null)
		{
			target = Camera.main.transform;
		}

		// Set the position of the camera on the x-z plane to:
		transform.position = new Vector3(target.position.x, y_axis, target.position.z);

		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, target.eulerAngles.y, transform.eulerAngles.z);
	}
}