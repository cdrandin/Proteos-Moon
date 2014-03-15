using UnityEngine;
using System.Collections;

public class FogOfWarPlayer : MonoBehaviour {

	public Transform FogOfWarPlane;
	private GameObject Leader1, Leader2;
	// Use this for initialization
	void Awake () {
		Leader1 = GameObject.Find ("Leader1");
		Leader2 = GameObject.Find ("Leader2");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newLeaderPos1 = Leader1.transform.position;
		Vector3 newLeaderPos2 = Leader2.transform.position;
		FogOfWarPlane.GetComponent<Renderer>().material.SetVector("_Leader1_Pos", newLeaderPos1);
		FogOfWarPlane.GetComponent<Renderer>().material.SetVector("_Leader2_Pos", newLeaderPos2);

	}
}
