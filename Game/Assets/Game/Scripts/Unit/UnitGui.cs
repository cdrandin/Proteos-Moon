using UnityEngine;
using System.Collections;

public class UnitGUI : MonoBehaviour {


	#region class_variables	
	private delegate void GUIMethod();
	private GUIMethod gui_method;
	private UnitCost _unit_cost;
	GameObject focusObject;
	private bool init;
	#endregion
	
	// Use this for initialization
	void Start () {
		focusObject = null;
		init = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.IsOn())
		{
			focusObject = GameManager.GetCurrentFocus();
			
			if(!init && focusObject != null){
				this.gui_method += UnitsOptions;
				print ("The focus is not null");
				init = true;
			}
			else if (init && focusObject == null){
				
				this.gui_method -= UnitsOptions;
				print ("The focus is null");
				init = false;
			}
		}
		
	}
	
	void OnGUI(){
	
		if(this.gui_method != null ){
		
			this.gui_method();
		}
	}

	void UnitsOptions(){
	
		if(MakeButton(200,0,"Attack")){
			
			
		}
		else if(MakeButton(200, 50, "Movement")){
			this.gui_method -= UnitsOptions;
			this.gui_method += MovementActive;
		}
		else if(MakeButton(200, 100, "End Phase")){
			
		}
	}
	
	void MovementActive(){
	
		if(MakeButton(200, 0, "End Movement")){
		
			this.gui_method -= MovementActive;
			this.gui_method += UnitsOptions;
		}
		
	}
	
	bool MakeButton(float left, float top, string name){
		return GUI.Button(new Rect(left,top+50, 150,50), name);
	}
}

