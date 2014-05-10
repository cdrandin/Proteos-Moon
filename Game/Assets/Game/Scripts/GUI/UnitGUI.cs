using UnityEngine;
using System.Collections;


//How to get screen ratios
//If you want to move it change the ratio values
//Example (25 * Screen.width)/ 32  to (51 * Screen.width) /64
//NOTE (30/32)*Screen.width does not work

//Find the ratio of the GUI, 60 height by 40 width, then the ratio is  3 by 2
//Create a variable height = Screen.height/64
//Then the width = 2 * height / 3

public class UnitGUI : MonoBehaviour {


	#region class_variables	
	//Delegate variables
	private delegate void GUIMethod();
	private GameObject [] procite_locations;
	private GUIMethod gui_method;
	private GameObject focusTemp, focusObject;
	private bool isInitialize, smoothPos, isMoving, proteus, isAction,isRecruiting;
	public float lookAtHeight = 1.0f, DistancefromPlayer = 3.0f;
	private float heightDamping = 2.0f , rotationDamping = 3.0f;//, button_pos = Screen.width - 250;
	private float wantedRotationAngle, wantedHeight, currentRotationAngle, currentHeight;
	public float distanceScale;
	private Quaternion currentRotation;
	private float shift;
	private Transform from;
	public GUISkin mySkin;
	private Rect informationBox, UnitInfoLocation;
	Quaternion newRotation; 
	public int toolbarInt = -1;

	private Vector3 current_procite_pos;
	public Quaternion look;

	//GameObject unit_character_controller;
	
	
	public GameObject Portraits, Bars, Icons;
	
	public Texture2D highlight, clicked;
	
	
	public static UnitGUI instance;
	
	private RecruitSystem _rs;
	private bool _reset_once;

	private enum Style
	{
	
		action, move, gather, summon, rest, move_cancel, item,
		special, attack, arcane, braver, back, gigan, sniper, 
		vanguard, scout, blue_box
	 
	 }
	//	public static UnitGUI instance;
	#endregion

	public static Rect UnitBoxLocation(){
	
		return UnitGUI.instance.UnitInfoLocation;
	
	}
	public static GUISkin UnitGUISkin(){
	
		return UnitGUI.instance.mySkin;
	}
	
	

	public GameObject focus_object 
	{
		get 
		{ 
			return (this.focusObject == null)?null:this.focusObject; 
		}
	}
	
	//Updates the flag when you want to reset the GUI functionality
	private void ResetFlags(){
		//set objects to null
		isRecruiting = false;
		focusObject = null;
		focusTemp = null;
		proteus = false;
		//isAttacking = false;
		isAction = false;
		//Set bools to false
		isMoving = false;
		smoothPos = false;
		_reset_once = false;

		shift = 0;
		if(WorldCamera.instance != null)
			WorldCamera.instance.TurnCameraControlsOn();
	}
	
	
	//This will Update the GUI Skin Layout, this will allow the hightlight feature and clicked feature
	public void UpdateSkinLayout(){
		
		Texture2D normal = mySkin.button.normal.background;
		
		mySkin.button.active.background = CombineTextures(normal, clicked);
		mySkin.button.hover.background = CombineTextures(normal, highlight);
		
		mySkin.button.onActive.background = CombineTextures(normal, clicked);
		mySkin.button.onHover.background = CombineTextures(normal, highlight);
		
		//This will only work for the given custom styles, any extra need to add after the loop
		for(int i = 0; i < (int)Style.blue_box ; ++i){
		
			normal = mySkin.customStyles[i].normal.background;
			
			mySkin.customStyles[i].active.background = CombineTextures(normal, clicked);
			mySkin.customStyles[i].hover.background = CombineTextures(normal, highlight);

			mySkin.customStyles[i].onActive.background = CombineTextures(normal, clicked);
			mySkin.customStyles[i].onHover.background = CombineTextures(normal, highlight);
			
		}
	
	}
	
	// Use this for initialization
	void Awake(){
		instance = this;
		DistancefromPlayer = 3.5f;
		float pheight = (5*Screen.height/ 24);
		UnitInfoLocation = new Rect( Screen.width - (5* pheight / 2) , 0 , 5* pheight / 2 , pheight );
		informationBox = new Rect(0,0, UnitInfoLocation.width , UnitInfoLocation.height) ;
		ResetFlags();
		shift = 0;
		UpdateSkinLayout();
		lookAtHeight = 5;
		
		distanceScale = 1.0f;
		
	}
	
	void Start () {
		//Initialize World Camera Object
		//worldCamera = GameObject.Find("WorldCamera");
		
		procite_locations = GameObject.FindGameObjectsWithTag("Resource");
		_rs               = GameObject.FindObjectOfType<RecruitSystem>();
		
		//unit_character_controller = GameObject.FindWithTag("UnitController");
		
	}
	
	
	// Update is called once per frame
	void Update () {
		if(GM.instance.IsOn)
		{
			if(GM.instance.IsItMyTurn())
			{
				if(!_reset_once)
				{
					ResetFlags();
					RemoveGUI();
					_reset_once = true;
				}
			}
			focusTemp = GM.instance.CurrentFocus;
			
			//Conditions to print the GUI		
			if( (!isInitialize && focusTemp != null &&  !(focusTemp.GetComponent<BaseClass>().unit_status.status.Rest) ) ){
				
				CombatSystem.instance.UpdateWithinRangeDelegate();
				focusObject = focusTemp;
				GM.instance.SetUnitControllerActiveOff();
				
				
				
				this.gui_method += UnitInformationBox;
				
				
				if (GM.instance.IsItMyTurn() && focusObject.GetPhotonView().isMine ){
				
					focusObject.GetComponentInChildren<AnimationTriggers>().ReadyAnimation();
					this.gui_method += BaseSelectionButtons;
				}
					
			}
			if(isMoving){
				if(focusObject == null){
					Debug.LogError("The unit focus is missing");
				}
				
				focusObject.GetComponentInChildren<AnimationTriggers>().MoveAnimation( Input.GetAxis("Vertical") );
				
				CombatSystem.instance.CallCombatDelegates(focusObject);
				
				if (proteus != NearProcite()){
					proteus = NearProcite();
				}
			}
			
			if(CombatSystem.instance.CheckIfAttacking() ){

				CombatSystem.instance.CheckIfChangingTarget();
				StartCoroutine(CombatSystem.instance.Attack(focusObject));
			}
			CheckButtonsPressedToRemoveGUI();
		}
		
	}
	
	void LateUpdate(){
		if(isMoving && focusObject != null){
			SmoothFollow(focusObject);
			
		}
		if(focusObject != null && CombatSystem.instance.CheckIfAttacking()){
			
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
		
		isInitialize = false;
		this.gui_method -= UnitInformationBox;
		this.gui_method -= BaseSelectionButtons;
		this.gui_method -= ActionSelectionButtons;
		this.gui_method -= MovementEndButton;
		GM.instance.SetUnitControllerActiveOff();
	}
	
	void OnGUI(){
		GUI.skin = mySkin;
		if(this.gui_method != null ){
		
			this.gui_method();
		}
	}

	#region UNIT GUI BUTTONS

	public void UnitInformationBox(){
	
		GUI.BeginGroup(UnitInfoLocation);
		
			GUI.depth = 1	;
			isInitialize = true;
			GUI.Box( informationBox, "");
			CharacterPortrait(informationBox, focusObject, GM.instance.CurrentPlayer);
			HealthExhaustBars(informationBox, focusObject);

		
		GUI.EndGroup();
	
	}
	
	//This is the method that will create new the health bar and the exhaust bar
	//info_box is the GUI box location
	public static void HealthExhaustBars(Rect info_box, GameObject char_stats){
	
		float currentHealth = char_stats.GetComponent<BaseClass>().vital.HP.current;
		float maxHealth = char_stats.GetComponent<BaseClass>().vital.HP.max;
		
		string healthLabel = "HP";// + currentHealth.ToString() + " / " + maxHealth.ToString() ;
		
		float currentExhaust = char_stats.GetComponent<BaseClass>().vital.Exhaust.current;
		float maxExhaust = char_stats.GetComponent<BaseClass>().vital.Exhaust.max;
		
		string exhaustLabel = "Exhaust";// + currentExhaust.ToString() + " / " + maxExhaust.ToString();
						
		//(3*info_box.width / 5 + 16) * 1.1f
		Rect label_pos = new Rect( ( ( 4* ( info_box.height - 32 ) / 6) * 1.1f) + 16 , 
									16 + (5*info_box.width)/ 64  , 
									3*info_box.width / 5 , 
									(info_box.height - 40 -  (5*info_box.width)/ 64 )/ 4);
		Rect texture_pos = label_pos;
		texture_pos.y += texture_pos.height;
	
		
		UnitGUI.instance.mySkin.label.fontSize = (int)( texture_pos.height ) - (int)( UnitGUI.instance.mySkin.label.padding.bottom )- (int)( UnitGUI.instance.mySkin.label.padding.top ) - 2;
		UnitGUI.instance.mySkin.label.alignment = TextAnchor.LowerLeft;
		
				
		GUI.Label( label_pos , healthLabel );
		
		GUI.DrawTexture( texture_pos, UnitGUI.instance.Bars.transform.Find("Empty").guiTexture.texture );
		GUI.DrawTexture( new Rect(texture_pos.x,texture_pos.y, (currentHealth * texture_pos.width) / maxHealth, texture_pos.height ), UnitGUI.instance.Bars.transform.Find("Health").guiTexture.texture);
		texture_pos.y += 2f*texture_pos.height;
		label_pos.y += 2f*texture_pos.height;

		GUI.Label( label_pos , exhaustLabel );
		
		GUI.DrawTexture( texture_pos, UnitGUI.instance.Bars.transform.Find("Empty").guiTexture.texture ) ;
		GUI.DrawTexture( new Rect(texture_pos.x, texture_pos.y , (currentExhaust * texture_pos.width) / maxExhaust, texture_pos.height ), UnitGUI.instance.Bars.transform.Find("Exhaust").guiTexture.texture);
		
	}
	
	
	//This will draw the appropiate picture for each unit
	public static void CharacterPortrait(Rect info_box, GameObject char_portrait, Player player){
		GUIStyle style;
		GUITexture portrait_texture = null;
		GUITexture icon_texture = null;
		string char_name = "";
		
		if(char_portrait.GetPhotonView().isMine ){
		
		
			style = UnitGUI.UnitGUISkin().FindStyle("blue_box");
		}
		else{
		
			style = UnitGUI.UnitGUISkin().FindStyle("red_box");
		}
		
		switch(char_portrait.GetComponent<BaseClass>().unit_status.unit_type){
		
		case UnitType.Arcane:
			portrait_texture = UnitGUI.instance.Portraits.transform.Find("Arcane").gameObject.guiTexture;
			icon_texture = UnitGUI.instance.Icons.transform.Find("Arcane").gameObject.guiTexture;
			char_name = "Unit: Arcane";
			break;		
		case UnitType.Braver:
			portrait_texture = UnitGUI.instance.Portraits.transform.Find("Braver").gameObject.guiTexture;
			icon_texture = UnitGUI.instance.Icons.transform.Find("Braver").gameObject.guiTexture;
			char_name = "Unit: Braver";
			break;
		case UnitType.Scout:
			portrait_texture = UnitGUI.instance.Portraits.transform.Find("Scout").gameObject.guiTexture;
			icon_texture = UnitGUI.instance.Icons.transform.Find("Scout").gameObject.guiTexture;
			char_name = "Unit: Scout";
			break;		
		case UnitType.Titan:
			portrait_texture = UnitGUI.instance.Portraits.transform.Find("Gigan").gameObject.guiTexture;
			icon_texture = UnitGUI.instance.Icons.transform.Find("Gigan").gameObject.guiTexture;
			char_name = "Unit: Gigan";
			break;
		case UnitType.Sniper:
			portrait_texture = UnitGUI.instance.Portraits.transform.Find("Sniper").gameObject.guiTexture;
			icon_texture = UnitGUI.instance.Icons.transform.Find("Sniper").gameObject.guiTexture;
			char_name = "Unit: Sniper";
			break;		
		case UnitType.Vangaurd:
			portrait_texture = UnitGUI.instance.Portraits.transform.Find("Vanguard").gameObject.guiTexture;
			icon_texture = UnitGUI.instance.Icons.transform.Find("Vanguard").gameObject.guiTexture;
			char_name = "Unit: Vanguard";
			break;
		
		case UnitType.Leader:
			
			if(char_portrait.GetComponent<BaseClass>().unit_status.leader == Leader.Altier_Seita){
			
				portrait_texture = UnitGUI.instance.Portraits.transform.Find("Seita").gameObject.guiTexture;
				icon_texture = UnitGUI.instance.Icons.transform.Find("Braver").gameObject.guiTexture;
				char_name = "Leader: Seita";
				
			}
			else if (char_portrait.GetComponent<BaseClass>().unit_status.leader == Leader.Captain_Mena){
			
				portrait_texture = UnitGUI.instance.Portraits.transform.Find("Mena").gameObject.guiTexture;
				icon_texture = UnitGUI.instance.Icons.transform.Find("Sniper").gameObject.guiTexture;
				char_name = "Leader: Mena";
				
			}
			else{
			
				Debug.LogError( "Unit Status Leader is incorrect in BaseClass");
			}
			
			break;
			
		default:
		
			Debug.LogError( "Unit Status is incorrect in BaseClass");
			break;
		}
		//32 is because the is always 16 pixels
		float pheight  = info_box.height - 32;
		//Fix the position based off of the box because the offset is 16 by 16
		Rect box_pos = new Rect(16,16, 4*pheight / 6, pheight );
		
		
		// Since the box height width ratio is 4 over 6, we can find the left offset
		float pLeftOverWidth = info_box.width - (4*pheight / 6) - 32;
		float icon_left_offset =  (pLeftOverWidth / 2) -  ( pheight / 2) + 16 + box_pos.width ;		

		//icon_texture.renderer.material.color = Color.blue	;	
		//Renders Icon in the screen
		GUI.Box( box_pos, portrait_texture.guiTexture.texture, style );
		GUI.DrawTexture( new Rect(icon_left_offset, 16, pheight, pheight) , icon_texture.texture);
		
		
		box_pos.x += 1.1f*box_pos.width;
		box_pos.y += 0.05f*info_box.height;
		box_pos.width = 3*info_box.width / 5;
		box_pos.height = (5*info_box.width)/ 64 ;
		
		//mySkin.label.alignment = TextAnchor.LowerCenter;

		UnitGUI.instance.mySkin.label.fontSize = (int)( box_pos.height) - (int)UnitGUI.instance.mySkin.label.padding.bottom - (int)UnitGUI.instance.mySkin.label.padding.top;
		//print (box_pos);
		//print ((int)( box_pos.height));
		UnitGUI.instance.mySkin.label.alignment = TextAnchor.LowerLeft;
		
		GUI.Label(box_pos, char_name);
		
	}
	
	// Base selections i.e. move, action, gather, rest
	public void BaseSelectionButtons(){
	
		//If you want to move the buttons this is where you want to modify
		GUI.BeginGroup(new Rect( (3 * Screen.width)/ 4  ,  (3 * Screen.height)/ 4 , (3 * Screen.width)/8, (3*Screen.height)/ 10 ));
		
			GUI.depth = 1;
			mySkin.box.fontSize = mySkin.box.fontSize = Screen.height / 32;
			//GUI.enabled = !isAction && (GetCurrentFocusStatus() == ((Status.Clean | Status.Move) | GetCurrentFocusStatus()));// &&  (focusObject.GetComponent<BaseClass>().unit_status.status == Status.Gather) ;
			
			
			//GUI.enabled is how you make the button inactive to click on
			//This will check to see if there is not in the action window
			//or if the user has moved the unit or not
			GUI.enabled = !isAction && !GetCurrentFocusStatus().Move;// &&  (focusObject.GetComponent<BaseClass>().unit_status.status == Status.Gather) ;
			if(MakeButton(0,0,"Move", Style.move)){
				
				focusObject.GetComponent<BaseClass>().unit_status.Move();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				GM.instance.SetUnitControllerActiveOn(ref focusObject);	
				GM.instance.SetFocusController(true);

				WorldCamera.instance.transform.eulerAngles = Vector3.zero;
				//WorldCamera.instance.MainCamera = CurrentMainCamera();
				WorldCamera.instance.TurnCameraControlsOff();

				gui_method += MovementEndButton;
				//unit_character_controller = GameObject.FindWithTag("UnitController");
				smoothPos = true;
				//this will turn on the update that will allow update calls 
				//on used while the character is moving
				isMoving = true;
				//This will turn off the buttons so they wont be able to select the base selction buttons
				isAction = true;
			}
			
			//This will check to see if there is not in the action window
			//or if the user has used an action or not
			GUI.enabled = !isAction && !GetCurrentFocusStatus().Action;
			if(MakeButton(0, Screen.height/ 16, "Action", Style.action)){
				isAction = true;
				gui_method += ActionSelectionButtons;
				
			}
			
		GUI.enabled = proteus && !isAction && !GetCurrentFocusStatus().Gather;	
			if(MakeButton(0, Screen.height/ 8, "Gather", Style.gather)){

				/*
				focusObject.GetComponentInChildren<AnimationTriggers>().GatherAnimation();
				GM.instance.AddResourcesToCurrentPlayer(focusObject.GetComponent<BaseClass>().gather_amount);
				focusObject.GetComponent<BaseClass>().unit_status.Gather ();	
				*/
				
				// Send act of doing gathering over network
				focusObject.gameObject.GetPhotonView().RPC("UnitGather", PhotonTargets.AllBuffered);

				Quaternion lookat = Quaternion.LookRotation(current_procite_pos);
				lookat.eulerAngles = new Vector3(lookat.eulerAngles.x, focusObject.transform.rotation.eulerAngles.y, lookat.eulerAngles.z);
				focusObject.transform.rotation = lookat;

				// Send updateded transformation
				focusObject.gameObject.GetPhotonView().RPC("UpdateUnitTransformation", PhotonTargets.AllBuffered, 
			                                           focusObject.gameObject.transform.position, lookat);

				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
			}
			GUI.enabled = !isAction;
			if(MakeButton(0, (3 * Screen.height)/ 16, "Rest", Style.rest)){
				focusObject.GetComponent<BaseClass>().unit_status.Rest();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				
				focusObject.GetComponentInChildren<AnimationTriggers>().IdleAnimation();
				
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
	
	
	
	public static Texture2D CombineTextures(Texture2D aBaseTexture, Texture2D aToCopyTexture)
	{
		int aWidth = aBaseTexture.width;
		int aHeight = aBaseTexture.height;
		
		Texture2D aReturnTexture = new Texture2D(aWidth, aHeight, TextureFormat.RGBA32, false);
		
		Color[] aBaseTexturePixels = aBaseTexture.GetPixels();
		Color[] aCopyTexturePixels = aToCopyTexture.GetPixels();
		Color[] aColorList = new Color[aBaseTexturePixels.Length];
		
		int aPixelLength = aBaseTexturePixels.Length;
		
		for(int p = 0; p < aPixelLength; p++)
		{
			aColorList[p] = Color.Lerp(aBaseTexturePixels[p], aCopyTexturePixels[p], aCopyTexturePixels[p].a);
		}
		
		aReturnTexture.SetPixels(aColorList);
		aReturnTexture.Apply(false);
		
		return aReturnTexture;
	}
	
	//This is the end button movement
	public void MovementEndButton(){
	
		GUI.BeginGroup(new Rect( (25 * Screen.width)/ 32  ,  (29 * Screen.height)/ 40 , (3 * Screen.width)/8, (3 * Screen.height)/ 10 ));
		
			GUI.depth = 2;
			if( MakeButton(0,0, "End Movement", Style.move_cancel) ){
				if(GM.instance.CurrentFocus != null)
					GM.instance.SetFocusController(false);
				//GM.instance.SetFocusController(false);
				focus_object.GetComponentInChildren<AnimationTriggers>().MoveAnimation(0.0f);
				
				isMoving = false;
				smoothPos = true;
				WorldCamera.instance.ResetCamera();
				WorldCamera.instance.TurnCameraControlsOn();
				gui_method -= MovementEndButton;
				isAction  = false;
			}
		GUI.EndGroup();
	}



	//The action buttons is the attack button, use buttons, special buttons and a back button
	public void ActionSelectionButtons(){
	
		//This is the locations of where the buttons is located

		GUI.BeginGroup(new Rect( (25 * Screen.width)/ 32  ,  ((29 * Screen.height)/ 40) - shift , (3 * Screen.width)/8, ((3*Screen.height) / 10)+ shift ) );
		
		GUI.depth = 2;
		GUI.enabled = !CombatSystem.instance.CheckIfAttacking() && CombatSystem.instance.AnyNearbyUnitsToAttack(focusObject)  && !(GetCurrentFocusStatus().Action) && !isRecruiting;
			GUI.depth = 2;
			if(MakeButton(0,0, "Attack", Style.attack)){
				//Expend units action
//				CombatSystem.instance.GetNearbyAttackableUnits(focusObject);
				if(GM.instance.CurrentFocus != null)
					GM.instance.SetFocusController(false);
				focusObject.GetComponent<BaseClass>().unit_status.Action();		
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				CombatSystem.instance.AttackButtonClicked();
				//isAction  = false;
				
			}
			if(MakeButton(0, Screen.height/ 15 , "Use", Style.item)){
			
				focusObject.GetComponent<BaseClass>().unit_status.Action();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				gui_method -= ActionSelectionButtons;
				isAction = false;
			}
			if(MakeButton(0, (2 * Screen.height)/ 15, "Special", Style.special)){
			
				focusObject.GetComponent<BaseClass>().unit_status.Action();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				gui_method -= ActionSelectionButtons;
				isAction  =false;
			}
			
			GUI.enabled = true;
			GUI.depth = 2;
			
			if(focus_object.GetComponent<BaseClass>().unit_status.unit_type == UnitType.Leader)	{	
				shift = Screen.height/ 16;
				if(MakeButton(0, (3 * Screen.height)/ 15, "Recruit", Style.summon)){
					
					gui_method -= ActionSelectionButtons;
					gui_method -= BaseSelectionButtons;
					gui_method += RecruitMenuButtons;
					isRecruiting = true;
					isAction = false;
				}
				else
				{
					shift = 0;
				}
			}
			
			if(MakeButton(0, ((3 * Screen.height)/ 15) + shift, "Back", Style.back)){
				
				if(CombatSystem.instance.CheckIfAttacking()){
					CombatSystem.ResetCombatSystem();
				}	
				else{
					gui_method -= ActionSelectionButtons;
					isAction  = false;
				}
			}

		GUI.EndGroup();
	}
	
	//Recruit Menu buttons
	//This will need to get modify
	//What we want is the bio's to display on hover
	public void RecruitMenuButtons(){
	
		if(!GM.instance.IsItMyTurn())
			return;

		GUI.BeginGroup(new Rect( (24 * Screen.width)/ 32  ,  (10 * Screen.height)/ 40 , (2 * Screen.width)/8, 3* Screen.height/ 4 ));
			mySkin.box.fontSize = Screen.height / 32;
			GUI.Box (  new Rect (0,0,(2 * Screen.width)/8, 3*Screen.height/ 4), "Recruit Menu"  );
//			GUI.enabled = 
		GUI.enabled =  (GM.instance.GetResourceFrom(GM.instance.CurrentPlayer) > _rs.unit_cost.scout );
		if (MakeButton((1 * Screen.width)/64, (95*Screen.height)/1024 ,string.Format("Scout  {0}", _rs.unit_cost.scout), Style.scout)){
				focusObject.GetComponent<BaseClass>().unit_status.Action();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Scout);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				isRecruiting = false;
			}
		GUI.enabled =  (GM.instance.GetResourceFrom(GM.instance.CurrentPlayer) > _rs.unit_cost.braver );
		
		if (MakeButton((1 * Screen.width)/64, (2*95*Screen.height) /1024,string.Format("Braver  {0}", _rs.unit_cost.braver), Style.braver)){
			
				focusObject.GetComponent<BaseClass>().unit_status.Action();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Braver);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				isRecruiting = false;
			
			}
		GUI.enabled =  (GM.instance.GetResourceFrom(GM.instance.CurrentPlayer) > _rs.unit_cost.arcane );
		
		if (MakeButton((1 * Screen.width)/64, (3*95*Screen.height) /1024, string.Format("Arcane  {0}", _rs.unit_cost.arcane), Style.arcane)){
			
				focusObject.GetComponent<BaseClass>().unit_status.Action();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Arcane);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				isRecruiting = false;
			
			}
		GUI.enabled =  (GM.instance.GetResourceFrom(GM.instance.CurrentPlayer) > _rs.unit_cost.sniper );
		
		if (MakeButton((1 * Screen.width)/64, (4*95*Screen.height) /1024,string.Format("Sniper  {0}", _rs.unit_cost.sniper), Style.sniper)){
			
				focusObject.GetComponent<BaseClass>().unit_status.Action();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Sniper);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				isRecruiting = false;
			
			}
		GUI.enabled =  (GM.instance.GetResourceFrom(GM.instance.CurrentPlayer) > _rs.unit_cost.titan );
		
		if (MakeButton((1 * Screen.width)/64, (5*95*Screen.height) /1024,string.Format("Gigan  {0}", _rs.unit_cost.titan), Style.gigan)){
				
				focusObject.GetComponent<BaseClass>().unit_status.Action();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Titan);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				isRecruiting = false;
			
			}
		GUI.enabled =  /*false &&*/ (GM.instance.GetResourceFrom(GM.instance.CurrentPlayer) > _rs.unit_cost.vangaurd );
		
		if (MakeButton((1 * Screen.width)/64, (6*95*Screen.height) /1024,string.Format("Vangaurd  {0}", _rs.unit_cost.vangaurd), Style.vanguard)){
			
				focusObject.GetComponent<BaseClass>().unit_status.Action();
				//focusObject.GetPhotonView().RPC("UpdateUnitStatus", PhotonTargets.AllBuffered, focusObject.GetComponent<BaseClass>().unit_status.status);
				GM.instance.RecruitUnitOnCurrentPlayer(UnitType.Vangaurd);
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				isRecruiting = false;
			
			}
		GUI.enabled = true;
		if (MakeButton((1 * Screen.width)/16, (660*Screen.height) /1024, "Back", Style.back)){
				
				isAction = true;
				gui_method -= RecruitMenuButtons;
				gui_method += BaseSelectionButtons;
				gui_method += ActionSelectionButtons;
				isRecruiting = false;
			
			}
		
		GUI.EndGroup();
	}
	#endregion
	
	
	
	
	#region Helper Functions
	
	
	private Status GetCurrentFocusStatus(){
	
		return focusObject.GetComponent<BaseClass>().unit_status.status;
	}
	
	public bool NearProcite(){
	
		if (procite_locations.Length != 0 ){
			Vector3 offset;
			for(int i = 0; i < procite_locations.Length; ++i){
				offset = procite_locations[i].gameObject.transform.position - focusObject.transform.position;
				float range = focusObject.GetComponent<BaseClass>().gather_range;
				range = range * range;
				if( offset.sqrMagnitude < range) {
					current_procite_pos = procite_locations[i].gameObject.transform.position;
					return true;
				}
			
			}
			return false;			
		}else
			return false;
	}

	private bool MakeButton(float left, float top, string buttonName, Style index){
		float height = Screen.height / 16;
		return GUI.Button(new Rect(left, top, 64* height / 15, height), buttonName, mySkin.customStyles[(int)index]);
		
	}
	private bool MakeButton(Rect box, string buttonName, Style index){
		
		return GUI.Button ( box, buttonName, mySkin.customStyles[(int)index]);
	}	

	public void SmoothFollow(GameObject target){

		//lookAtHeight = WorldCamera.instance.MinCameraHeight() / 2;
		
		Vector3 focus =  target.transform.position;
		
		focus.y += (0.85f) * target.GetComponent<CapsuleCollider>().height;
		
		wantedRotationAngle = target.transform.eulerAngles.y;
		wantedHeight = focus.y + lookAtHeight;
		
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
		Vector3 worldCameraPosition =  target.transform.position;
		
		DistancefromPlayer = (wantedHeight - target.transform.position.y )/ distanceScale;
		
		worldCameraPosition -= currentRotation * target.transform.forward * DistancefromPlayer;	
		
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
			
			WorldCamera.instance.transform.position = worldCameraPosition;
		//	mainCamera.transform.LookAt(target);
			
		}
		WorldCamera.instance.MainCamera.transform.LookAt(focus);
		
		//var rotation = Quaternion.LookRotation(target.position - worldCamera.transform.position);
		//mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, rotation, Time.deltaTime * 5.5);
	}
	GameObject CurrentMainCamera(){
		
		return Camera.main.gameObject;
		
	}
	#endregion
}

