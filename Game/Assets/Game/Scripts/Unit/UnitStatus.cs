using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UnitSelected))]
public class UnitStatus : MonoBehaviour {
	public Status status;

	// Use this for initialization
	void Start () {
		status = Status.Clean;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
