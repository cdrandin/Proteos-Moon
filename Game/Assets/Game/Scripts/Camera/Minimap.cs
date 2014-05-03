/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/
using UnityEngine;
using System.Collections;

public class Minimap: MonoBehaviour 
{
	// The target we are following
	private Transform target;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	
	private float wantedRotationAngle;
	private float wantedHeight;
	private float currentRotationAngle;
	private float currentHeight;
	private Quaternion currentRotation;
	//public GameObject camera;
	private float y_axis;

	void Awake () {
		target = Camera.main.transform;
		y_axis = target.position.y;
	}
	
	void LateUpdate () {
		// Early out if we don't have a target
		if (!target)
			return;
		
		// Calculate the current rotation angles
		wantedRotationAngle = target.eulerAngles.y;
		wantedHeight = target.position.y + height;
		
		currentRotationAngle = transform.eulerAngles.y;
		currentHeight = transform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = new Vector3(target.position.x, y_axis, target.position.z);

		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, target.eulerAngles.y, transform.eulerAngles.z);
	}
}

