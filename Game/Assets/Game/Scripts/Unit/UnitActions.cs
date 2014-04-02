using UnityEngine;
using System.Collections;

/*
 * TODO: - Make it a system of its own so this script isn't needed to be attached to each GameObject (STANDALONE script)
 *       - Entity of its own to faciliate objects
 */

public class UnitActions : MonoBehaviour {

	public static bool isInRange;
	private Transform myTransform;
	
	
	void Awake(){
	
		myTransform = this.GetComponent<Transform>();	
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
//		if(isSelected){
//		
//			SelectedProjector.transform.position = gameObject.transform.position;
//		
//		}
			
	}
	
	public void WithinRange(GameObject currentFocus){
		if( Vector3.SqrMagnitude(currentFocus.transform.position - myTransform.position )
		   < currentFocus.GetComponent<BaseClass>().attack_range * currentFocus.GetComponent<BaseClass>().attack_range){
		   
		   
		   		isInRange = true;
		   }		
	}
}
