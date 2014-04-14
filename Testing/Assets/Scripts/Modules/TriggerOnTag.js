
#pragma strict

public var triggerTag : String = "Player";
public var enterSignals : SignalSender;
public var exitSignals : SignalSender;

private var playersInsidearea : List.<Transform> = new List.<Transform>();

function OnTriggerEnter (other : Collider) {
	if (other.isTrigger)
		return;
	
	if (other.gameObject.tag == triggerTag || triggerTag == "") {
		playersInsidearea.Add(other.transform);
		if(playersInsidearea.Count==1)
		    DoEnter();		
	}
}

function DoEnter(){
    enterSignals.SendSignals (this);
}
function DoExit(){
    exitSignals.SendSignals (this);
}

function OnTriggerExit (other : Collider) {
	if (other.isTrigger)
		return;
	
	if (other.gameObject.tag == triggerTag || triggerTag == "") {
		playersInsidearea.Remove(other.transform);
		
		if(playersInsidearea.Count<1)
		    DoExit();
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
		DoExit();
}
