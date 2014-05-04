
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * This class will allow the user to change the camera position of their units
 * 
 * 
 * 
 * */


public class TargetTransition : MonoBehaviour {


	#region class variables

	private Vector3 distanceToUnit; //this will be the value of where the camera should be

	private Vector3 newPosition;

	private Quaternion newRot;

	public float smooth = 5.0f, rotation_smooth = 5.0f;

	private int unitIndex = 0;
	private bool interpolate = false;


	#endregion

	// Use this for initialization

	void Start () {
		
	}

	void Awake (){
		//friendlyUnits = GM.instance.GetUnitsFromPlayer(GM.instance.CurrentPlayer);
		unitIndex = 0;
//		MainCamera = CameraController.transform.FindChild ("Main Camera").gameObject;
	}

	// Update is called once per frame
	void Update () {

		if (WorldCamera.instance.IsCameraOnControlsOn() && GM.instance.IsOn) {
			
			int previousUnitIndex = unitIndex;
			bool dirty = HandleUnitSwitching();
			ClampIndexValue();

			//Input keys were called
			if (dirty) {
					
				//These conditions choose which model to put the camera focus on
				distanceToUnit = GetVectorDistanceFromUnit(previousUnitIndex);

				//Find the new position
				//GM.instance.controll
				GM.instance.SetUnitControllerActiveOn ( ref GetCurrentList() [unitIndex] );
				newPosition = GetCurrentList() [unitIndex].transform.position + distanceToUnit;
				interpolate = true;
			}

			float buffer = 0.5f;

			if(interpolate){
				InterpolateToNewPosition();

				if( IsWithinBuffer(buffer) || AreCameraMovementPressed() || WorldCamera.IsMousePositionWithinBoundaries()){
					interpolate = false;
				}
			}
		}

	}

	public GameObject[] GetCurrentList(){
		return GM.instance.GetUnitsFromPlayer(GM.instance.WhichPlayerAmI);
	}
	
	#region Helper Functions
	public GameObject GetFocusedTarget(){
	
		
		return GetCurrentList()[unitIndex];
	
	}

	public static bool AreCameraMovementPressed(){
		if ( Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.A) || 
		     Input.GetKey (KeyCode.D) || (Input.GetAxis("Mouse ScrollWheel") != 0 ) || Input.GetMouseButtonDown (1))
			return true;
		else
			return false;

		}

	public bool IsWithinBuffer(float buffer){

		return (WorldCamera.instance.transform.position.x < newPosition.x + buffer && WorldCamera.instance.transform.position.x > newPosition.x - buffer) &&
			(WorldCamera.instance.transform.position.y < newPosition.y + buffer && WorldCamera.instance.transform.position.y > newPosition.y - buffer) &&
				(WorldCamera.instance.transform.position.z < newPosition.z + buffer && WorldCamera.instance.transform.position.z > newPosition.z - buffer);
	}
				
	// Changing unitIndex to scroll through units
	public bool HandleUnitSwitching(){

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			unitIndex -= 1;
			return true;
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			unitIndex += 1; 
			return true;
		}
		return false;
	}
	//Fixes the unitIndex to prevent segment fault

	//Prevents index from exceeding the index boundaries
	public void ClampIndexValue(){

		if (unitIndex <= -1) {
			
			unitIndex = GetCurrentList().Length - 1;
			
		} else if (unitIndex > GetCurrentList().Length - 1) {
			
			unitIndex = 0;
		}					

	}

	public Vector3 GetVectorDistanceFromUnit(int previousUnitIndex){

		//If the user changes the distance of the camera, but stays focus on the unit
		//keep the same vector distance.
		if (GetCurrentList() [previousUnitIndex].transform.position == WorldCamera.instance.MainCamera.transform.position) {
			return WorldCamera.instance.transform.position - GetCurrentList() [previousUnitIndex].transform.position;
		}
		//If not then use the default vector distance
		else {
			float temp = WorldCamera.instance.MinCameraHeight() + 1;
			return new Vector3(temp, temp, -temp);
		}
	}

	//This is the location to interpolation to.
	void InterpolateToNewPosition(){

		WorldCamera.instance.transform.position = Vector3.Lerp (WorldCamera.instance.transform.position, newPosition, Time.deltaTime * smooth);

		newRot =  Quaternion.FromToRotation( WorldCamera.instance.MainCamera.transform.forward , GetCurrentList() [unitIndex].transform.position - newPosition );
		WorldCamera.instance.MainCamera.transform.Rotate(new Vector3(newRot.eulerAngles.x , 0 , 0));
		WorldCamera.instance.transform.Rotate(new Vector3(0, newRot.eulerAngles.y, 0));

	}

	#endregion
}
