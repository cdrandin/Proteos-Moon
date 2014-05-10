using UnityEngine;
using System.Collections;

public class CombatFocusTest : MonoBehaviour {


	public GameObject Target;
	Vector3 direction;
	Vector3 attacker;
	Vector3 enemy;
	
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
