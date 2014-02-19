using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {

	// Event Handler
	public delegate void OnClickEvent(GameObject g);
	public static event OnClickEvent OnClick;

	void Update () {
	
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		if( Physics.Raycast(ray, out hit, Mathf.Infinity)){
		
		
			if(Input.GetMouseButtonUp(0)){
					OnClick(hit.transform.gameObject);
			}
		}
	}
}
