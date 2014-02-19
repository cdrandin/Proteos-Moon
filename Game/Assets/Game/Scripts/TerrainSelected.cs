using UnityEngine;
using System.Collections;

public class TerrainSelected : MonoBehaviour {

	// Use this for initialization

	private GameObject SelectedProjector;
	
	
	void Awake(){
		
		SelectedProjector = GameObject.Find ("SelectProjector");
		Mouse.OnClick += this.OnClick;
		
	}
	
	void Start(){
	
		SelectedProjector.SetActive(false);
	}
			
	public void OnClick(GameObject g){
		if(g.tag == "Terrain"){
			SelectedProjector.SetActive(false);		
		}
	
	}
}
