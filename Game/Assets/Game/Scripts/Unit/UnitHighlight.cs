using UnityEngine;
using System.Collections;

public class UnitHighlight : MonoBehaviour {
	
	public Renderer rend;
	
	// Use this for initialization
	void Start () {
		
		rend = this.GetComponentInChildren<Transform>().GetComponentInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

	
	}
	
	void OnMouseOver()
	{
		print ("Is this mine? " + this.gameObject.GetPhotonView().isMine);
		print ("Am I not restring? " + !this.gameObject.GetComponent<BaseClass>().unit_status.status.Rest);
		
		if(!UnitGUI.instance.isInitialize && !this.gameObject.GetComponent<BaseClass>().unit_status.status.Rest && this.gameObject.GetPhotonView().isMine)
		{
			
			rend.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
			rend.material.SetColor ("_OutlineColor", Color.blue);
		}
	}
	
	
	public void RestingUnitGrayOut(){
		
		rend.material.shader = Shader.Find("Diffuse");
	}
	
	public void CleanUnit(){
	
		rend.material.shader = Shader.Find("Diffuse Detail");
	}
	
	void OnMouseExit()
	{
		if(!this.gameObject.GetComponent<BaseClass>().unit_status.status.Rest)
			rend.material.shader = Shader.Find("Diffuse Detail");
	}
}
