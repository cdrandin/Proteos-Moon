using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {


	RaycastHit hit;

	public static GameObject CurrentlySelectedUnit;

	public GameObject Target;

	private Vector3 mouseDownPoint;
	

	void Awake(){

		mouseDownPoint = Vector3.zero;

		}

	void Update () {
	
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);


		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){

			//store point at mouse button down
			if(Input.GetMouseButtonDown(0))
				mouseDownPoint = hit.point;
		
	
			if(hit.collider.name == "Terrain"){
				Debug.Log("Collide with Terrain");
				//When we click the right mouse button, instantiate target
				//0 left
				//1 right
				//2 middle
				if(Input.GetMouseButtonDown(1)){

					GameObject TargetObj = Instantiate(Target, hit.point, Quaternion.identity) as GameObject;
					TargetObj.name = "Target Instantiated";
				}

				else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint) )
					DeselectGameobjectIfSelected();
			} // end of the terrain

			else{
				//hitting other objects!
				if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
				{
					Debug.Log("Hit Colider Position" + hit.collider.transform.position);
					//is the user hitting a unit?
					if(hit.collider.transform.FindChild ("Selected")){


						//found a unit we can select!
						Debug.Log ("Found a unit!");
						//are we selecting a different object?
						if(CurrentlySelectedUnit != hit.collider.gameObject){

							//activate the selector
							GameObject SelectedObj = hit.collider.transform.FindChild("Selected").gameObject;
							SelectedObj.SetActive(true);

							//deactivate the currently selected objects selector
							if(CurrentlySelectedUnit != null)
								CurrentlySelectedUnit.transform.FindChild("Selected").gameObject.SetActive(false);

							//repalce currently seelcted unit
							CurrentlySelectedUnit = hit.collider.gameObject;
						}
					} else {

						//if this object is not a unit
						Debug.Log ("Not a unit!");
						DeselectGameobjectIfSelected();
					}



				}
			}

		} else {

			if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
				DeselectGameobjectIfSelected();

		}

		Debug.DrawRay (ray.origin, ray.direction * Mathf.Infinity, Color.magenta);

	}



	#region Helper Functions

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

		if (CurrentlySelectedUnit != null) {

			CurrentlySelectedUnit.transform.FindChild("Selected").gameObject.SetActive(false);
			CurrentlySelectedUnit = null;
		}

	}

	#endregion
}
