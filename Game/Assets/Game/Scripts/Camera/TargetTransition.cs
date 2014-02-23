using UnityEngine;
using System.Collections;

/*
 * This class will allow the user to change the camera position of their units
 * 
 * 
 * 
 * */


public class TargetTransition : MonoBehaviour {


	#region class variables
	
	public GameObject CameraController;
	private GameObject MainCamera;

	private GameObject[] unitArray; // need to recreate so it can be used as a list
	private Vector3 distanceToUnit; //this will be the value of where the camera should be

	private Vector3 newPosition;

	private Quaternion newRot;

	public float smooth = 30f, rotation_smooth = 30f;

	private int unitIndex = 0;
	private bool interpolate = false;


	#endregion

	// Use this for initialization

	void Start () {}

	void Awake (){
		unitArray = GameObject.FindGameObjectsWithTag ("Unit");
		unitIndex = 0;
		MainCamera = CameraController.transform.FindChild ("Main Camera").gameObject;
	}

	// Update is called once per frame
	void Update () {
	
		bool cameraOn = true;

		if (cameraOn) {

			int previousUnitIndex = unitIndex;
			bool dirty = HandleUnitSwitching();
			ClampIndexValue();

			//Input keys were called
			if (dirty) {
					
				//These conditions choose which model to put the camera focus on
				distanceToUnit = GetVectorDistanceFromUnit(previousUnitIndex);

				//Find the new position
				newPosition = unitArray [unitIndex].transform.position + distanceToUnit;
				newRot = Quaternion.LookRotation (newPosition, new Vector3(0,1,0));
				print ("Initial Euler Angles" + MainCamera.transform.rotation.eulerAngles);
//				print ("Euler anlges: " + newRot.eulerAngles);

				interpolate = true;
			}

			float buffer = 0.5f;

			if(interpolate){
				InterpolateToNewPosition();

				if( IsWithinBuffer(buffer) || AreCameraMovementPress() || WorldCamera.IsMousePositionWithinBoundaries()){
					interpolate = false;
					print ("Current Euler Angles" + MainCamera.transform.rotation.eulerAngles);
				}
			}
		}
	}


	#region Helper Functions

	public bool AreCameraMovementPress(){
		if ( Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.A) || 
		     Input.GetKey (KeyCode.D) || (Input.GetAxis("Mouse ScrollWheel") != 0 ) || Input.GetMouseButtonDown (1))
			return true;
		else
			return false;

		}

	public bool IsWithinBuffer(float buffer){

		return (CameraController.transform.position.x < newPosition.x + buffer && CameraController.transform.position.x > newPosition.x - buffer) &&
			(CameraController.transform.position.y < newPosition.y + buffer && CameraController.transform.position.y > newPosition.y - buffer) &&
				(CameraController.transform.position.z < newPosition.z + buffer && CameraController.transform.position.z > newPosition.z - buffer);
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
			
			unitIndex = unitArray.Length - 1;
			
		} else if (unitIndex > unitArray.Length - 1) {
			
			unitIndex = 0;
		}					

	}

	public Vector3 GetVectorDistanceFromUnit(int previousUnitIndex){

		//If the user changes the distance of the camera, but stays focus on the unit
		//keep the same vector distance.
		if (unitArray [previousUnitIndex].transform.position == MainCamera.transform.position) {
			return CameraController.transform.position - unitArray [previousUnitIndex].transform.position;
		}
		//If not then use the default vector distance
		else {
			return new Vector3(-50, 40, -50);
		}
	}

	void InterpolateToNewPosition(){

		CameraController.transform.position = Vector3.Lerp (CameraController.transform.position, newPosition, Time.deltaTime * smooth);
		//print ("CameraController Euler Angles" + CameraController.transform.rotation.eulerAngles);
		//print ("MainCamera Euler Angles" + MainCamera.transform.rotation.eulerAngles);
		newRot =  Quaternion.FromToRotation( MainCamera.transform.forward , 
		                                        unitArray [unitIndex].transform.position - newPosition );
		
		print ("New Rotation" + newRot.eulerAngles);
		MainCamera.transform.Rotate(new Vector3(newRot.eulerAngles.x , 0 , 0));
		CameraController.transform.Rotate(new Vector3(0, newRot.eulerAngles.y, 0));
		//print ("Euler X angle: " + newRot.eulerAngles.x + ", Rotation X: " + newRot.x);
		//print ("Euler Y angle: " + newRot.eulerAngles.y + ", Rotation Y: " + newRot.y);
		//CameraController.transform.LookAt (unitArray [unitIndex].transform.position);
		//MainCamera.transform.LookAt (unitArray [unitIndex].transform.position, new Vector3(0,1,0));
	//	MainCamera.transform.rotation = Quaternion.Lerp (MainCamera.transform.rotation, newRot, Time.deltaTime * rotation_smooth);

	}

	#endregion
}
