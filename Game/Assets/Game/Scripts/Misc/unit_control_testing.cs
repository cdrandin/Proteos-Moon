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
			if(GUI.Button(new Rect(200 + 100*i,100,100,50), string.Format("Unit{0}", i+1)))
			{
				if(GameManager.IsOn())
					GameManager.SetUnitControllerActiveOn(_units[i]);
			}
		}

		if(GUI.Button(new Rect(100,200,100,50), "Unfocus"))
		{
			if(GameManager.IsOn())
				GameManager.SetUnitControllerActiveOff();
		}
	}
}
