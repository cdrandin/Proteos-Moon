
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

	public float smooth = 5.0f, rotation_smooth = 5.0f;

	private int unitIndex = 0;

	private GameObject[] CurrentUnitList;

	#endregion

	// Use this for initialization

	void Start () {
		CurrentUnitList =  GM.instance.GetUnitsFromPlayer(GM.instance.WhichPlayerAmI);
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


			//Input keys were called
			if (dirty) {
				UpdateCurrentList();
				ClampIndexValue();
				//These conditions choose which model to put the camera focus on
				distanceToUnit = GetVectorDistanceFromUnit(previousUnitIndex);

				//Find the new position
				//GM.instance.controll
				//UnitGUI.instance.UpdateUnitInformation();
				GM.instance.SetUnitControllerActiveOff();
				GM.instance.SetUnitControllerActiveOn ( ref CurrentUnitList [unitIndex] );
				newPosition = CurrentUnitList [unitIndex].transform.position + distanceToUnit;
				
				StopCoroutine("InterpolateToNewPosition");
				StartCoroutine("InterpolateToNewPosition");
			}

		}

	}
	
	

	public void UpdateCurrentList(){
		CurrentUnitList =  GM.instance.GetUnitsFromPlayer(GM.instance.WhichPlayerAmI);
	}
	
	#region Helper Functions
	public GameObject GetFocusedTarget(){
	
		
		return CurrentUnitList[unitIndex];
	
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
			
			unitIndex = CurrentUnitList.Length - 1;
			
		} else if (unitIndex >= CurrentUnitList.Length) {
			
			unitIndex = 0;
		}
		


	}

	public Vector3 GetVectorDistanceFromUnit(int previousUnitIndex){

		//If the user changes the distance of the camera, but stays focus on the unit
		//keep the same vector distance.
		
		Quaternion newRot =  Quaternion.FromToRotation( WorldCamera.instance.MainCamera.transform.forward , CurrentUnitList [unitIndex].transform.position - newPosition );

		
		if ( Mathf.Abs( WorldCamera.instance.MainCamera.transform.eulerAngles.x - newRot.eulerAngles.x   ) < 1.0f 
		    &&  Mathf.Abs( WorldCamera.instance.transform.eulerAngles.y - newRot.eulerAngles.y   ) < 1.0f ){
		    
			return WorldCamera.instance.transform.position - CurrentUnitList [previousUnitIndex].transform.position;
		}
		//If not then use the default vector distance
		else {
			float temp = WorldCamera.instance.minDistanceToObject + CurrentUnitList [ unitIndex ].GetComponent<CapsuleCollider>().height ;
			return new Vector3(temp, 
							   temp , 
			                   -(temp ) );
		}
	}

	//This is the location to interpolate to.
	
	private IEnumerator InterpolateToNewPosition(){
	
		Quaternion newRot =  Quaternion.FromToRotation( WorldCamera.instance.MainCamera.transform.forward , CurrentUnitList [unitIndex].transform.position - newPosition );
		WorldCamera.instance.MainCamera.transform.Rotate(new Vector3(newRot.eulerAngles.x , 0 , 0));
		WorldCamera.instance.transform.Rotate(new Vector3(0, newRot.eulerAngles.y, 0));
	
		while( !GM.WithinEpsilon(WorldCamera.instance.transform.position, newPosition, 0.001f) && !WorldCamera.AreCameraKeyboardButtonsPressed() ){
		
			WorldCamera.instance.transform.position = Vector3.Lerp (WorldCamera.instance.transform.position, newPosition, Time.deltaTime * smooth);	
			yield return null;
		}
		
		WorldCamera.instance.transform.position = newPosition;
		
	}
	

	#endregion
}
