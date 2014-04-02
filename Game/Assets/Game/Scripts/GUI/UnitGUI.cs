using UnityEngine;
using System.Collections;

public class UnitGUI : MonoBehaviour {


	#region class_variables	
	private delegate void GUIMethod();
	private GUIMethod gui_method;
	private UnitCost _unit_cost;
	private GameObject focusTemp, focusObject, worldCamera, mainCamera;
	private bool init, smoothPos;
	private float height = 5.0f;
	private float heightDamping = 2.0f;
	private float rotationDamping = 3.0f;
	private float button_pos = Screen.width - 250;
	private float wantedRotationAngle;
	private float wantedHeight;
	private float currentRotationAngle;
	private float currentHeight;
	private Quaternion currentRotation;
	private Transform from;
	private bool movement;
	#endregion
	
	// Use this for initialization
	void Awake(){
	
	}
	
	void Start () {
		worldCamera = GameObject.Find("WorldCamera");
		focusObject = null;
		focusTemp = null;
		init = false;
		movement = false;
		smoothPos = false;
	}
	
	
	// Update is called once per frame
	void Update () {
		if(GameManager.IsOn())
		{
			focusTemp = GameManager.GetCurrentFocus();
			
			if(!init && focusTemp != null){
				focusObject = focusTemp;
				GameManager.SetUnitControllerActiveOff();
				this.gui_method += UnitsOptions;
				init = true;
			}
			RemoveGUI();
		}
		
	}
	
	void LateUpdate(){
		if(movement && focusObject != null){
			SmoothFollow(focusObject.transform);
		}
		
	}
	
	void RemoveGUI(){
	
		if( Input.GetKeyUp(KeyCode.Escape) || WorldCameraModified.AreCameraKeyboardButtonsPressed() ){
		
			focusObject = null;
			this.gui_method -= UnitsOptions;
			init = false;
		}
	}
	void OnGUI(){
	
		if(this.gui_method != null ){
		
			this.gui_method();
		}
	}

	void UnitsOptions(){
		
		if(MakeButton(button_pos,0,"Attack")){
			
			
		}
		else if(MakeButton(button_pos, 50, "Movement")){
			if( GameManager.GetCurrentPlayer() == 0 ){			
				mainCamera = GameObject.Find ("camera_player1");
				}
			else{
				mainCamera = GameObject.Find ("camera_player2");
			}
			
			GameManager.SetUnitControllerActiveOn(ref focusObject);
			this.gui_method -= UnitsOptions;
			this.gui_method += MovementActive;
			smoothPos = true;
			worldCamera.transform.eulerAngles = Vector3.zero;
		}
		else if(MakeButton(button_pos, 100, "Wait")){
			//Expend units action
			GameManager.SetUnitControllerActiveOff();
			this.gui_method -= UnitsOptions;
			focusObject = null;
			init = false;
		}
	}
	
	void MovementActive(){

		movement = true;
		if(MakeButton(button_pos, 0, "End Movement")){
			GameManager.SetUnitControllerActiveOff();
			this.gui_method -= MovementActive;
			this.gui_method += UnitsOptions;
			movement = false;
			RestCamera();
	//		Pop ();
		}
		
	}
	
	bool MakeButton(float left, float top, string name){
		return GUI.Button(new Rect(left,top+50, 150,50), name);
	}
	
	private void RestCamera(){

		Vector3 oldWorldTransformEul = new Vector3(0.0f, worldCamera.transform.eulerAngles.y + mainCamera.transform.localEulerAngles.y, 0.0f);
		Vector3 oldMainEul = new Vector3(mainCamera.transform.localEulerAngles.x, 0.0f, 0.0f);
		worldCamera.transform.rotation = Quaternion.Euler(oldWorldTransformEul);
		mainCamera.transform.localRotation = Quaternion.Euler(oldMainEul);
	}
		
	public void SmoothFollow(Transform target){
		
		//print (target.localPosition);
		wantedRotationAngle = target.eulerAngles.y;
		wantedHeight = target.position.y + height;
		
		currentRotationAngle = worldCamera.transform.eulerAngles.y;
		currentHeight = worldCamera.transform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		Vector3 worldCameraPosition =  target.position;
		worldCameraPosition -= currentRotation * Vector3.forward * 10;	
		
		// Set the height of the camera
		worldCameraPosition = new Vector3 (worldCameraPosition.x, currentHeight, worldCameraPosition.z);
		
		if (Mathf.Abs(worldCamera.transform.position.x - worldCameraPosition.x) < 0.1 &&
		    Mathf.Abs(worldCamera.transform.position.y - worldCameraPosition.y) < 0.1 &&
		    Mathf.Abs(worldCamera.transform.position.z - worldCameraPosition.z) < 0.1
		    ){
		    	
		    	smoothPos = false;
		    	
		    }
		
		if(smoothPos){
		
			worldCamera.transform.position = Vector3.Slerp(worldCamera.transform.position, worldCameraPosition, Time.deltaTime *5.5f);
		}else{
			worldCamera.transform.position = worldCameraPosition;
		}
		
		mainCamera.transform.LookAt(target);
	}
}

