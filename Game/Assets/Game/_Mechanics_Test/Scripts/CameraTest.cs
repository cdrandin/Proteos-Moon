using UnityEngine;
using System.Collections;

/*
 * This class will allow the user to change the camera position of their units
 * 
 * 
 * 
 * */


public class CameraTest : MonoBehaviour {

	private GameObject[] game_array;

	public Vector3 distance_to_unit;

	private Vector3 new_position;
	private Quaternion newRot;

	public float smooth = 1f, rotation_smooth = 1f;

	private int index = 0;
	// Use this for initialization

	void Start () {


	}

	void Awake (){

		game_array = GameObject.FindGameObjectsWithTag ("Unit");
		index = 0;
		transform.LookAt (game_array [index].transform.position);




	}


	// Update is called once per frame
	void Update () {
	
		bool cameraOn = true;

		if (cameraOn) {

			bool dirty = false;
			int previous_index = index;

			// Changing Index to scroll through units
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {

					dirty = true;
					index -= 1;
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {

					dirty = true;
					index += 1; 
			
			}
			//Input keys were called
			if (dirty) {
					
					//These conditions choose which model to put the camera focus on
					distance_to_unit = GetVectorDistanceFromUnit(previous_index);


					//Fixes the index to prevent segment fault
					if (index <= -1) {

							index = game_array.Length - 1;

					} else if (index > game_array.Length - 1) {

							index = 0;
					}					
					//Find the new position
					new_position = game_array [index].transform.position + distance_to_unit;
					newRot = Quaternion.LookRotation (new_position);

			}

			//TODO - Add mouse movement
			//Functionality to work on
			//Mouse scrolling to zoom in and out
			//Mouse hitting edge of the screen to move the camera view
			//   ^^ Make sure that the camera is parallel to the plane of the game.
			InterpolateToNewPosition(index, new_position);
		}
	}

	Vector3 GetVectorDistanceFromUnit(int previous_index){

		//If the user changes the distance of the camera, but stays focus on the unit
		//keep the same vector distance.
		if (game_array [previous_index].transform.position == transform.position) {
			return transform.position - game_array [previous_index].transform.position;
		}
		//If not then use the default vector distance
		else {
			Vector3 default_vector = new Vector3(-5, 5, -5);
			return default_vector;
		}
	}

	void InterpolateToNewPosition(int index, Vector3 new_position){

		transform.position = Vector3.Lerp (transform.position, new_position, Time.deltaTime);
		transform.LookAt (game_array [index].transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, newRot, Time.deltaTime * 0.001f);

	}
}
