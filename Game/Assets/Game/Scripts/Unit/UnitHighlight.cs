using UnityEngine;
using System.Collections;

public class UnitHighlight : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseOver() {
	
		TraverseOutline(this.gameObject);
	

	}
	
	void OnMouseExit()
	{
	
		TraverseDiffuse(this.gameObject);
		
	}
	void TraverseOutline(GameObject obj)
	{
		Renderer obj_renderer = obj.GetComponent<Renderer>() ;
		
		if(obj_renderer != null){
		
			obj_renderer.material.color = Color.yellow;
		}
		
		foreach (Transform child in obj.transform)
		{
			TraverseOutline(child.gameObject);
		}
		
	}
	
	void TraverseDiffuse(GameObject obj)
	{
		Renderer obj_renderer = obj.GetComponent<Renderer>() ;
		
		if(obj_renderer != null){
			
			obj_renderer.material.color = Color.white;
		}
		
		foreach (Transform child in obj.transform)
		{

			TraverseDiffuse(child.gameObject);

		}
		
	}
}
