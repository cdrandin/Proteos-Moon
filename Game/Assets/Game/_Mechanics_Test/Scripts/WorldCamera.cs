using UnityEngine;
using System.Collections;

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
	public static WorldCamera Instance;

	public GameObject MainCamera;
	private GameObject ScrollAngle;

	private float cameraMoveSpeed = 60f; // This values adjust the camera speed
	private float shiftBonus      = 45f; 
	private float mouseBoundary   = 25f; //This value is the padding around the screen to apply mouse movement

	private float mouseX;
	private float mouseY;

	private bool VerticalRotationEnabled = true;

	//These values are in degrees
	private float VerticalRoationMin = 0f;
	private float VerticalRoationMax = 65f;

	public Terrain WorldTerrain;
	public float WorldTerrainPadding = 25f;

	[HideInInspector] public float cameraHeight; //Only for scrolling or zooming
	[HideInInspector] public float cameraY; //this will change relative to terrain
	private float maxCameraHeight = 85f;
	public LayerMask TerrainOnly;
	private float minDistanceToObject = 40f;

	#endregion
	
	
	void Awake()
	{
		Instance = this;
	}
	
	
	void Start () {
		
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
		ScrollAngle =  new GameObject();
	}
	
	
	
	
	void LateUpdate () {

		HandleMouseRotation ();

		ApplyScroll ();

		if(CheckIfUserCameraInput())
		{
			Vector3 desiredTranslation = GetDesiredTranslation();
			if(!isDesiredPositionOverBoundaries(desiredTranslation))
			{
				Vector3 desiredPosition = transform.position + desiredTranslation;

				UpdateCameraY(desiredPosition);

				this.transform.Translate(desiredTranslation);
			}
		}

		ApplyCameraY();
	}

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
	public void HandleMouseRotation()
	{
		var easeFactor = 10f;
		if (Input.GetMouseButton (1)) {
			
			//Horiztonal rotation
			if(Input.mousePosition.x != mouseX){
				var cameraRotationY = (Input.mousePosition.x - mouseX) * easeFactor * Time.deltaTime;
				this.transform.Rotate(0, cameraRotationY, 0);
			}
			
			//Vertical Rotation
			if(VerticalRotationEnabled && Input.mousePosition.y != mouseY){
				
				GameObject MainCamera = this.gameObject.transform.FindChild("Main Camera").gameObject;
				
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
		float easeFactor = 150f;

		if (Application.isWebPlayer)
			easeFactor = 20f;

		float ScrollWheelValue = Input.GetAxis ("Mouse ScrollWheel") * easeFactor;

		//check deadZone
		if ((ScrollWheelValue > -deadZone && ScrollWheelValue < deadZone) || ScrollWheelValue == 0f)
			return;

		float EulerAnglesX = MainCamera.transform.localEulerAngles.x;

		//Configure the ScrollAngle GameObject
		ScrollAngle.transform.position = transform.position;
		ScrollAngle.transform.eulerAngles = new Vector3 (EulerAnglesX, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
		ScrollAngle.transform.Translate (Vector3.back * ScrollWheelValue);

		Vector3 desiredScrollPosition = ScrollAngle.transform.position;

		//check if in boundaries
		if (desiredScrollPosition.x < cameraLimits.LeftLimit || desiredScrollPosition.x > cameraLimits.RightLimit) return;
		if (desiredScrollPosition.z < cameraLimits.TopLimit || desiredScrollPosition.z > cameraLimits.BottomLimit) return;
		if (desiredScrollPosition.y > maxCameraHeight || desiredScrollPosition.y < MinCameraHeight()) return;

		//update the cameraHeight and the CameraY;
		float heightDifference = desiredScrollPosition.y - this.transform.position.y;
		cameraHeight += heightDifference;
		UpdateCameraY (desiredScrollPosition);

		//update the camera position
		this.transform.position = desiredScrollPosition;

		return;

	}


	//Calculate the new height for the camera baesed on the terrain height
	public void  UpdateCameraY(Vector3 desiredPosition){

		RaycastHit hit;
		float deadZone = 0.1f;

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
		float smoothTime = 0.2f;
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
		float moveSpeed = 0f;
		Vector3 desiredTranslation = new Vector3 ();
		
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			moveSpeed = (cameraMoveSpeed + shiftBonus) * Time.deltaTime;
		else
			moveSpeed = cameraMoveSpeed * Time.deltaTime;
		
		
		//move via keyboard and via mouse
		if(Input.GetKey(KeyCode.W) || Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit))
			desiredTranslation += Vector3.forward * moveSpeed;
		
		if (Input.GetKey (KeyCode.S) || Input.mousePosition.y < mouseScrollLimits.BottomLimit)
			desiredTranslation += Vector3.back * moveSpeed;
		
		
		if (Input.GetKey (KeyCode.A) || Input.mousePosition.x < mouseScrollLimits.LeftLimit)
			desiredTranslation += Vector3.left * moveSpeed;
		
		
		if (Input.GetKey (KeyCode.D) || Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit))
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
	
	
	
	
	
	#region Helper functions
	
	public static bool AreCameraKeyboardButtonsPressed()
	{
		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
			return true; else return false;
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
}
