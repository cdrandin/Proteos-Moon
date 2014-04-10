using UnityEngine;
using System.Collections;

public class UnitGUI : MonoBehaviour {


	#region class_variables	
	//Delegate variables
	private delegate void GUIMethod();
	private GameObject [] procite_locations;
	private GUIMethod gui_method;
	private GameObject focusTemp, focusObject, worldCamera, mainCamera;
	private bool isInitialize, smoothPos, isMoving, proteus, isAttacking, isAction;
	private float height = 5.0f, heightDamping = 0.5f , rotationDamping = 0.5f, button_pos = Screen.width - 250;
	private float wantedRotationAngle, wantedHeight, currentRotationAngle, currentHeight;
	private Quaternion currentRotation;
	private float [] shift;
	private Transform from;
	public GUISkin mySkin;
	private Rect informationBox;
	Quaternion newRotation; 
	public int toolbarInt = -1;
	#endregion

	public GameObject focus_object 
	{
		get { return this.focusObject; }
	}
	
	private void ResetFlags(){
		//set objects to null
		focusObject = null;
		focusTemp = null;
		proteus = false;
		isAttacking = false;
		isAction = false;
		//Set bools to false
		isInitialize = false;
		isMoving = false;
		smoothPos = false;
		
	}
	
	// Use this for initialization
	void Awake(){
	
		informationBox = new Rect(0,0,(3 * Screen.width)/ 8, (5 * Screen.height)/ 20) ;
		ResetFlags();

		
	}
	
	void Start () {
		//Initialize World Camera Object
		worldCamera = GameObject.Find("WorldCamera");
		
		procite_locations = GameObject.FindGameObjectsWithTag("Resource");
		
	}
	
	
	// Update is called once per frame
	void Update () {
		if(GM.instance.IsOn)
		{
			focusTemp = GM.instance.CurrentFocus;
			
			if(!isInitialize && focusTemp != null){
				CombatSystem.UpdateWithinRangeDelegate();
				focusObject = focusTemp;
				GM.instance.SetUnitControllerActiveOff();
				this.gui_method += UnitInformationBox;
				this.gui_method += BaseSelectionButtons;
				
			}
			if(isMoving){

				CombatSystem.CallCombatDelegates(focusObject);
				
				if (proteus != NearProcite()){

					proteus = NearProcite();
					
				}
			}
			CheckButtonsPressedToRemoveGUI();
		}
		
	}
	
	void LateUpdate(){
		if(isMoving && focusObject != null){
			SmoothFollow(focusObject.transform);
			
		}
		
	}
	
	private void CheckButtonsPressedToRemoveGUI(){
	
		if( Input.GetKeyUp(KeyCode.Escape) || WorldCameraModified.AreCameraKeyboardButtonsPressed() ){
		
			RemoveGUI();

			ResetFlags();
			
		}
	}
	
	private void RemoveGUI(){

		this.gui_method -= UnitInformationBox;
		this.gui_method -= BaseSelectionButtons;
		this.gui_method -= ActionSelectionButtons;
		this.gui_method -= MovementEndButton;
		
	}
	
	void OnGUI(){
		GUI.skin = mySkin;
		if(this.gui_method != null ){
		
			this.gui_method();
		}
	}

	#region UNIT GUI BUTTONS
/*	private void UnitsOptions(){
		
		if(MakeButton(button_pos,TopButtonPos (0),"Attack")){
			//TODO: Attack Code
			
		}
		
		GUI.enabled = proteus;

		if(MakeButton(button_pos, TopButtonPos(1), "Gather"	) ){
			//TODO: Gather code
			GM.instance.AddResourcesToCurrentPlayer(50);
			focusObject.GetComponent<BaseClass>().unit_status.status = Status.Gathering;
		}
		GUI.enabled = true;
		if(!isInitialize){
			isInitialize = true;
			CombatSystem.UpdateWithinRangeDelegate();
			
			if(isMoving){
				
				this.gui_method += EndMovement;
			}else{
				this.gui_method += MovementButton;
				this.gui_method += WaitButton;
			}
		}
	}
	

	
	private void MovementButton(){
	
		if(MakeButton(button_pos, TopButtonPos(2), "Movement")){
			focusObject.GetComponent<BaseClass>().unit_status.status = Status.Movement;
			
			GM.instance.SetUnitControllerActiveOn(ref focusObject);			
			worldCamera.transform.eulerAngles = Vector3.zero;
			mainCamera = CurrentMainCamera();

			smoothPos = true;
			isMoving = true;
			this.gui_method -= WaitButton;
			this.gui_method -= MovementButton;
			this.gui_method += EndMovement;
		}
	}
	
	*/
	#endregion

	public void UnitInformationBox(){
	
		GUI.BeginGroup(new Rect( (3 * Screen.width)/ 8  ,  (3 * Screen.height)/	4 , (3 * Screen.width)/8, (3*Screen.height)/ 10 ));
		
			GUI.depth = 1	;
			isInitialize = true;
			GUI.Box( informationBox, "");//focusObject.GetComponent<BaseClass>().unit_status.unit_type.ToString() );
				
			string currentHealth = focusObject.GetComponent<BaseClass>().vital.HP.current.ToString();
			string maxHealth = focusObject.GetComponent<BaseClass>().vital.HP.max.ToString();
			
			string healthLabel = "HP\t" + currentHealth + " / " + maxHealth ;
	
			string currentExhaust = focusObject.GetComponent<BaseClass>().vital.Exhaust.current.ToString();
			string maxExhaust = focusObject.GetComponent<BaseClass>().vital.Exhaust.max.ToString();
			
			string exhaustLabel = "Exhaust\t" + currentExhaust + " / " + maxExhaust;
							
			GUI.Label( new Rect( informationBox.width / 2 - 60, informationBox.height/2 - 40, 100, 30) , healthLabel );
			GUI.Label( new Rect( informationBox.width / 2 - 60, informationBox.height/2 - 10, 200, 30) , exhaustLabel );
		
		GUI.EndGroup();
	
	}
	
	
	public void BaseSelectionButtons(){
	
		GUI.BeginGroup(new Rect( (3 * Screen.width)/ 4  ,  (3 * Screen.height)/ 4 , (3 * Screen.width)/8, (3*Screen.height)/ 10 ));
		
			GUI.depth = 1;
			GUI.enabled = !isAction;
			if(GUI.Button(new Rect(0,0, (1 * Screen.width)/ 8, Screen.height/ 16) , "Move")){
				focusObject.GetComponent<BaseClass>().unit_status.status = Status.Movement;
				
				GM.instance.SetUnitControllerActiveOn(ref focusObject);			
				worldCamera.transform.eulerAngles = Vector3.zero;
				mainCamera = CurrentMainCamera();

				gui_method += MovementEndButton;
				smoothPos = true;
				isMoving = true;
				isAction = true;
			}
			if(GUI.Button(new Rect(0, Screen.height/ 16, Screen.width/ 8, Screen.height/ 16) , "Action")){
				isAction = true;
				gui_method += ActionSelectionButtons;
				
			}
			
			GUI.enabled = proteus && !isAction;	
			if(GUI.Button(new Rect(0, Screen.height/ 8, Screen.width/ 8, Screen.height/ 16) , "Gather")){
				//TODO: Gather code
				GM.instance.AddResourcesToCurrentPlayer(50);
				focusObject.GetComponent<BaseClass>().unit_status.status = Status.Gathering;
				
			}
			GUI.enabled = !isAction;
			if(GUI.Button(new Rect(0, (3 * Screen.height)/ 16, Screen.width/ 8, Screen.height/ 16) , "Rest")){
				focusObject.GetComponent<BaseClass>().unit_status.status = Status.Resting;
				GM.instance.SetUnitControllerActiveOff();
				this.gui_method -= UnitInformationBox;
				this.gui_method -= BaseSelectionButtons;
				this.gui_method -= ActionSelectionButtons;
				focusObject = null;
				isInitialize = false;
			}
			GUI.enabled = true;
		GUI.EndGroup();
		
	}
	
	public void MovementEndButton(){
	
		GUI.BeginGroup(new Rect( (25 * Screen.width)/ 32  ,  (29 * Screen.height)/ 40 , (3 * Screen.width)/8, (3 * Screen.height)/ 10 ));
		
			GUI.depth = 2;
			if(GUI.Button(new Rect(0,0,  Screen.width/ 8, Screen.height/ 16) , "End Movement")){
				GM.instance.SetUnitControllerActiveOff();
				isMoving = false;
				smoothPos = true;
				ResetCamera();
				gui_method -= MovementEndButton;
				isAction  = false;
			}
		GUI.EndGroup();
	}
	
	public void ActionSelectionButtons(){
	
		GUI.BeginGroup(new Rect( (25 * Screen.width)/ 32  ,  (29 * Screen.height)/ 40 , (3 * Screen.width)/8, (3*Screen.height)/ 10 ));

			GUI.depth = 2;
			if(GUI.Button(new Rect(0,0, Screen.width/ 8, Screen.height/ 16) , "Attack")){
				//Expend units action
				
				gui_method -= ActionSelectionButtons;
				isAction  = false;
			}
			if(GUI.Button(new Rect(0, Screen.height/ 15, Screen.width/ 8, Screen.height/ 16) , "Use")){
				
				gui_method -= ActionSelectionButtons;
				isAction = false;
			}
			if(GUI.Button(new Rect(0, (2 * Screen.height)/ 15, Screen.width/ 8, Screen.height/ 16) , "Special")){
				
				gui_method -= ActionSelectionButtons;
				isAction  =false;
			}
			if(GUI.Button(new Rect(0, (3 * Screen.height)/ 15, Screen.width/ 8, Screen.height/ 16) , "Back")){
				
				gui_method -= ActionSelectionButtons;
				isAction  = false;
			}
		GUI.EndGroup();
	}
	
	
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
	
	//TODO: Make this function work to hit something
	private bool Attackable(GameObject enemyObject){
		
		return Vector3.SqrMagnitude(focusObject.transform.position - enemyObject.transform.position )
			< focusObject.GetComponent<BaseClass>().attack_range * focusObject.GetComponent<BaseClass>().attack_range;
		
	}
	bool MakeButton(float left, float top, string name){
		return GUI.Button(new Rect(left,top+0, 150,50), name);
	}
	
	float TopButtonPos(float button_index){
	
		return button_index * 50;
	}
		
	private void ResetCamera(){

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
		
		if (smoothPos && 
			(Mathf.Abs(worldCamera.transform.position.x - worldCameraPosition.x) < 0.1 &&
		    Mathf.Abs(worldCamera.transform.position.y - worldCameraPosition.y) < 0.1 &&
		    Mathf.Abs(worldCamera.transform.position.z - worldCameraPosition.z) < 0.1
		    )){
				
		    	smoothPos = false;
		    }
		
		if(smoothPos){

			worldCamera.transform.position = Vector3.Slerp(worldCamera.transform.position, worldCameraPosition, Time.deltaTime *5.5f);
			
		}
		if (!smoothPos && Input.anyKey){
			//print("in here");
			worldCamera.transform.position = worldCameraPosition;
		//	mainCamera.transform.LookAt(target);
			
		}
		mainCamera.transform.LookAt(target);
		
		//var rotation = Quaternion.LookRotation(target.position - worldCamera.transform.position);
		//mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, rotation, Time.deltaTime * 5.5);
	}
	GameObject CurrentMainCamera(){
		
		return  GameObject.Find ("camera_player" + ((int)GM.instance.CurrentPlayer + 1 ).ToString() );
		
	}
	#endregion
}

