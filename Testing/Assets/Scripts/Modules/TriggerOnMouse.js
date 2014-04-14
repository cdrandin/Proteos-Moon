#pragma strict

public var mouseDownSignals : SignalSender;
public var mouseUpSignals : SignalSender;

function Update () {
	if (Input.GetMouseButtonDown(0) && GameManager.GameCanClickHere())
		mouseDownSignals.SendSignals (this);
	
	if (Input.GetMouseButtonUp(0))
		mouseUpSignals.SendSignals (this);
}
