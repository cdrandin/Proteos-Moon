using UnityEngine;
using System.Collections;

public class UnitGUI : MonoBehaviour {


	#region class_variables	
	//Delegate variables
	private delegate void GUIMethod();
	private GameObject [] procite_locations;
	private GUIMethod gui_method;
	private GameObject focusTemp, focusObject;
	private bool isInitialize, smoothPos, isMoving, proteus, isAttacking, isAction;
	private float height = 5.0f, heightDamping = 0.5f , rotationDamping = 0.5f, button_pos = Screen.width - 250;
	private float wantedRotationAngle, wantedHeight, currentRotationAngle, currentHeight;
	private Quaternion currentRotation;
	private float shift;
	private Transform from;
	public GUISkin mySkin;
	private Rect informationBox;
	Quaternion newRotation; 
	public int toolbarInt = -1;

	private RecruitSystem _rs;

	//	public static UnitGUI instance;
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
		shift = 0;
		if(WorldCamera.instance != null)
			WorldCamera.instance.TurnCameraControlsOn();
		
	}
	
	// Use this for initialization
	void Awake(){
	
		informationBox = new Rect(0,0,(3 * Screen.width)/ 8, (5 * Screen.height)/ 20) ;
		ResetFlags();
		shift = 0;
		
	}
	
	void Start () {
		//Initialize World Camera Object
		//worldCamera = GameObject.Find("WorldCamera");
		
		procite_locations = GameObject.FindGameObjectsWithTag("Resource");
		_rs               = GameObject.FindObjectOfType<RecruitSystem>();
	}
	
	
	// Update is called once per frame
	void Update () {
		if(GM.instance.IsOn)
		{
			focusTemp = GM.instance.CurrentFocus;
			
			if(!isInitialize && focusTemp != null /*&& focusTemp.GetComponent<BaseClass>().unit_status.status != Status.Rest*/){
				CombatSystem.instance.UpdateWithinRangeDelegate();
				focusObject = focusTemp;
				GM.instance.SetUnitControllerActiveOff();
				if(focus_object.GetComponent<BaseClass>().unit_status.unit_type == UnitType.Leader)		
					shift = Screen.height/ 16;
				this.gui_method += UnitInformationBox;
				this.gui_method += BaseSelectionButtons;
				
				
			}
			if(isMoving){

				CombatSystem.instance.CallCombatDelegates(focusObject);
				
				if (proteus != NearProcite()){

					proteus = NearProcite();
					
				}
			}
			
			if(CombatSystem.instance.CheckIfAttacking() ){

				CombatSystem.instance.CheckIfChangingTarget();
				CombatSystem.instance.Attack(focusObject);
			}
			CheckButtonsPressedToRemoveGUI();
		}
		
	}
	
	void LateUpdate(){
		if(isMoving && focusObject != null){
			SmoothFollow(focusObject.transform);
			
		}
		if(CombatSystem.instance.CheckIfAttacking()){
			
			CombatSystem.instance.CombatLookAt(focusObject);;
			
		}
		
	}
	
	private void CheckButtonsPressedToRemoveGUI(){
	
		if( WorldCamera.instance.IsCameraOnControlsOn()  && (Input.GetKeyUp(KeyCode.Escape) || WorldCamera.AreCameraKeyboardButtonsPressed()) ){
			
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
			mySkin.box.fontSize = mySkin.box.fontSize = Screen.height / 32;
			//GUI.enabled = !isAction && (GetCurrentFocusStatus() == ((Status.Clean | Status.Move) | GetCurrentFocusStatus()));// &&  (focusObject.GetComponent<BaseClass>().unit_status.status == Status.Gather) ;
			GUI.enabled = !isAction && (GetCurrentFocusStatus().CompareTo(Status.Clean | Status.Move) < 0);// &&  (focusObject.GetComponent<BaseClass>().unit_status.status == Status.Gather) ;
			if(GUI.Button(new Rect(0,0, (1 * Screen.width)/ 8, Screen.height/ 16) , "Move")){
				
				UpdateFocusObjectsStatus(Status.Move);
				GM.instance.SetUnitControllerActiveOn(ref focusObject);	
				Debug.Log(string.Format("Unit Controller: {0}", GM.instance.CurrentFocus));
				GM.instance.SetFocusController(true);

				WorldCamera.instance.transform.eulerAngles = Vector3.zero;
				WorldCamera.instance.MainCamera = CurrentMainCamera();
				WorldCamera.instance.TurnCameraControlsOff();

				gui_method += MovementEndButton;
				smoothPos = true;
				isMoving = true;
				isAction = true;
			}
			GUI.enabled = !isAction && (GetCurrentFocusStatus().CompareTo(Status.Clean | Status.Action) < 0);
			if(GUI.Button(new Rect(0, Screen.height/ 16, Screen.width/ 8, Screen.height/ 16) , "Action")){
				isAction = true;
				gui_method += ActionSelectionButtons;
				
			}
			
			GUI.enabled = proteus && !isAction && (GetCurrentFocusStatus().CompareTo(Status.Clean | Status.Gather) < 0);	
			if(GUI.Button(new Rect(0, Screen.height/ 8, Screen.width/ 8, Screen.height/ 16) , "Gather")){
				//TODO: Gather code
				GM.instance.AddResourcesToCurrentPlayer(50);
				UpdateFocusObjectsStatus(Status.Gather);
				
			}
			GUI.enabled = !isAction;
			if(GUI.Button(new Rect(0, (3 * Screen.height)/ 16, Screen.width/ 8, Screen.height/ 16) , "Rest")){
				focus_object.GetComponent<BaseClass>().unit_status.status = Status.Rest;
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
				//GM.instance.SetFocusController(false);
			
				isMoving = false;
				smoothPos = true;
				WorldCamera.instance.ResetCamera();
				WorldCamera.instance.TurnCameraControlsOn();
				gui_method -= MovementEndButton;
				isAction  = false;
			}
		GUI.EndGroup();
	}
	
	public void ActionSelectionButtons(){
	
		GUI.BeginGroup(new Rect( (25 * Screen.width)/ 32  ,  ((29 * Screen.height)/ 40) - shift , (3 * Screen.width)/8, ((3*Screen.height) / 10)+ shift ) );
		
			
		GUI.enabled = !CombatSystem.instance.CheckIfAttacking() && CombatSystem.instance.AnyNearbyUnitsToAttack(focusObject)  && (GetCurrentFocusStatus().CompareTo(Status.Clean | Status.Action) < 0);
			GUI.depth = 2;
			if(GUI.Button(new Rect(0,0, Screen.width/ 8, Screen.height/ 16) , "Attack")){
				//Expend units action
//				CombatSystem.instance.GetNearbyAttackableUnits(focusObject);
			
				UpdateFocusObjectsStatus(Status.Action);
				CombatSystem.instance.AttackButtonClicked();
				//isAction  = false;
				
			}
			if(GUI.Button(new Rect(0, Screen.height/ 15, Screen.width/ 8, Screen.height/ 16) , "Use")){
			
				UpdateFocusObjectsStatus(Status.Action);
				gui_method -= ActionSelectionButtons;
				isAction = false;
			}
			if(GUI.Button(new Rect(0, (2 * Screen.height)/ 15, Screen.width/ 8, Screen.height/ 16) , "Special")){
			
				UpdateFocusObjectsStatus(Status.Action);
				gui_method -= ActionSelectionButtons;
				isAction  =false;
			}
			
			GUI.enabled = true;
			GUI.depth = 2;
			
			if(shift > 0){
				if(GUI.Button(new Rect(0, (3 * Screen.height)/ 15, Screen.width/ 8, Screen.height/ 16) , "Recruit")){
					
					gui_method -= ActionSelectionButtons;
					gui_method -= BaseSelectionButtons;
					gui_method += RecruitMenuButtons;
					isAction = false;
				}
				
			}
			
			if(GUI.Button(new Rect(0, ((3 * Screen.height)/ 15) + shift, Screen.width/ 8, Screen.height/ 16) , "Back")){
				
				if(CombatSystem.instance.CheckIfAttacking()){
				
					CombatSystem.instance.ResetCombatSystem();
				}	
				else{
					gui_method -= ActionSelectionButtons;
					isAction  = false;
				}
			}
		GUI.EndGroup();
	}
	
	public void RecruitMenuButtons(){
	
		GUI.BeginGroup(new Rect( (24 * Screen.width)/ 32  ,  (10 * Screen.height)/ 40 , (2 * Screen.width)/8, 3* Screen.height/ 4 ));
			mySkin.box.fontSize = Screen.height / 32;
			GUI.Box (  new Rect (0,0,(2 * Screen.width)/8, 3*Screen.height/ 4), "Recruit Menu"  );
//			GUI.enabled = 
		if (GUI.Button (new Rect ((1 * Screen.width)/64, (95*Screen.height)/1024 ,(7 * Screen.width)/32, (95*Screen.height)/1024), string.Format("Scout  {0}", _rs.unit_cost.scout))){

				UpdateFocusObjectsStatus(Status.Action);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Scout);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
			}
		if (GUI.Button (new Rect ((1 * Screen.width)/64, (2*95*Screen.height) /1024,(7 * Screen.width)/32, (95*Screen.height)/1024), string.Format("Braver  {0}", _rs.unit_cost.braver))){
			
				UpdateFocusObjectsStatus(Status.Action);	
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Braver);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				
			}
		if (GUI.Button (new Rect ((1 * Screen.width)/64, (3*95*Screen.height) /1024,(7 * Screen.width)/32, (95*Screen.height)/1024), string.Format("Arcane  {0}", _rs.unit_cost.arcane))){
			
				UpdateFocusObjectsStatus(Status.Action);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Arcane);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				
			}
		if (GUI.Button (new Rect ((1 * Screen.width)/64, (4*95*Screen.height) /1024,(7 * Screen.width)/32, (95*Screen.height)/1024), string.Format("Sniper  {0}", _rs.unit_cost.sniper))){
			
				UpdateFocusObjectsStatus(Status.Action);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Sniper);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				
			}
		if (GUI.Button (new Rect ((1 * Screen.width)/64, (5*95*Screen.height) /1024,(7 * Screen.width)/32, (95*Screen.height)/1024), string.Format("Titan  {0}", _rs.unit_cost.titan))){
			
				UpdateFocusObjectsStatus(Status.Action);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Titan);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				
			}
		if (GUI.Button (new Rect ((1 * Screen.width)/64, (6*95*Screen.height) /1024,(7 * Screen.width)/32, (95*Screen.height)/1024), string.Format("Vangaurd  {0}", _rs.unit_cost.vangaurd))){
			
				UpdateFocusObjectsStatus(Status.Action);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Vangaurd);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				
			}
		if (GUI.Button (new Rect ((1 * Screen.width)/16, (660*Screen.height) /1024,(1 * Screen.width)/8, (95*Screen.height)/1024), "Back")){
				
				isAction = true;
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				gui_method += ActionSelectionButtons;
			}
		
		GUI.EndGroup();
	}
	#endregion
	
	
	#region Helper Functions
	
	private Status GetCurrentFocusStatus(){
	
		return focusObject.GetComponent<BaseClass>().unit_status.status;
	}
	private void UpdateFocusObjectsStatus(Status newStatus){
	
		Status oldStatus = focusObject.GetComponent<BaseClass>().unit_status.status;
		focusObject.GetComponent<BaseClass>().unit_status.status = (oldStatus | newStatus);
	}
	
	public bool NearProcite(){
	
		if (procite_locations.Length != 0 ){
			Vector3 offset;
			for(int i = 0; i < procite_locations.Length; ++i){
				offset = procite_locations[i].gameObject.transform.position - focusObject.transform.position;
				float range = focusObject.GetComponent<BaseClass>().gather_range;
				range = range * range;
				if( offset.sqrMagnitude < range) {
					return true;
				}
			
			}
			return false;			
		}else
			return false;
	}
	

	bool MakeButton(float left, float top, string name){
		return GUI.Button(new Rect(left,top+0, 150,50), name);
	}
		
	public void SmoothFollow(Transform target){
		
		//print (target.localPosition);
		wantedRotationAngle = target.eulerAngles.y;
		wantedHeight = target.position.y + height;
		
		currentRotationAngle = WorldCamera.instance.transform.eulerAngles.y;
		currentHeight = WorldCamera.instance.transform.position.y;
		
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
		    (Mathf.Abs(WorldCamera.instance.transform.position.x - worldCameraPosition.x) < 0.1 &&
			 Mathf.Abs(WorldCamera.instance.transform.position.y - worldCameraPosition.y) < 0.1 &&
			 Mathf.Abs(WorldCamera.instance.transform.position.z - worldCameraPosition.z) < 0.1
		    )){
				
		    	smoothPos = false;
		    }
		
		if(smoothPos){

			WorldCamera.instance.transform.position = Vector3.Slerp(WorldCamera.instance.transform.position, worldCameraPosition, Time.deltaTime *5.5f);
			
		}
		if (!smoothPos && Input.anyKey){
			//print("in here");
			WorldCamera.instance.transform.position = worldCameraPosition;
		//	mainCamera.transform.LookAt(target);
			
		}
		WorldCamera.instance.MainCamera.transform.LookAt(target);
		
		//var rotation = Quaternion.LookRotation(target.position - worldCamera.transform.position);
		//mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, rotation, Time.deltaTime * 5.5);
	}
	GameObject CurrentMainCamera(){
		
		return  GameObject.Find ("camera_player" + ((int)GM.instance.CurrentPlayer + 1 ).ToString() );
		
	}
	#endregion

	void OnDisable()
	{
		this.enabled = true;
	}
}

