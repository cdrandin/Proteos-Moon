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
	
		bool dirty = false;
		int previous_index = index;

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {

			dirty = true;
			index -= 1;
		}
		else if (Input.GetKeyDown (KeyCode.RightArrow)) {

			dirty = true;
			index += 1; 
		}

		if (dirty) {

			if (game_array [previous_index].transform.position == transform.position) {

					distance_to_unit = transform.position - game_array [previous_index].transform.position;
			} else {

					distance_to_unit = new Vector3 (-5, 5, -5);
			}

			if (index <= -1) {

					index = game_array.Length - 1;

			} else if (index > game_array.Length - 1) {

					index = 0;
			}
			Debug.Log (index);
			Vector3 unit_position = game_array [index].transform.position;

			new_position = unit_position + distance_to_unit;

			newRot = Quaternion.LookRotation(new_position);

			}

		transform.position =  Vector3.Lerp(transform.position, new_position, Time.deltaTime  );
		transform.LookAt(game_array[index].transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * 0.001f);


	}

}
