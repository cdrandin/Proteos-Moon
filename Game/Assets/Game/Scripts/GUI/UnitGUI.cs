using UnityEngine;
using System.Collections;

public class UnitGUI : MonoBehaviour {


	#region class_variables	
	private delegate void GUIMethod();
	private GUIMethod gui_method;
	private UnitCost _unit_cost;
	GameObject focusTemp, focusObject, worldCamera, mainCamera;
	private bool init;
	private float height = 5.0f;
	private float heightDamping = 2.0f;
	private float rotationDamping = 3.0f;
	
	private float wantedRotationAngle;
	private float wantedHeight;
	private float currentRotationAngle;
	private float currentHeight;
	private Quaternion currentRotation;
	
	private Vector3 oldWorldTransformPos, oldWorldTransformEul, oldMainPos, oldMainEul;
	private bool movement;
	#endregion
	
	// Use this for initialization
	void Awake(){
	
		oldWorldTransformPos = Vector3.zero;
		oldWorldTransformEul = Vector3.zero;
		oldMainPos = Vector3.zero;
		oldMainEul = Vector3.zero;
	}
	void Start () {
		worldCamera = GameObject.Find("WorldCamera");
		focusObject = null;
		focusTemp = null;
		init = false;
		movement = false;
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
		if(movement){
			print ("Focus Object:" + focusObject);
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
		if(MakeButton(200,0,"Attack")){
			
			
		}
		else if(MakeButton(200, 50, "Movement")){
			if( GameManager.GetCurrentPlayer() == 0 ){			
				mainCamera = GameObject.Find ("camera_player1");
				}
			else{
				mainCamera = GameObject.Find ("camera_player2");
			}
			
			GameManager.SetUnitControllerActiveOn(ref focusObject);
			this.gui_method -= UnitsOptions;
			this.gui_method += MovementActive;
			
			Push();
			worldCamera.transform.eulerAngles = Vector3.zero;
		}
		else if(MakeButton(200, 100, "Wait")){
			//Expend units action
			GameManager.SetUnitControllerActiveOff();
			this.gui_method -= UnitsOptions;
			focusObject = null;
			init = false;
		}
	}
	
	void MovementActive(){
		movement = true;
		if(MakeButton(200, 0, "End Movement")){
			GameManager.SetUnitControllerActiveOff();
			this.gui_method -= MovementActive;
			this.gui_method += UnitsOptions;
			movement = false;
			Pop ();
		}
		
	}
	
	bool MakeButton(float left, float top, string name){
		return GUI.Button(new Rect(left,top+50, 150,50), name);
	}
	
	public void Push(){

		oldWorldTransformPos = worldCamera.transform.position;
		oldWorldTransformEul = worldCamera.transform.eulerAngles;
		oldMainPos = mainCamera.transform.localPosition;
		oldMainEul = mainCamera.transform.localEulerAngles;
	}
	
	public void Pop(){
		
		worldCamera.transform.position = oldWorldTransformPos;
		worldCamera.transform.eulerAngles = oldWorldTransformEul;
		mainCamera.transform.localPosition = oldMainPos;
		mainCamera.transform.localEulerAngles = oldMainEul;
	}
	
	public void SmoothFollow(Transform target){
		
		print (target.position);
		//print (target.localPosition);
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
		worldCamera.transform.position = target.position;
		worldCamera.transform.position -= currentRotation * Vector3.forward * 10;
		print (worldCamera.transform.position);
	
		
		// Set the height of the camera
		worldCamera.transform.position = new Vector3 (worldCamera.transform.position.x, currentHeight, worldCamera.transform.position.z);
		
		mainCamera.transform.LookAt(target.position);
	}
}

