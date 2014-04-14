#pragma strict
#pragma downcast
import System.Collections.Generic;

public var affected : List.<GameObject> = new List.<GameObject> ();
private var playersInsidearea : List.<Transform> = new List.<Transform>();


function Awake(){
	ActivateAffected (false);
}
function OnTriggerEnter (other : Collider) {
	if (other.tag == "Player"){
		ActivateAffected (true);
		playersInsidearea.Add(other.transform);
	}
		
}

function OnTriggerExit (other : Collider) {
	if (other.tag == "Player"){	
		playersInsidearea.Remove(other.transform);
		
		//Check if ALL players have left
		if(playersInsidearea.Count<1)
		    ActivateAffected (false);


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
		    ActivateAffected(false);
}


function ActivateAffected (state : boolean) {
	for (var go : GameObject in affected) {
		if (go == null)
			continue;
		go.SetActiveRecursively (state);
		yield;
	}
	for (var tr : Transform in transform) {
		tr.gameObject.SetActiveRecursively (state);
		yield;
	}
}
