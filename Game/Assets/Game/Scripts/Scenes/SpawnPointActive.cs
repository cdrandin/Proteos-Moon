using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class SpawnPointActive : MonoBehaviour {
	public bool isActive = false;
	public WaitingRoomScript wrm;
	void Awake(){
		wrm = Camera.main.GetComponent<WaitingRoomScript>();
	}
	// Update is called once per frame
	void Update () {
		if (isActive){
			wrm.gameReady = true;
		}
	}
}
