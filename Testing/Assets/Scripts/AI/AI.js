#pragma strict

class AI extends Photon.MonoBehaviour {

// Public member data
public var behaviourOnSpotted : MonoBehaviour;
public var soundOnSpotted : AudioClip;
public var behaviourOnLostTrack : MonoBehaviour;

// Private member data
private var character : Transform;
private var insideInterestArea : boolean = true;

function Awake () {          
	character = transform;
        
}

function OnEnable () {
	behaviourOnLostTrack.enabled = true;
	behaviourOnSpotted.enabled = false;
}

function OnTriggerEnter (other : Collider) {
	if (other.tag == "Player" && CanSeePlayer ()) {
		OnSpotted ();
	}
}

function OnEnterInterestArea () {
	insideInterestArea = true;
}

function OnExitInterestArea () {
	insideInterestArea = false;
	OnLostTrack ();
}

function OnSpotted () {
	if (!insideInterestArea)
		return;
	if (!behaviourOnSpotted.enabled) {
		behaviourOnSpotted.enabled = true;
		behaviourOnLostTrack.enabled = false;
		
		if (audio && soundOnSpotted) {
			audio.clip = soundOnSpotted;
			audio.Play ();
		}
	}
}

function OnLostTrack () {
    //if(photonView.isMine){
    //    photonView.RPC("OnLostTrack",  PhotonTargets.Others);
    //}
	if (!behaviourOnLostTrack.enabled) {
		behaviourOnLostTrack.enabled = true;
		behaviourOnSpotted.enabled = false;
	}
}

function CanSeePlayer () : boolean {
	var player : Transform = GameManager.GetClosestPlayer(transform.position);
	if(player == null){
		Debug.LogError("No player!");
		return false;
	}
	
	var playerDirection : Vector3 = (player.position - character.position);
	var hit : RaycastHit;
	Physics.Raycast (character.position, playerDirection, hit, playerDirection.magnitude);	
	if (hit.collider && hit.collider.transform == player) {
		return true;
	}
	return false;
}

}