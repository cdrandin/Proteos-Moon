using UnityEngine;
using System.Collections;

public class CombatFocusTest : MonoBehaviour {


	public GameObject Target;
	Vector3 direction;
	Vector3 attacker;
	Vector3 enemy;
	
	Renderer rend;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.Space)){
			this.gameObject.GetComponent<BaseClass>().unit_status.Rest();
			this.gameObject.GetComponent<UnitHighlight>().RestingUnitGrayOut();
		}
	}

}
