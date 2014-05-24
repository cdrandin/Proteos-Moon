/*
 * UnitController.cs
 * 
 * Christopher Randin
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitNetworking : MonoBehaviour
{
	public struct MovementInfo{
	
		public Quaternion currentRotation;
		public Vector3 currentPosition;
		public bool isInOtherPlayerFOV;
	
	}
	
	public List<MovementInfo> movementList;
	private PhotonView _my_photon_view;
	private AnimationTriggers unitAnim;
	
	// Use this for initialization
	void Start ()
	{
		movementList = new List<MovementInfo>();
		_my_photon_view = this.gameObject.GetPhotonView();
		unitAnim = this.gameObject.GetComponentInChildren<AnimationTriggers>();
	}

	public void UpdateUnitPosition()
	{
		// Get focused object
		if(GM.instance.CurrentFocus == this.gameObject)
		{
			// Send RPC call to update unit's position
			_my_photon_view.RPC("UpdateUnitTransformation", PhotonTargets.OthersBuffered, this.gameObject.transform.position, this.gameObject.transform.rotation);	
		}
	}

	
	//Call this when you are starting to move your character
	public void StartStoringMovements(int whichPlayerAmI)
	{

		StartCoroutine("WhileMoving", (Player)whichPlayerAmI);
	}
	
	//Call this when you are no longer moving your character
	public void StopStoringMovements(int whichPlayerAmI){
	
	
		StopCoroutine("WhileMoving");
		
		//Make sure to store the final position to the movementlist
		GameObject [] enemyList = GM.instance.GetUnitsFromPlayer((Player)whichPlayerAmI);
		_my_photon_view.RPC("AddToMovementList", PhotonTargets.OthersBuffered, this.gameObject.transform.position, this.gameObject.transform.rotation, CanTheOtherPlayerSeeMe(enemyList));	
		_my_photon_view.RPC("StartMovementLocally", PhotonTargets.OthersBuffered);
		
	}
	
	[RPC]
	private void StartMovementLocally(){
	
		StartCoroutine(MoveCharacterLocally());
	}
	
	//This is called locally in the machine based off the 
	private IEnumerator MoveCharacterLocally(){
		PhotonNetwork.isMessageQueueRunning = false;
		bool movementValid = false;
		
		print (movementList.Count);
		
		for(int i = 1; i < movementList.Count - 1 ; ++i){
			
			print ("Before " + Time.time);
			movementValid = movementList[i-1].isInOtherPlayerFOV || movementList[i].isInOtherPlayerFOV;
			transform.position = movementList[i-1].currentPosition;
			transform.rotation = movementList[i-1].currentRotation;
			Vector3 nextPosition = movementList[i].currentPosition;
			Vector3 nextEuler = movementList[i].currentRotation.eulerAngles;
			while(movementValid && !GM.WithinEpsilon(transform.position, nextPosition, 0.01f) 
			      &&  !GM.WithinEpsilon(transform.eulerAngles, nextEuler, 0.01f )){
				
				unitAnim.MoveAnimation(1.0f);
				transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime);
				transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, nextEuler, Time.deltaTime);
				
				yield return null;				
			}
			print ("After " + Time.time);
			
			unitAnim.MoveAnimation(1.0f);
			transform.position = nextPosition	;
			transform.eulerAngles = nextEuler ;
			
		}
		//End of for loop stop movement animation
		unitAnim.MoveAnimation(0.0f);
		PhotonNetwork.isMessageQueueRunning = true;
		
	}
	
	//A coroutine that updates the position the of character
	private IEnumerator WhileMoving(Player ownerOfUnitMoving){
	
		//Clear the list before beginning
		_my_photon_view.RPC("ClearMovementList", PhotonTargets.AllBuffered);
		
		//Get the list of enemies to check to see if they are within their sight range
		GameObject [] enemyList = GM.instance.GetUnitsFromPlayer(ownerOfUnitMoving);
		
		//store the first position to compare the next positions
		Vector3 previousPosition = this.gameObject.transform.position;

		//Add the first position to the list
		_my_photon_view.RPC("AddToMovementList", PhotonTargets.OthersBuffered, this.gameObject.transform.position, this.gameObject.transform.rotation, CanTheOtherPlayerSeeMe(enemyList));	
				
		//the time to check the next frame		
		float delta = 0.25f;
		
		while(true){
		
			//if the next position square magnitude is larger than 1 then store the value
			if (Mathf.Abs( (transform.position.sqrMagnitude - previousPosition.sqrMagnitude) ) > 1.0f){
				
				//update the previous position
				previousPosition = this.gameObject.transform.position;
				
				//Add the new transform to the movement list
				_my_photon_view.RPC("AddToMovementList", PhotonTargets.OthersBuffered, this.gameObject.transform.position, this.gameObject.transform.rotation, CanTheOtherPlayerSeeMe(enemyList));	
				
			}
			
			yield return new WaitForSeconds(delta);
		}
	
	}
	
	//This checks to see if the character is in the sight range
	public int CanTheOtherPlayerSeeMe(GameObject[] enemyList){
	
		//HACK: Since RPC calls cant send bool parameters, we are going to send integers with the values of 0 or 1
	
		for(int i = 0; i < enemyList.Length; ++i){
			
			//the range y value in revealer is the max distance the player can see 
			float sightRange = enemyList[i].GetComponent<FOWUnitRevealer>().range.y;
			//The magnitude between both players
			float sqrMag = Vector3.SqrMagnitude(enemyList[i].transform.position - this.transform.position);
			//Checking to see if they are within the sight range
			if(sightRange * sightRange > sqrMag ){
			
				return 1;
			}
			
		}
		return 0;
	}
	
	//To remove overhead we are using functions within the networking that will send all rpc calls for each individual unit
	
	public void UnitIsIdle(){
		
		_my_photon_view.RPC("UnitIdle", PhotonTargets.AllBuffered);
	}
	
	public void UnitIsReady(){
	
		_my_photon_view.RPC("UnitReady", PhotonTargets.AllBuffered);
	}

	public void UnitIsGathering(Vector3 current_procite_pos){
	
		Quaternion lookat = Quaternion.LookRotation(current_procite_pos);
		lookat.eulerAngles = new Vector3(lookat.eulerAngles.x, this.gameObject.transform.eulerAngles.y, lookat.eulerAngles.z);
		
		// Send updated transformation
		_my_photon_view.RPC("UpdateUnitTransformation", PhotonTargets.AllBuffered, this.gameObject.transform.position, lookat);

		// Send act of doing gathering over network
		_my_photon_view.RPC("UnitGather", PhotonTargets.AllBuffered);
	}

	#region RPC calls
	[RPC]
	void ClearMovementList(){
		
		if (movementList != null)
			movementList.Clear();

	}
	
	
	[RPC]
	void AddToMovementList(Vector3 position, Quaternion rotation, int boolean)
	{
		//Storing the values to its appropiate locations
		MovementInfo newMovementInfo = new MovementInfo();
		newMovementInfo.currentPosition = position;
		newMovementInfo.currentRotation = rotation;
		newMovementInfo.isInOtherPlayerFOV =  (boolean == 1)? true : false ;
		
		movementList.Add(newMovementInfo);
		
	}
	

	// Update the unit's current position. Allowing it to move
	[RPC]
	void UpdateUnitTransformation(Vector3 position, Quaternion rotation)
	{
		// Get the unit in which to move
		this.gameObject.transform.position = position;
		StartCoroutine(SmoothRotation(rotation));

	}
	
	private IEnumerator SmoothRotation(Quaternion rotation){
		
		
		if(this.gameObject.GetComponent<FOWRenderers>().isVisible){
			PhotonNetwork.isMessageQueueRunning = false;	
		
		Transform myTransform = this.gameObject.transform;
		Vector3 finalEuler = rotation.eulerAngles;
		while ( !GM.WithinEpsilon(myTransform.eulerAngles, finalEuler, 0.01f) ){
			
			myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rotation, Time.deltaTime);
			yield return null;
		}
		myTransform.rotation = rotation;
		
		PhotonNetwork.isMessageQueueRunning = true;
		
		}else
			
			this.gameObject.transform.rotation = rotation;
	}
	
	// Put unit in its correct player container
	[RPC]
	void ParentUnitToCurrentPlayerContainer()
	{
		GM.instance.AddUnitToCurrentPlayerContainer(this.gameObject);
	}

	// Update the units status for both players.
	[RPC]
	public void UpdateUnitStatus(Status status)
	{
		this.gameObject.GetComponent<BaseClass>().unit_status.status = status;
		if(status.Dead)
		{
			_my_photon_view.RPC("UnitDead", PhotonTargets.AllBuffered);
		}
	}

	/// <summary>
	/// Deals the damage to the "victim".
	/// </summary>
	/// <param name="victim_id">Victim_id.</param>
	/// <param name="inc_damage">Inc_damage.</param>
	[RPC]
	public void DealDamage(float inc_damage)
	{
		BaseClass unit = this.gameObject.GetComponent<BaseClass>();
		unit.DealDamage(inc_damage);
		print ("increment damage " + inc_damage);
	}
	
	[RPC]
	public void DamageAnimation(float inc_damage){
	
		BaseClass unit = this.gameObject.GetComponent<BaseClass>();
		
		int newHealth = 0;
		
		if( unit.vital.HP.current - inc_damage > 0) {
		
			newHealth = (int)(unit.vital.HP.current - inc_damage);
		
		}
		
		unitAnim.DamageAnimation(newHealth);
	
	}

	[RPC]
	void UnitReady()
	{
		unitAnim.ReadyAnimation();
	}
	
	[RPC]
	void UnitIdle()
	{
		unitAnim.IdleAnimation();
	}
	
	[RPC]
	void UnitGather()
	{
		BaseClass unit = this.gameObject.GetComponent<BaseClass>();
		GM.instance.AddResourcesToCurrentPlayer(unit.gather_amount);
		unitAnim.GatherAnimation();
		unit.unit_status.Gather();
	}

	[RPC] 
	void UnitDead()
	{
		GM.instance.UnitDied(this.gameObject);
	}
	
	#endregion
}