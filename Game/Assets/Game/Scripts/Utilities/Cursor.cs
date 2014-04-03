using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour 
{
	public Texture mouse_up ;
	public Texture mouse_down ;

	private Texture _current;
	
	// Use this for initialization
	void Start () 
	{
		Screen.showCursor = false;
		_current = mouse_up;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
			_current = mouse_down;
		else if(Input.GetMouseButtonUp(0))
			_current = mouse_up;
	}


	void OnGUI()
	{
		Vector3 mousePos = Input.mousePosition;
		Rect pos = new Rect(mousePos.x, Screen.height - mousePos.y, _current.width, _current.height);

		GUI.Label(pos, _current);
	}
}
