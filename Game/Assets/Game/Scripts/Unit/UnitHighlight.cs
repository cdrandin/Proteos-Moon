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
	
	
		foreach(Component child in this.transform)
		{
			print (child);
			Renderer child_render = child.GetComponent<Renderer>();
			if(child_render != null)
			{
				child_render.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
				print ("Hovered over");
			}
		}
		/*
		foreach (var r in renderers)
		{
			// Do something with the renderer here...
			r.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse"); // like disable it for example. 
			print ("Hovered over");
		}
		*/
		//renderer.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
	}
	
	void OnMouseExit()
	{
		foreach(Transform child in this.transform)
		{
			Renderer child_render = child.GetComponent<Renderer>();
            if(child_render != null)
            {
						child_render.material.shader = Shader.Find("Diffuse");
			}
		}
		/*
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		foreach (var r in renderers)
		{
			renderer.material.shader = Shader.Find("Diffuse"); 
			print ("Mouse exit");
		}
		*/
	}
}
