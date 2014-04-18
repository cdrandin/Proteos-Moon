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
	
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach (var r in renderers)
		{
			// Do something with the renderer here...
			r.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse"); // like disable it for example. 
		}
		
		//renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
	}
	
	void OnMouseExit()
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach (var r in renderers)
		{
			r.material.shader = Shader.Find("Diffuse"); 
		}
		
	}
}
