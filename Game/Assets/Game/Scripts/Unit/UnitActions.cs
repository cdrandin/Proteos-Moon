using UnityEngine;
using System.Collections;

/*
 * TODO: - Make it a system of its own so this script isn't needed to be attached to each GameObject (STANDALONE script)
 *       - Entity of its own to faciliate objects
 */

public class UnitActions : MonoBehaviour {

	public static bool isInRange;
	private Transform myTransform;
	Renderer myRenderer;
	float sightRange;
	void Awake(){}
	// Use this for initialization
	void Start () {
		myRenderer = this.GetComponentInChildren<Transform>().GetComponentInChildren<Renderer>();
		myTransform = this.GetComponent<Transform>();	
		sightRange = this.gameObject.GetComponent<FOWUnitRevealer>().range.y;
	}
	
	public static bool IsUnitInRange(){
	
		return isInRange;
	}
	
	// Update is called once per frame
	void Update () {}
	
	public void TurnOnHighlight(){
		
		if(isInRange){
			myRenderer.material.shader = Shader.Find("Outlined/Diffuse");
			myRenderer.material.SetColor ("_OutlineColor", Color.red);
			
		}
		if( !isInRange	){
		
			myRenderer.material.shader = Shader.Find("Diffuse Detail");
			
		}
	}
	
	public void TurnOffHighlight(){
	
		myRenderer.material.shader = Shader.Find("Diffuse Detail");
		
	}
	
	public void WithinAttackRange(GameObject currentFocus){
		if( Vector3.SqrMagnitude(currentFocus.transform.position - myTransform.position )
			< currentFocus.GetComponent<BaseClass>().attack_range * currentFocus.GetComponent<BaseClass>().attack_range ){
		
		
			isInRange = true;
		}
		else{
		
			isInRange = false;
		}	
	}
	
	public void WithinSightRange(GameObject currentFocus){
	
		if( Vector3.SqrMagnitude(currentFocus.transform.position - myTransform.position )
		   < sightRange * sightRange ){
			
			
			isInRange = true;
		}
		else{
			
			isInRange = false;
		}	
		
	
	
	}
}
