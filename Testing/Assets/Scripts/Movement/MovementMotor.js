#pragma strict
#pragma downcast
/*
This class can be used like an interface.
Inherit from it to define your own movement motor that can control
the movement of characters, enemies, or other entities.
*/
class MovementMotor extends Photon.MonoBehaviour {

// The direction the character wants to move in, in world space.
// The vector should have a length between 0 and 1.
@HideInInspector
public var movementDirection : Vector3;

// Simpler motors might want to drive movement based on a target purely
@HideInInspector
public var movementTarget : Vector3;

// The direction the character wants to face towards, in world space.
@HideInInspector
public var facingDirection : Vector3;


 function OnPhotonSerializeViewBase (stream : PhotonStream,  info : PhotonMessageInfo)    
    {
        
    	if (stream.isWriting)
        {            
    		//We own this player: send the others our data
    		stream.SendNext (transform.position);
    		stream.SendNext (transform.rotation); 
        }
        else
        {        	
            //Network player, receive data			
            correctPlayerPos = stream.ReceiveNext();
            correctPlayerRot = stream.ReceiveNext();
         }
    }
    
    
    //
    //  TODO: fix constant movement!
    //
    //

    private var correctPlayerPos : Vector3= Vector3.zero; //We lerp towards this
    private var correctPlayerRot : Quaternion = Quaternion.identity; //We lerp towards this

    function Update()
    {		    
        if (!photonView.isMine)
        {
        	if(correctPlayerPos==Vector3.zero) return;
        	
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            if (Vector3.Distance(correctPlayerPos, transform.position) < 4)
            {
                transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
                transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
            }
            else
            {
                 transform.position =  correctPlayerPos;
                 transform.rotation = correctPlayerRot;
            }
        }
    }
    
    
    
 }