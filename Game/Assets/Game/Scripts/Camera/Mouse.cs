using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {


	RaycastHit hit;


	public static GameObject CurrentlySelectedUnit;
	public static GameObject SelectedProjector;
	private static Vector3 mouseDownPoint;
	private static bool isSelection;

	void Start(){
		}

	void Awake(){

		mouseDownPoint = Vector3.zero;
		isSelection = false;
		SelectedProjector = GameObject.Find ("SelectProjector");
		SelectedProjector.SetActive (false);
	}

	void Update () {
	
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);


		if (isSelection) {

			UpdateSelectedProjection(CurrentlySelectedUnit.transform.position);

		}


		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
			//store point at mouse button down
			if(Input.GetMouseButtonDown(0))
				mouseDownPoint = hit.point;
			if(hit.collider.name == "Terrain"){
				//When we click the right mouse button, instantiate target
				//0 left
				//1 right
				//2 middle
				if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint) ){
					DeselectGameobjectIfSelected();
					isSelection = false;
				}
			} // end of the terrain

			else{
				//hitting other objects!
				if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
				{
					//is the user hitting a unit?
					if(hit.collider.tag == "Unit"){

						//found a unit we can select!
						//are we selecting a different object?
						if(CurrentlySelectedUnit != hit.collider.gameObject){
							//move the selector position and activate it
							UpdateSelectedProjection(hit.collider.gameObject.transform.position);
							//repalce currently seelcted unit
							CurrentlySelectedUnit = hit.collider.gameObject;
							isSelection = true;
						}
					} else {
						//if this object is not a unit
						DeselectGameobjectIfSelected();
						isSelection = false;
					}
				}
			}

		} else {

			if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
				DeselectGameobjectIfSelected();
		}
	}	

	#region Helper Functions

	// Align projectors over selected unit
	public static void UpdateSelectedProjection(Vector3 newPosition)
	{
		SelectedProjector.transform.position = newPosition;

		//if selected is not active activate
		if(!SelectedProjector.activeSelf)
			SelectedProjector.SetActive (true);
	}

	//did user perform a mouse click
	public bool DidUserClickLeftMouse(Vector3 hitPoint)
	{

		float clickZone = 1.3f;

		if ((mouseDownPoint.x < hitPoint.x + clickZone && mouseDownPoint.x > hitPoint.x - clickZone) &&
						(mouseDownPoint.y < hitPoint.y + clickZone && mouseDownPoint.y > hitPoint.y - clickZone) &&
						(mouseDownPoint.z < hitPoint.z + clickZone && mouseDownPoint.z > hitPoint.z - clickZone)) {
						return true;
				}
		else
				return false;
	}

	//deselects gameobject if selected
	public static void DeselectGameobjectIfSelected(){

		SelectedProjector.SetActive (false);
		CurrentlySelectedUnit = null;

	}

	#endregion
}
