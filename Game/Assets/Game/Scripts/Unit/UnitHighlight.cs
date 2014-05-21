using UnityEngine;
using System.Collections;

public class UnitHighlight : MonoBehaviour {
	
	private Renderer rend;
	private BaseClass baseClass;
	private bool IsMine;
	
	
	// Use this for initialization
	void Start () {
		
		rend = this.GetComponentInChildren<Transform>().GetComponentInChildren<Renderer>();
		baseClass = this.gameObject.GetComponent<BaseClass>();
		IsMine = this.gameObject.GetPhotonView().isMine	;	
		
	}	
		
	// Update is called once per frame
	void Update () {}
	
	public void OnMouseOver()
	{
	
			if(!UnitGUI.instance.isInitialize && !baseClass.unit_status.status.Rest && GM.instance.IsItMyTurn() && IsMine)
			{
				
				rend.material.shader = Shader.Find("Outlined/Diffuse");
				rend.material.SetColor ("_OutlineColor", Color.blue);
			}
	}
	
	
	
	public void RestingUnitGrayOut(){
		
		rend.material.shader = Shader.Find("Diffuse");
	}
	
	public void CleanUnit(){
	
		rend.material.shader = Shader.Find("Diffuse Detail");
	}
	
	public void OnMouseExit()
	{
		
		if(!baseClass.unit_status.status.Rest && GM.instance.IsItMyTurn())
			rend.material.shader = Shader.Find("Diffuse Detail");
				
	}
}
