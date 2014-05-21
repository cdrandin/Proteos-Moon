using UnityEngine;
using System.Collections;

/*
 * TODO: - Set "focus" target when a unit or leader is select using camera or mouse
 * 		 - Method for return the focus target
 *       - If camera is not focused on the unit or mose clicks off the unit, focus = null
 */


/* 
Class to control the camera within the game world.
Camera will move up, down, left and right when the users mouse hits the side of the screen in 2D space.
Camera will check desired location, will stop if over limits.
Camera can also be controlled by W,A,S,D keys, will call the same movement as the mouse events.
*/


public class WorldCamera : MonoBehaviour {
	
	#region structs
	
	//box limits Struct
	public struct BoxLimit {
		public float LeftLimit;
		public float RightLimit;
		public float TopLimit;
		public float BottomLimit;
		
	}
	
	#endregion
	
	
	#region class variables
	
	public static BoxLimit cameraLimits       = new BoxLimit();
	public static BoxLimit mouseScrollLimits  = new BoxLimit();
	public static WorldCamera instance;

	public GameObject MainCamera;
	private GameObject ScrollAngle;

	private float cameraMoveSpeed = 50.0f; // This values adjust the camera speed
	private float shiftBonus      = 45f; // This value will increase the speed while holding shift

	private float mouseX;
	private float mouseY;

	private bool VerticalRotationEnabled = true;
	private bool cameraOn = true;

	//These values are in degrees
	private float VerticalRoationMin = 0f;
	private float VerticalRoationMax = 65f;

	public Terrain WorldTerrain;
	public float WorldTerrainPadding = 5f;

	[HideInInspector] public float cameraHeight; //Only for scrolling or zooming
	[HideInInspector] public float cameraY; //this will change relative to terrain
	private float maxCameraHeight = 200f;
	public LayerMask TerrainOnly;
	public float minDistanceToObject = 10f;

	//private bool _local;
	private Vector3 _previous_location; // Use to keep track of previous location before following a unit

	//private float rotationDamping = 3.0f;
	

	#endregion
	
	
	void Awake()
	{
		
		this.transform.localEulerAngles = Vector3.zero;
		instance = this;	
		//_local = true; // simply bool to show local host
		ScrollAngle =  new GameObject("ScrollAngle");
		ScrollAngle.transform.parent = gameObject.transform;
	}
	
	void Start () {

		InitializeMainCamera();
		this.transform.localEulerAngles = Vector3.zero;
		//Declare camera limits
		cameraLimits.LeftLimit   = WorldTerrain.transform.position.x + WorldTerrainPadding;
		
		cameraLimits.RightLimit  = WorldTerrain.terrainData.size.x - WorldTerrainPadding;

		cameraLimits.TopLimit    = WorldTerrain.terrainData.size.z - WorldTerrainPadding;
		
		cameraLimits.BottomLimit = WorldTerrain.transform.position.z + WorldTerrainPadding;
		

		//cameraHeight = transform.position.y;	
		//ScrollAngle = gameObject;
		
	}
	
	public void LeaderFocus(){
	
	
		transform.position = GM.instance.Photon_Leader.transform.position;
		
		transform.position -= GM.instance.Photon_Leader.transform.forward * 
								   GM.instance.Photon_Leader.transform.localScale.y * 
								   GM.instance.Photon_Leader.GetComponent<CapsuleCollider>().height * 2;
		transform.position = new Vector3(transform.position.x, 
		                                      transform.position.y +( GM.instance.Photon_Leader.transform.localScale.y *  GM.instance.Photon_Leader.GetComponent<CapsuleCollider>().height * 2) 
		                                      ,transform.position.z );
		cameraHeight = GM.instance.Photon_Leader.transform.localScale.y *  GM.instance.Photon_Leader.GetComponent<CapsuleCollider>().height * 2;
		cameraY = transform.position.y;
		WorldCamLookAt(GM.instance.Photon_Leader );
		
	}
	
	void InitializeMainCamera(){
	
		MainCamera = Camera.main.gameObject;
		
		MainCamera.GetComponent<AudioListener>().enabled = true;
		MainCamera.GetComponent<Camera>().enabled = true;
		
		// Add camera from container
		MainCamera.transform.parent = this.transform;
		
		//Change Transform information
		//this.transform.position = new Vector3( MainCamera.transform.position.x, Mathf.Clamp(MainCamera.transform.position.y, minDistanceToObject+1, maxCameraHeight - 1), MainCamera.transform.position.z);
		MainCamera.transform.localPosition = new Vector3(0.0f ,0.0f ,0.0f);
		this.transform.localEulerAngles = new Vector3( 0.0f, MainCamera.transform.localEulerAngles.y, 0.0f);
		MainCamera.transform.localEulerAngles = new Vector3( MainCamera.transform.localEulerAngles.x, 0.0f, 0.0f);
	}
	
	
	void Update (){}

	void LateUpdate () {
		if(cameraOn ){
			
			HandleMouseRotation ();
			
			ApplyScroll ();
			
			if(CheckIfUserCameraInput()){
				Vector3 desiredTranslation = GetDesiredTranslation();
				if(!isDesiredPositionOverBoundaries( desiredTranslation )){
					
					Vector3 desiredPosition = transform.position + desiredTranslation;
					
					UpdateCameraY(desiredPosition);
	
					this.transform.Translate(desiredTranslation);
				}
				ApplyCameraY();
			}
		}
	}

	
	#region Helper functions
	//calculate the minimum camera height
	public float MinCameraHeight(){

		RaycastHit hit;
		float minCameraHeight = WorldTerrain.transform.position.y;

		if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, TerrainOnly)){

			minCameraHeight = hit.point.y + minDistanceToObject;
		}

		return minCameraHeight;

	}

	//Handles the mouse rotation vertically and horizontally.
	public void HandleMouseRotation(){
		var easeFactor = 10f;
		if (Input.GetMouseButton (1)) {
			
			//Horiztonal rotation
			if(Input.mousePosition.x != mouseX){
				var cameraRotationY = (Input.mousePosition.x - mouseX) * easeFactor * Time.deltaTime;
				this.transform.Rotate(0, cameraRotationY, 0);
			}
			
			//Vertical Rotation
			if(VerticalRotationEnabled && Input.mousePosition.y != mouseY){

				var cameraRotationX =( mouseY - Input.mousePosition.y) * easeFactor * Time.deltaTime;
				var desiredRotationX = MainCamera.transform.eulerAngles.x + cameraRotationX;
				
				if(desiredRotationX >= VerticalRoationMin && desiredRotationX <= VerticalRoationMax){
					MainCamera.transform.Rotate (cameraRotationX, 0, 0);
				}
				
			}
		}
		
		mouseX = Input.mousePosition.x;
		mouseY = Input.mousePosition.y;
		
	}

	// Apply a scroll using the mouse wheel, or the trackpad
	public void ApplyScroll(){

		float deadZone = 0.01f;
		float easeFactor = 50f;

		if (Application.isWebPlayer)
			easeFactor = 20f;

		float ScrollWheelValue = -1*Input.GetAxis ("Mouse ScrollWheel") * easeFactor;
		//check deadZone
		
		if ((ScrollWheelValue > -deadZone && ScrollWheelValue < deadZone) || ScrollWheelValue == 0f)
			return;

		float EulerAnglesX = MainCamera.transform.localEulerAngles.x;

		//Configure the ScrollAngle GameObject
		ScrollAngle.transform.position = transform.position;
		ScrollAngle.transform.eulerAngles = new Vector3 (EulerAnglesX, transform.eulerAngles.y, this.transform.eulerAngles.z);
		ScrollAngle.transform.Translate (Vector3.back * ScrollWheelValue);

		Vector3 desiredScrollPosition = ScrollAngle.transform.position;

		//check if in boundaries
		if (desiredScrollPosition.x < cameraLimits.LeftLimit || desiredScrollPosition.x > cameraLimits.RightLimit) return;
		if (desiredScrollPosition.z > cameraLimits.TopLimit || desiredScrollPosition.z < cameraLimits.BottomLimit) return;
		if (desiredScrollPosition.y > maxCameraHeight || desiredScrollPosition.y < MinCameraHeight ()) return;
		//update the cameraHeight and the CameraY;
		float heightDifference = desiredScrollPosition.y - this.transform.position.y;
		cameraHeight += heightDifference;
		UpdateCameraY (desiredScrollPosition);

		//update the camera position
		this.transform.position = desiredScrollPosition;

		return;

	}


	//Calculate the new height for the camera baesed on the terrain height
	public void UpdateCameraY(Vector3 desiredPosition){

		RaycastHit hit;
		float deadZone = 0.0001f;
		
		if(Physics.Raycast(desiredPosition, Vector3.down, out hit, Mathf.Infinity)){
			float newHeight = cameraHeight + hit.point.y;
			
			float heightDifference = newHeight - cameraY;
			
			if(heightDifference > -deadZone && heightDifference < deadZone) return;

			if(newHeight > maxCameraHeight || newHeight < MinCameraHeight()) return;
			
			cameraY = newHeight;
			
		}
		return;

	}

	//Apply the camera Y to a smooth damp, and update camera Y position
	public void ApplyCameraY(){
		
		if (cameraY == transform.position.y || cameraY == 0)
			return;

		//smooth damp
		float smoothTime = 0.1f;
		float yVelocity = 0.0f;
		
		float newPoisitionY = Mathf.SmoothDamp (transform.position.y, cameraY, ref yVelocity, smoothTime);

		if (newPoisitionY < maxCameraHeight) {
			
			transform.position = new Vector3(transform.position.x, newPoisitionY, transform.position.z);

		}


	}
	
	//Check if the user is inputting commands for the camera to move
	public bool CheckIfUserCameraInput()
	{

		
		return WorldCamera.AreCameraKeyboardButtonsPressed();
	}

	//Works out the cameras desired location depending on the players input
	public Vector3 GetDesiredTranslation()
	{
		float moveSpeed = 5f;
		Vector3 desiredTranslation = new Vector3 ();
		
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			moveSpeed = (cameraMoveSpeed + shiftBonus) * Time.deltaTime;
		else
			moveSpeed = cameraMoveSpeed * Time.deltaTime;
		
		
		//move via keyboard and via mouse
		if (Input.GetKey (KeyCode.W))
			desiredTranslation += Vector3.forward * moveSpeed;
		
		if (Input.GetKey (KeyCode.S))
			desiredTranslation += Vector3.back * moveSpeed;
		
		if (Input.GetKey (KeyCode.A))
			desiredTranslation += Vector3.left * moveSpeed;
		
		if (Input.GetKey (KeyCode.D))
			desiredTranslation += Vector3.right * moveSpeed;
		return desiredTranslation;
	}
		
	//checks if the desired position crosses boundaries
	public bool isDesiredPositionOverBoundaries(Vector3 desiredTranslation)
	{

		Vector3 desiredWorldPosition = this.transform.TransformPoint (desiredTranslation);

		bool overBoundaries = false;
		//check boundaries
		if(desiredWorldPosition.x < cameraLimits.LeftLimit)
			overBoundaries = true;
		
		if(desiredWorldPosition.x > cameraLimits.RightLimit)
			overBoundaries = true;
		
		if(desiredWorldPosition.z > cameraLimits.TopLimit)
			overBoundaries = true;
		
		if(desiredWorldPosition.z < cameraLimits.BottomLimit)
			overBoundaries = true;
		
		return overBoundaries;
	}
	

	
	public static bool AreCameraKeyboardButtonsPressed()
	{
		return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) ?
				true : 
				false;
	}
	
	#endregion

	public bool IsCameraOnControlsOn( ){return cameraOn;}

	public void TurnCameraControlsOff(){ 

	cameraOn = false;}

	public void TurnCameraControlsOn(){ 

	cameraOn = true;}


	public void WorldCamLookAt(GameObject focus){
	
		var newRot = Quaternion.FromToRotation( this.transform.forward, focus.transform.position - this.transform.position );
		
		this.transform.localEulerAngles = new Vector3 (0.0f , newRot.eulerAngles.y, 0.0f);
		MainCamera.transform.localEulerAngles = new Vector3 (newRot.eulerAngles.x , 0.0f, 0.0f);
	}

	public void ResetCamera(){
		
		Vector3 oldWorldTransformEul = new Vector3(0.0f, WorldCamera.instance.transform.eulerAngles.y + WorldCamera.instance.MainCamera.transform.localEulerAngles.y, 0.0f);
		Vector3 oldMainEul = new Vector3(WorldCamera.instance.transform.eulerAngles.x + WorldCamera.instance.MainCamera.transform.localEulerAngles.x, 0.0f, 0.0f);
		WorldCamera.instance.transform.rotation = Quaternion.Euler(oldWorldTransformEul);
		WorldCamera.instance.MainCamera.transform.localRotation = Quaternion.Euler(oldMainEul);
	}
	
	public void StartCharacterFollow(GameObject target){
	
		MainCamera.transform.localEulerAngles = Vector3.zero;
		
		TurnCameraControlsOff();	
		
		StartCoroutine(SmoothFollow(target));
	}
	
	public void StopCharacterFollow(){
	
		StopCoroutine("SmoothFollow");
		ResetCamera();
		TurnCameraControlsOn();
		
	}
	
	public IEnumerator SmoothFollow(GameObject target){
		
		//lookAtHeight = WorldCamera.instance.MinCameraHeight() / 2;
		
		
		float characterHeight = (0.85f) * target.GetComponent<CapsuleCollider>().height;
		
		float lookAtHeight = 5.0f;
		float heightDamping = 10.0f , rotationDamping = 10.0f;//, button_pos = Screen.width - 250;
		float wantedRotationAngle, currentRotationAngle, currentHeight;
		float distanceScale = 0.5f;
		Vector3  characterPosition;
		Quaternion currentRotation;	
		float DistancefromPlayer = characterHeight / distanceScale;
		cameraHeight = characterHeight;

		while(true){
		
			wantedRotationAngle = target.transform.eulerAngles.y;
			cameraY = target.transform.position.y + characterHeight +  lookAtHeight;
			
			currentRotationAngle = transform.eulerAngles.y;
			currentHeight = transform.position.y;
			
			// Damp the rotation around the y-axis
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			
			// Damp the height
			currentHeight = Mathf.Lerp (currentHeight, cameraY, heightDamping * Time.deltaTime);
			
			// Convert the angle into a rotation
			currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
			
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			Vector3 worldCameraPosition =  target.transform.position;

			//worldCameraPosition -= currentRotation * target.transform.forward * DistancefromPlayer;	
			worldCameraPosition -= currentRotation * Vector3.forward * DistancefromPlayer;	
			
			// Set the height of the camera
			worldCameraPosition = new Vector3 (worldCameraPosition.x, currentHeight, worldCameraPosition.z);
			
				
			transform.position = worldCameraPosition;
			//	mainCamera.transform.LookAt(target);
			
			characterPosition = target.transform.position;
			characterPosition.y += characterHeight;
			
			transform.LookAt(characterPosition);
			
			yield return null;
		}	
		//var rotation = Quaternion.LookRotation(target.position - worldCamera.transform.position);
		//mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, rotation, Time.deltaTime * 5.5);
	}
	
	public void SetMainCamera ( GameObject focus )
	{
		MainCamera = focus;
	}
	
//	rotation = Quaternion.LookRotation(target.position - transform.position);
//	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
}
