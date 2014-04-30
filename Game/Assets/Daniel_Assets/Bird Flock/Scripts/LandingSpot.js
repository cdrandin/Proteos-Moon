/**************************************									
	LandingSpot.js v0.21
	Copyright 2013 Unluck Software	
 	www.chemicalbliss.com						
***************************************/
#pragma strict
var landingChild: FlockChild;
@HideInInspector
var landing: boolean;
var _featherPS: GameObject;
private var lerpCounter: int;
private var speed: float = 3;
var _controller: LandingSpotController;
@HideInInspector
var _ruffling: boolean;
@HideInInspector
var _ruffle: boolean;
var _offsetPlatformHeight: float = 1;

function Start() {		
    if (!_controller)
        _controller = transform.parent.GetComponent(LandingSpotController);
    if (_controller._autoCatchDelay.x > 0)
        GetFlockChild(_controller._autoCatchDelay.x, _controller._autoCatchDelay.y);
    if (_controller._ruffleFeatherChance > 0)
        InvokeRepeating("Ruffle", 1.0, 1.0);

}

function Ruffling() {
    if (!_ruffle) {
        var foo: float = Random.Range(.3, .5);
        _ruffle = true;
        lerpCounter = 0;
        transform.position.y += foo;
        if (_controller._randomRotate.x > 0 || _controller._randomRotate.y > 0)
            transform.rotation.eulerAngles.y = Random.Range(_controller._randomRotate.x, _controller._randomRotate.y);
        yield(WaitForSeconds(foo + .5));
        if (_controller._randomRotate.x > 0 || _controller._randomRotate.y > 0)
            transform.rotation.eulerAngles.y = Random.Range(_controller._randomRotate.x, _controller._randomRotate.y);
        lerpCounter = 0;
        transform.position.y -= foo;
        _ruffle = false;
    }
}

function Ruffle() {
    //Debug.Log("Ruffle");
    if (landingChild && landingChild._speed < 1 && _controller._ruffleFeatherChance > Random.value) {
        lerpCounter = 0;
        Ruffling();
        //	landingChild.Flap();
        _ruffling = true;
        RuffleOff();
    }
}

function RuffleOff() {
    yield(WaitForSeconds(0.5));
    _ruffling = false;
}

function OnDrawGizmos() {
	if (!_controller)
        _controller = transform.parent.GetComponent(LandingSpotController);
    
    Gizmos.color = Color.yellow;
    // Draw a yellow cube at the transforms position
    if (landingChild && landing)
        Gizmos.DrawLine(transform.position, landingChild.transform.position);
    if (transform.rotation.eulerAngles.x != 0 || transform.rotation.eulerAngles.z != 0)
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    Gizmos.DrawWireCube(Vector3(transform.position.x, transform.position.y + _offsetPlatformHeight, transform.position.z), Vector3(.2, .2, .2));
    Gizmos.DrawWireCube(transform.position + (transform.forward * .2) + Vector3(0, _offsetPlatformHeight, 0), Vector3(.1, .1, .1));
    Gizmos.color = Color(1, 1, 0, .05);
    Gizmos.DrawWireSphere(transform.position, _controller._maxBirdDistance);
}

function LateUpdate() {
    if (landing && landingChild) {
        var distance: float = Vector3.Distance(landingChild.transform.position, transform.position);

        if (distance < 15 && distance > 7) {
            landingChild._model.animation.CrossFade(landingChild._spawner._soarAnimation, .5);
            landingChild._targetSpeed = 10; //landingChild._spawner._minSpeed;
            landingChild._damping = 3;
            landingChild._wayPoint = transform.position;
            landingChild._wayPoint.x += Random.Range(-5, 5);
            landingChild._wayPoint.z += Random.Range(-5, 5);
        } else if (distance <= 7) {
            landingChild._wayPoint = transform.position;

            //				if(distance> 3){
            //					landingChild._wayPoint.y += 1.5;		
            //				}else{
            if (distance < .1) {
                if (!_ruffling && !_ruffle)
                    landingChild._model.animation.CrossFade(landingChild._spawner._idleAnimation, .55);
            } else {
                landingChild._model.animation.CrossFade(landingChild._spawner._flapAnimation, .2);
                //landingChild._wayPoint.y -= .05;
                landingChild._targetSpeed = -1;
            }
            if (lerpCounter == 0 && !_ruffle) {
                //	Debug.Log("1");
                landingChild._model.animation[landingChild._spawner._idleAnimation].time = Random.value * landingChild._model.animation[landingChild._spawner._idleAnimation].length;
                InvokeRepeating("Ruffle", 1.0, 1.0);
                ReleaseFlockChild(_controller._autoDismountDelay.x, _controller._autoDismountDelay.y);
            }
            //landingChild._landing = true;
            if (distance > .01)
                landingChild.transform.position += (transform.position - landingChild.transform.position) * .018;



            landingChild._move = false;
            landingChild._speed = 0;

            lerpCounter++;
            if (distance < 1) {
                landingChild._targetSpeed = -1;
                landingChild.transform.rotation = Quaternion.Lerp(landingChild.transform.rotation, transform.rotation, lerpCounter * Time.deltaTime * .01);
            }

            //			}
            landingChild._damping = 5;
            //Vector3.Distance(landingChild.transform.position,transform.position)*5;
        } else {
            //Debug.Log("Flap a while");
            landingChild.Flap();
            landingChild._wayPoint = transform.position;
            landingChild._damping = 3;
        }

    } else if (landingChild) {
        //landingChild._targetSpeed = 1;
        landingChild.transform.position.y += speed * Time.deltaTime;
        _featherPS.transform.position = landingChild.transform.position;
    }
}

function GetFlockChild(minDelay: float, maxDelay: float): IEnumerator {
    yield(WaitForSeconds(Random.Range(minDelay, maxDelay)));
    if (!landingChild) {
        var __child: FlockChild;
        if (_controller._randomRotate.x > 0 || _controller._randomRotate.y > 0)
            transform.rotation.eulerAngles.y = Random.Range(_controller._randomRotate.x, _controller._randomRotate.y);
        for (var i: int; i < _controller._flock._roamers.length; i++) {
            var child: FlockChild = _controller._flock._roamers[i] as FlockChild;
            if (!child._flatFlyDown && !child._dived) {
                if (!__child && _controller._maxBirdDistance > Vector3.Distance(child.transform.position, transform.position) && _controller._minBirdDistance < Vector3.Distance(child.transform.position, transform.position)) {
                    __child = child;
                    if (!_controller._takeClosest) break;
                } else if (__child && Vector3.Distance(__child.transform.position, transform.position) > Vector3.Distance(child.transform.position, transform.position)) {
                    __child = child;
                }
            }
        }
        if (__child) {
            landingChild = __child;
            landing = true;
            landingChild._landingSpotted = true;
            landingChild._flatFlyDown = true;
        } else if (_controller._autoCatchDelay.x > 0) {
            GetFlockChild(_controller._autoCatchDelay.x, _controller._autoCatchDelay.y);
        }
    }
}

function InstantLand() {
    if (!landingChild) {
        var __child: FlockChild;
        if (_controller._randomRotate.x > 0 || _controller._randomRotate.y > 0)
            transform.rotation.eulerAngles.y = Random.Range(_controller._randomRotate.x, _controller._randomRotate.y);
        for (var i: int; i < _controller._flock._roamers.length; i++) {
            var child: FlockChild = _controller._flock._roamers[i] as FlockChild;
            if (!child._flatFlyDown && !child._dived) {
                     __child = child;
                
            }
        }
        if (__child) {
            landingChild = __child;
            landing = true;
            landingChild._landingSpotted = true;
            landingChild._flatFlyDown = true;
            landingChild.transform.position = transform.position;
            landingChild._model.animation.Play(landingChild._spawner._idleAnimation);
        } else if (_controller._autoCatchDelay.x > 0) {
            GetFlockChild(_controller._autoCatchDelay.x, _controller._autoCatchDelay.y);
        }
    }
}

function ReleaseFlockChild(minDelay: float, maxDelay: float) {
    yield(WaitForSeconds(Random.Range(minDelay, maxDelay)));
    if (landingChild && landingChild._landingSpotted) {
        lerpCounter = 0;
        if (Random.value > .5)
            _featherPS.transform.particleSystem.Play();
        landingChild._move = true;
        landing = false;
        landingChild._model.animation.CrossFade(landingChild._spawner._flapAnimation, .2);
        landingChild.animationSpeed();
        speed = Random.Range(1.0, 3.0);
        landingChild._landingSpotted = false;
        landingChild._landing = false;
        yield(WaitForSeconds(2));
        if (_controller._autoCatchDelay.x > 0) {
            GetFlockChild(_controller._autoCatchDelay.x, _controller._autoCatchDelay.y);
        }
        _featherPS.transform.particleSystem.Stop();
        landingChild._flatFlyDown = false;
        landingChild = null;
    }
}