using UnityEngine;
using System.Collections;

/*
 * TODO: - Make it a system of its own so this script isn't needed to be attached to each GameObject (STANDALONE script)
 *       - Entity of its own to faciliate objects
 */

public class UnitSelected : MonoBehaviour {

	private bool isSelected;
	private GameObject SelectedProjector;

	
	void Awake(){
	
		SelectedProjector = GameObject.Find ("SelectProjector");
		Mouse.OnClick += this.OnClick;
	
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(isSelected){
		
			SelectedProjector.transform.position = gameObject.transform.position;
		
		}
			
	}
	
	public void OnClick(GameObject currentGameObject){
		if(currentGameObject == gameObject){
			
			isSelected = true;
			
			if(SelectedProjector.activeSelf == false)
				SelectedProjector.SetActive(true);
	
			SelectedProjector.transform.position = gameObject.transform.position;
		}
		else {
			isSelected = false;
		}
		
	}
}
