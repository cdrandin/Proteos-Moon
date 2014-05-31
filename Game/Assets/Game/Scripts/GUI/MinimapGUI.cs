using UnityEngine;
using System.Collections;

public class MinimapGUI : MonoBehaviour 
{

	public Texture navigation_icon;
	private Camera _minimap_camera;

	// Use this for initialization
	void Start ()
	{
		_minimap_camera = this.gameObject.camera;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}