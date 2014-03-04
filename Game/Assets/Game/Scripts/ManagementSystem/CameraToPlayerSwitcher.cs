using UnityEngine;
using System.Collections;

public class CameraToPlayerSwitcher : Photon.MonoBehaviour {
	private bool toggle = true;
	public WorldCameraModified _world_camera;
	// Use this for initialization
	void Start () {
		_world_camera = GameObject.Find ("WorldCamera").GetComponent<WorldCameraModified>();
		_world_camera.enabled = false;
	}
	// Update is called once per frame
	void Update () {
		if (IsF1Pressed()){
			Camera.main.GetComponent<SmoothFollow>().enabled = !toggle;
			Camera.main.GetComponent<SmoothLookAt>().enabled = !toggle;
			_world_camera.enabled = toggle;
			toggle = !toggle;
		}
	}

	#region Helper functions
	public static bool IsF1Pressed(){
		if(Input.GetKeyUp(KeyCode.F1))
			return true; else return false;
	}
	#endregion
}
