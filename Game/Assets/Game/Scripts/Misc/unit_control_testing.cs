using UnityEngine;
using System.Collections;

public class unit_control_testing : MonoBehaviour {

	public GameObject[] _units;

	// Use this for initialization
	void Start () {
		_units = GameObject.FindGameObjectsWithTag("Unit");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		for(int i=0;i<_units.Length;++i)
		{
			if(GUI.Button(new Rect(100 + 100*i,100,100,50), string.Format("Unit{0}", i+1)))
			{
				GameManager.SetUnitControllerActiveOn( _units[i] );
				//GameObject.FindGameObjectWithTag("UnitController").GetComponent<UnitController>().SetFocusOnUnit(_units[i]);
			}
		}

		if(GUI.Button(new Rect(100,200,100,50), "Unfocus"))
		{
			GameObject.FindGameObjectWithTag("UnitController").GetComponent<UnitController>().ClearFocusUnit();
		}
	}
}
