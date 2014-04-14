#pragma strict

@script RequireComponent (SphereCollider)

private var target : GameObject;
private var sphereCollider : SphereCollider;
private var activeRadius : float;

private var playersInsidearea : List.<Transform>;


function Awake () {
	target = transform.parent.gameObject;
	sphereCollider = GetComponent.<SphereCollider> ();
	activeRadius = sphereCollider.radius;
	playersInsidearea = new List.<Transform>();
	
	Disable ();
}


function OnTriggerEnter (other : Collider) {
	if (other.tag == "Player" && target.transform.parent == transform) {
		Enable ();
		playersInsidearea.Add(other.transform);
	}
}

function OnTriggerExit (other : Collider) {
	if (other.tag == "Player") {
		playersInsidearea.Remove(other.transform);
		
		//Check if ALL players have left
		if(playersInsidearea.Count<1)
		    Disable();
	}
}
function OnPhotonPlayerDisconnected(player : PhotonPlayer){
 
    for(var entry : Transform in playersInsidearea){
        if(entry==null){
            playersInsidearea.Remove(entry);		
        }
    }
    //Check if ALL players have left
		if(playersInsidearea.Count<1)
		    Disable();
}

function Disable () {
	if(target.active == true){
	    transform.parent = target.transform.parent;
	    target.transform.parent = transform;
	    target.SetActiveRecursively (false);
	    sphereCollider.radius = activeRadius;
	}
}

function Enable () {
    if(target.active == false){
	    target.transform.parent = transform.parent;
	    target.SetActiveRecursively (true);
	    transform.parent = target.transform;
	    sphereCollider.radius = activeRadius * 1.1;
	}

}
