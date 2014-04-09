using UnityEngine;
using System.Collections;

/*
 * TODO: - Make it a system of its own so this script isn't needed to be attached to each GameObject (STANDALONE script)
 *       - Entity of its own to faciliate objects
 */

public class UnitActions : MonoBehaviour {

	public static bool isInRange;
	private Transform myTransform;
	GameObject enemyProjector;
	
	void Awake(){
	
		myTransform = this.GetComponent<Transform>();	
		
	}
	// Use this for initialization
	void Start () {
		enemyProjector = GameObject.Find("EnemyProjector");
		
	}
	
	public static bool IsUnitInRange(){
	
		return isInRange;
	}
	
	// Update is called once per frame
	void Update () {
		
//		if(isSelected){
//		
//			SelectedProjector.transform.position = gameObject.transform.position;
//		
//		}
			
	}
	
	public void TurnOnProjector(){
	
		if(isInRange){
			enemyProjector.transform.position = new Vector3(this.transform.position.x ,this.transform.position.y + 10, this.transform.position.z)  ;
			enemyProjector.GetComponent<Projector>().enabled = true;
		
		}
		if( enemyProjector.GetComponent<Projector>().enabled && !isInRange	){
		
			enemyProjector.GetComponent<Projector>().enabled = false;
			
		}
	}
	
	public void WithinRange(GameObject currentFocus){

		if( Vector3.SqrMagnitude(currentFocus.transform.position - myTransform.position )
			< currentFocus.GetComponent<BaseClass>().attack_range * currentFocus.GetComponent<BaseClass>().attack_range ){
		
		
			isInRange = true;
		}
		else{
		
			isInRange = false;
		}	
	}
	
	// TODO: Create a function that will move a projector ontop of the current unit if isInRange is true
}
