using UnityEngine;
using System.Collections;

public class UnitHighlight : MonoBehaviour {
	
	Renderer rend;
	
	// Use this for initialization
	void Start () {
		
		rend = this.GetComponentInChildren<Transform>().GetComponentInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseOver()
	{
		rend.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
		rend.material.SetColor ("_OutlineColor", Color.blue);
	}
	
	void OnMouseExit()
	{
		rend.material.shader = Shader.Find("Diffuse Detail");
	}
}
