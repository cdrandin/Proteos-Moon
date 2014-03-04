using UnityEngine;
using System.Collections;

public class SmoothLookAt : MonoBehaviour {
	public Transform target;
	public float damping = 6.0f;
	public bool smooth = true;
	private Quaternion rotation;
	// Make the rigid body not change rotation
	void Start () {
		if (rigidbody) {
			rigidbody.freezeRotation = true;
		}
	}
	
	void LateUpdate () {
		if (target) {
			if (smooth)
			{
				// Look at and dampen the rotation
				rotation = Quaternion.LookRotation(target.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
			}
			else
			{
				// Just lookat
				transform.LookAt(target);
			}
		}
	}
	
	#region Helper functions

	#endregion
}

