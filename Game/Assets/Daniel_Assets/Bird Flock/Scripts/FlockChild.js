/**************************************									
	FlockChild v2.02
	Copyright 2013 Unluck Software	
 	www.chemicalbliss.com								
***************************************/
#pragma strict
#pragma downcast
@HideInInspector public var _spawner:FlockController;
@HideInInspector public var _wayPoint : Vector3;
public var _speed:float= 10;
@HideInInspector public var _dived:boolean =true;
@HideInInspector public var _stuckCounter:float;			//prevents looping around a waypoint
@HideInInspector public var _damping:float;
@HideInInspector public var _soar:boolean = true;
@HideInInspector public var _flatFlyDown:boolean;
@HideInInspector public var _landing:boolean;
@HideInInspector public var _landingSpotted:boolean;
private var _lerpCounter:int;
public var _targetSpeed:float;
@HideInInspector public var _move:boolean = true;

var _model:GameObject;

function Start(){
   Wander(0);
   var sc = Random.Range(_spawner._minScale, _spawner._maxScale);
   transform.localScale=Vector3(sc,sc,sc);
   transform.position = (Random.insideUnitSphere *_spawner._spawnSphere) + _spawner.transform.position;
   transform.position.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight*1.0) +_spawner.transform.position.y;
   if(!_model)_model= transform.FindChild("Model").gameObject;
   for (var state : AnimationState in _model.animation) {
   	 	state.time = Random.value * state.length;
   }
}

function Update() {
   
    if(!_landingSpotted && (transform.position - _wayPoint).magnitude < _spawner._waypointDistance+_stuckCounter){
        Wander(0);	//create a new waypoint
        _stuckCounter=0;
    }else{
    	_stuckCounter+=Time.deltaTime;
    }
    if(_targetSpeed > -1){
    var rotation = Quaternion.LookRotation(_wayPoint - transform.position);
	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _damping);
	}
	if(_spawner._childTriggerPos){
		if((transform.position - _spawner.transform.position).magnitude < 1){
			_spawner.randomPosition();
		}
	}
	_speed = Mathf.Lerp(_speed, _targetSpeed, _lerpCounter *Time.deltaTime*.05);
	_lerpCounter++;
	//Position forward based on object rotation
	if(_move)
	transform.position += transform.TransformDirection(Vector3.forward)*_speed*Time.deltaTime;	
	//Counteract Pitch Rotation When Flying Upwards
	if((_soar && _spawner._flatSoar|| _spawner._flatFly && !_soar)&& _wayPoint.y > transform.position.y||_flatFlyDown)
	_model.transform.localEulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, -transform.localEulerAngles.x, _lerpCounter * Time.deltaTime * .25);
	else
	_model.transform.localEulerAngles.x = Mathf.LerpAngle(_model.transform.localEulerAngles.x, 0, _lerpCounter * Time.deltaTime * .25);
}

function Wander(delay:float){
	yield(WaitForSeconds(delay));
	_damping = Random.Range(_spawner._minDamping, _spawner._maxDamping);       
    _targetSpeed = Random.Range(_spawner._minSpeed, _spawner._maxSpeed);
    _lerpCounter = 0;
    if(_spawner._soarAnimation!=null &&!_flatFlyDown && !_dived && Random.value < _spawner._soarFrequency){
   	 	Soar();
	}else if(!_flatFlyDown && !_dived && Random.value < _spawner._diveFrequency){	
		Dive();
	}else{
		if(!_landing && _model){
		Flap();
		}
	}
}

function Flap(){
	_model.animation.CrossFade(_spawner._flapAnimation, 1.0);
	_soar=false;
	animationSpeed();
	_wayPoint= (Random.insideUnitSphere *_spawner._spawnSphere) + _spawner.transform.position;
	_wayPoint.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight*1.0) +_spawner.transform.position.y;
	_dived = false;
}

function Soar(){
	_model.animation.CrossFade(_spawner._soarAnimation, 1.5);
   	_wayPoint= (Random.insideUnitSphere *_spawner._spawnSphere) + _spawner.transform.position;
	_wayPoint.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight*1.0) +_spawner.transform.position.y;
    _soar = true;
}

function Dive(){
	if(_spawner._soarAnimation!=null){
			_model.animation.CrossFade(_spawner._soarAnimation, 1.5);
		}else{
			for (var state : AnimationState in _model.animation) {
	   	 		if(transform.position.y < _wayPoint.y +25){
	   	 			state.speed = 0.1;
	   	 		}
	   	 	}
   	 	}
   	 	_wayPoint.x = transform.position.x + Random.Range(-1, 1);
    	_wayPoint.z = transform.position.z + Random.Range(-1, 1);
    	_wayPoint.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight) +_spawner.transform.position.y;
    	_wayPoint.y -= _spawner._diveValue;
    	_dived = true;
}

function animationSpeed(){
	for (var state : AnimationState in _model.animation) {
		if(!_dived && !_flatFlyDown){
			state.speed = Random.Range(_spawner._minAnimationSpeed, _spawner._maxAnimationSpeed);
		}else{
			state.speed = _spawner._maxAnimationSpeed;
		}   
	}
}