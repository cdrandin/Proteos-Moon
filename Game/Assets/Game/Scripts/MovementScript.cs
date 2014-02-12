using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {
	public int speed = 5;
	public int gravity = 5;
	private CharacterController cc;
	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (networkView.isMine){
			cc.Move(new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime));
		}
		else{
			enabled = false;
		}
	}
}