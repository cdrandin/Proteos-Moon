using UnityEngine;
using System.Collections;

public class UnitGUI : MonoBehaviour {


	#region class_variables	
	//Delegate variables
	private delegate void GUIMethod();
	private GameObject [] procite_locations;
	private GUIMethod gui_method;
	private GameObject focusTemp, focusObject, worldCamera, mainCamera;
	private bool init, smoothPos, movement, proteus;
	private float height = 5.0f,heightDamping = 2.0f ,rotationDamping = 3.0f, button_pos = Screen.width - 250;
	private float wantedRotationAngle, wantedHeight, currentRotationAngle, currentHeight;
	private Quaternion currentRotation;
	private float [] shift;
	private Transform from;
	#endregion

	public GameObject focus_object 
	{
		get { return this.focusObject; }
	}

	// Use this for initialization
	void Awake(){
		//set objects to null
		focusObject = null;
		focusTemp = null;
		proteus = false;
		//Set bools to false
		init = false;
		movement = false;
		smoothPos = false;
		
	}
	
	void Start () {
		//Initialize World Camera Object
		worldCamera = GameObject.Find("WorldCamera");
		
		procite_locations = GameObject.FindGameObjectsWithTag("Resource");
		
		shift = new float[2];
		shift[0] = 0.0f;
		shift[1] = 0.0f;
	}
	
	
	// Update is called once per frame
	void Update () {
		if(GM.instance.IsOn)
		{
			focusTemp = GM.instance.CurrentFocus;
			
			if(!init && focusTemp != null){
				focusObject = focusTemp;
				GM.instance.SetUnitControllerActiveOff();
				this.gui_method += UnitsOptions;
				
			}
			if(movement){
				if (proteus != NearProcite()){

					this.gui_method -= UnitsOptions;
					this.gui_method += UnitsOptions;
					proteus = NearProcite();
					
				}
			}
			CheckButtonsPressedToRemoveGUI();
		}
		
	}
	
	void LateUpdate(){
		if(movement && focusObject != null){
			SmoothFollow(focusObject.transform);
			
		}
		
	}
	
	private void CheckButtonsPressedToRemoveGUI(){
	
		if( Input.GetKeyUp(KeyCode.Escape) || WorldCameraModified.AreCameraKeyboardButtonsPressed() ){
		
			RemoveGUI();
			focusObject = null;
			init = false;
			
		}
	}
	
	private void RemoveGUI(){

		this.gui_method -= EndMovement;
		this.gui_method -= MovementButton;
		this.gui_method -= WaitButton;
		this.gui_method -= UnitsOptions;
		
	}
	
	void OnGUI(){
	
		if(this.gui_method != null ){
		
			this.gui_method();
		}
	}

	#region UNIT GUI BUTTONS
	private void UnitsOptions(){
		
		if(MakeButton(button_pos,TopButtonPos (0),"Attack")){
			//TODO: Attack Code
		}
		
		GUI.enabled = proteus;

		if(MakeButton(button_pos, TopButtonPos(1), "Gather"	) ){
			//TODO: Gather code
			GM.instance.AddResourcesToCurrentPlayer(50);
		}
		GUI.enabled = true;
		if(!init){
			init = true;
			if(movement){
				
				this.gui_method += EndMovement;
			}else{
				this.gui_method += MovementButton;
				this.gui_method += WaitButton;
			}
		}
	}
	
	
	private void MovementButton(){
	
		if(MakeButton(button_pos, TopButtonPos(2), "Movement")){

			GM.instance.SetUnitControllerActiveOn(ref focusObject);			
			worldCamera.transform.eulerAngles = Vector3.zero;
			mainCamera = CurrentMainCamera();

			smoothPos = true;
			movement = true;
			this.gui_method -= WaitButton;
			this.gui_method -= MovementButton;
			this.gui_method += EndMovement;
		}
	}
	
	GameObject CurrentMainCamera(){
	
		if( GM.instance.CurrentPlayer == 0 ){			
			return  GameObject.Find ("camera_player1");
		}
		else{
			return  GameObject.Find ("camera_player2");
		}
		
	}
	
	private void WaitButton(){
		
		
		if(MakeButton(button_pos,TopButtonPos(3), "Wait")){
			//Expend units action
			GM.instance.SetUnitControllerActiveOff();
			this.gui_method -= WaitButton;
			this.gui_method -= UnitsOptions;
			this.gui_method -= MovementButton;
			focusObject = null;
			init = false;
			}
			
	}
	
	private void EndMovement(){

		if(MakeButton(button_pos, TopButtonPos(2), "End Movement")){
			GM.instance.SetUnitControllerActiveOff();
			this.gui_method -= EndMovement;
			this.gui_method += MovementButton;
			this.gui_method += WaitButton;
			movement = false;
			RestCamera();
		//		Pop ();
		}
		
	}
	#endregion
	
	
	#region Helper Functions
	
	public bool NearProcite(){
	
		if (procite_locations.Length != 0 ){
			Vector3 offset;
			for(int i = 0; i < procite_locations.Length; ++i){
				offset = procite_locations[i].gameObject.transform.position - focusObject.transform.position;
				if(  offset.sqrMagnitude < 2 * 2) {
					return true;
				}
			
			}
			return false;			
		}else
			return false;
	}
	
	bool MakeButton(float left, float top, string name){
		return GUI.Button(new Rect(left,top+50, 150,50), name);
	}
	
	float TopButtonPos(float button_index){
	
		return button_index * 50;
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
		worldCameraPosition -= currentRotation * target.forward * 10;	
		
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
	
	#endregion
}

