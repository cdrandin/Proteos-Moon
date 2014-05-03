﻿/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/
using UnityEngine;
using System.Collections;

public class SmoothFollowLeader: MonoBehaviour {
	// The target we are following
	public Transform target;
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
	
	void Awake () {
		target = GM.instance.Photon_Leader.transform;
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
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		
		// Set the height of the camera
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
		
		// Always look at the target
		transform.LookAt (target);
	}
}
