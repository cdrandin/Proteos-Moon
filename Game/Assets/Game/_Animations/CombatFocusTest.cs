using UnityEngine;
using System.Collections;

public class CombatFocusTest : MonoBehaviour {


	public GameObject Target;
	Vector3 direction;
	Vector3 attacker;
	Vector3 enemy;
	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () {
		
		direction = this.gameObject.transform.forward ;
		attacker = this.gameObject.transform.position;
		enemy = Target.transform.position;
		
		print ("Direction " + direction);
		print ("Target Vector" + (enemy - attacker));
		
		direction.y = 0.0f;
		attacker.y = 0.0f;
		enemy.y = 0.0f;
		
		this.gameObject.transform.rotation = Quaternion.LookRotation((enemy - attacker));			
	}
}
