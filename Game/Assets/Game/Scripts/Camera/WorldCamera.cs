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
	private float mouseBoundary   = 5f; //This value is the padding around the screen to apply mouse movement

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

	private bool _local;
	private Vector3 _previous_location; // Use to keep track of previous location before following a unit

	//private float rotationDamping = 3.0f;
	public float DistanceFromPlayer = 5.0f;
	
	private float wantedRotationAngle;
	private float wantedHeight;
	private float currentRotationAngle;
	private float currentHeight;
	private Quaternion currentRotation;	
	#endregion
	
	
	void Awake()
	{
		this.transform.localEulerAngles = Vector3.zero;
		instance = this;	
		_local = true; // simply bool to show local host
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
		
		//Declare Mouse Scroll Limits
		mouseScrollLimits.LeftLimit   = mouseBoundary;
		mouseScrollLimits.RightLimit  = mouseBoundary;
		mouseScrollLimits.TopLimit    = mouseBoundary;
		mouseScrollLimits.BottomLimit = mouseBoundary;

		cameraHeight = transform.position.y;
		
		//ScrollAngle = gameObject;
		
	}
	
	public void LeaderFocus(){
	
	
		this.transform.position = GM.instance.Photon_Leader.transform.position;
		
		this.transform.position -= GM.instance.Photon_Leader.transform.forward * 
								   GM.instance.Photon_Leader.transform.localScale.y * 
								   GM.instance.Photon_Leader.GetComponent<CapsuleCollider>().height * 2;
		this.transform.position = new Vector3(this.transform.position.x, 
		                                      this.transform.position.y +( GM.instance.Photon_Leader.transform.localScale.y *  GM.instance.Photon_Leader.GetComponent<CapsuleCollider>().height * 2) 
		                                      ,this.transform.position.z );
		
		this.WorldCamLookAt(GM.instance.Photon_Leader );
		
	}
	
	void InitializeMainCamera(){
	
		MainCamera = GameObject.Find ("Main Camera");
		
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

		bool keyboardMove;
		bool mouseMove;
		bool canMove;
		
		//check keyboard		
		if(WorldCamera.AreCameraKeyboardButtonsPressed()){
			keyboardMove = true;			
		} else {
			keyboardMove = false;
		}
		
		//check mouse position
		if(WorldCamera.IsMousePositionWithinBoundaries())
			mouseMove = true; else mouseMove = false;
		
		
		if(keyboardMove || mouseMove)
			canMove = true; else canMove = false;
		
		return canMove;
	}

	//Works out the cameras desired location depending on the players input
	public Vector3 GetDesiredTranslation()
	{
		float moveSpeed = 10f;
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
	
	public static bool IsMousePositionWithinBoundaries()
	{
		if(
			(Input.mousePosition.x < mouseScrollLimits.LeftLimit && Input.mousePosition.x > -5) ||
			(Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit) && Input.mousePosition.x < (Screen.width + 5)) ||
			(Input.mousePosition.y < mouseScrollLimits.BottomLimit && Input.mousePosition.y > -5) ||
			(Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit) && Input.mousePosition.y < (Screen.height + 5))
			)
			return true; else return false;
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

	public void ChangeCamera()
	{
		string camera_name = "";

		if(GM.instance.IsOn)
		{
			camera_name = "camera_player" + ((int)GM.instance.CurrentPlayer + 1).ToString();
		}
		 else
		{
			camera_name = "camera_player" + ((int)GM.instance.CurrentPlayer + 1).ToString();
		}

		// Local stuff, same computer
		if(_local)
		{
			if(MainCamera != null)
			{
				// Remove camera from container
				MainCamera.transform.parent = null;

				//saves transform in old camera
				MainCamera.transform.position = this.transform.position;
				MainCamera.transform.eulerAngles = new Vector3( MainCamera.transform.eulerAngles.x,  this.transform.eulerAngles.y, 0.0f);


				// Disable audio listener before we switch
				MainCamera.GetComponent<AudioListener>().enabled = false;
				MainCamera.GetComponent<Camera>().enabled = false;
				
				MainCamera = GameObject.Find (camera_name);
				
				// switch cameras then turn on this ones audio listener
				MainCamera.GetComponent<AudioListener>().enabled = true;
				MainCamera.GetComponent<Camera>().enabled = true;

				// Add camera from container
				MainCamera.transform.parent = this.transform;

				//Update Transformation
				this.transform.position = MainCamera.transform.position;
				MainCamera.transform.localPosition = new Vector3(0.0f ,0.0f ,0.0f);
				this.transform.eulerAngles = new Vector3( 0.0f, MainCamera.transform.eulerAngles.y, 0.0f);
				MainCamera.transform.localEulerAngles = new Vector3( MainCamera.transform.eulerAngles.x, 0.0f, 0.0f);
			}
			else
			{
				MainCamera = GameObject.Find (camera_name);

				MainCamera.GetComponent<AudioListener>().enabled = true;
				MainCamera.GetComponent<Camera>().enabled = true;
				
				// Add camera from container
				MainCamera.transform.parent = this.transform;
				
				//Change Transform information
				this.transform.position = MainCamera.transform.position;
				MainCamera.transform.localPosition = new Vector3(0.0f ,0.0f ,0.0f);
				this.transform.eulerAngles = new Vector3( 0.0f, MainCamera.transform.eulerAngles.y, 0.0f);
				MainCamera.transform.localEulerAngles = new Vector3( MainCamera.transform.eulerAngles.x, 0.0f, 0.0f);
			}
		} // End of local
	}


	public void ResetCamera(){
		
		Vector3 oldWorldTransformEul = new Vector3(0.0f, WorldCamera.instance.transform.eulerAngles.y + WorldCamera.instance.MainCamera.transform.localEulerAngles.y, 0.0f);
		Vector3 oldMainEul = new Vector3(WorldCamera.instance.MainCamera.transform.localEulerAngles.x, 0.0f, 0.0f);
		WorldCamera.instance.transform.rotation = Quaternion.Euler(oldWorldTransformEul);
		WorldCamera.instance.MainCamera.transform.localRotation = Quaternion.Euler(oldMainEul);
	}
	/*
	public void SmoothFollow(GameObject target){

		target.transform.position =  new Vector3 (target.transform.position.x, target.transform.position.y + target.GetComponent<CharacterController>().height, target.transform.position.z);
		wantedRotationAngle = target.transform.eulerAngles.y;
		wantedHeight = target.transform.position.y + height;
		
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
		this.transform.position = target.transform.position;
		this.transform.position -= currentRotation * Vector3.forward * DistanceFromPlayer;
		
		// Set the height of the camera
		this.transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
		
		
		this.transform.LookAt(new Vector3(0,target.position.y, 0 ) );
		MainCamera.transform.LookAt(new Vector3(target.position.x, 0, 0) );
	}
*/
	public void SetMainCamera ( GameObject focus )
	{
		MainCamera = focus;
	}
	
//	rotation = Quaternion.LookRotation(target.position - transform.position);
//	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
}
