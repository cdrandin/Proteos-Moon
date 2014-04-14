#pragma strict

public var mouseDownSignals : SignalSender;
public var mouseUpSignals : SignalSender;

private var state : boolean = false;

#if UNITY_IPHONE || UNITY_ANDROID
private var joysticks : Joystick[] = new Joystick[0];
#endif
function Start () {
    yield  0; //wait for joystick setup
	#if UNITY_IPHONE || UNITY_ANDROID
	joysticks = FindObjectsOfType (Joystick) as Joystick[];	
#endif

	if(!(GetComponent("PhotonView") as PhotonView).isMine)
		enabled=false;
}




function Update () {
	
	
#if UNITY_IPHONE || UNITY_ANDROID
	if (state == false && joysticks[0].tapCount > 0) {
		mouseDownSignals.SendSignals (this);
		state = true;
	}
	else if (joysticks[0].tapCount <= 0) {
		mouseUpSignals.SendSignals (this);
		state = false;
	}	
#else	
	#if !UNITY_EDITOR && (UNITY_XBOX360 || UNITY_PS3)
		// On consoles use the right trigger to fire
		var fireAxis : float = Input.GetAxis("TriggerFire");
		if (state == false && fireAxis >= 0.2) {
			mouseDownSignals.SendSignals (this);
			state = true;
		}
		else if (state == true && fireAxis < 0.2) {
			mouseUpSignals.SendSignals (this);
			state = false;
		}
	#else
		if (state == false && Input.GetMouseButtonDown (0) && GameManager.GameCanClickHere() ) {
			mouseDownSignals.SendSignals (this);
			state = true;
		}
		
		else if (state == true && Input.GetMouseButtonUp (0)) {
			mouseUpSignals.SendSignals (this);
			state = false;
		}
	#endif
#endif
}
