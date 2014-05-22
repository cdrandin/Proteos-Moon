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
	
	private struct MovementInfo{
	
		public Transform currentTransform;
		public int isInOtherPlayerFOV;
	
	}
	
	private List<MovementInfo> movementList;
	private FOWRenderers fowRenderer;
	private PhotonView _my_photon_view;
	
	// Use this for initialization
	void Start ()
	{
		_my_photon_view = this.gameObject.GetPhotonView();
		fowRenderer = this.gameObject.GetComponent<FOWRenderers>();
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
	void StartStoringMovements(int whichPlayerAmI)
	{

		StartCoroutine("WhileMoving", (Player)whichPlayerAmI);
	}
	
	//Call this when you are no longer moving your character
	void StopStoringMovements(int whichPlayerAmI){
	
	
		StopCoroutine("WhileMoving");
		
		//Make sure to store the final position to the movementlist
		GameObject [] enemyList = GM.instance.GetUnitsFromPlayer((Player)whichPlayerAmI);
		_my_photon_view.RPC("AddToMovementList", PhotonTargets.OthersBuffered, this.gameObject.transform.position, this.gameObject.transform.rotation, CanTheOtherPlayerSeeMe(enemyList));	
		
	}
	
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
		newMovementInfo.currentTransform.position = position;
		newMovementInfo.currentTransform.rotation = rotation;
		newMovementInfo.isInOtherPlayerFOV = boolean;
		
		movementList.Add(newMovementInfo);
		
	}
	
	//A coroutine that updates the position the of character
	IEnumerator WhileMoving(Player ownerOfUnitMoving){
	
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
	

	// Update the unit's current position. Allowing it to move
	[RPC]
	void UpdateUnitTransformation(Vector3 position, Quaternion rotation)
	{
		if(GM.instance.IsOn)
		{
			// Get the unit in which to move
			this.gameObject.transform.position = position;
			this.gameObject.transform.rotation = rotation;
			this.gameObject.GetComponentInChildren<AnimationTriggers>().MoveAnimation(Input.GetAxis("Vertical"));
		}
	}
	
	// Put unit in its correct player container
	[RPC]
	void ParentUnitToCurrentPlayerContainer()
	{
		if(GM.instance.IsOn)
		{
			GM.instance.AddUnitToCurrentPlayerContainer(this.gameObject);
		}
	}

	// Update the units status for both players.
	[RPC]
	public void UpdateUnitStatus(Status status)
	{
		if(GM.instance.IsOn)
		{
			this.gameObject.GetComponent<BaseClass>().unit_status.status = status;
			if(status.Dead)
			{
				_my_photon_view.RPC("UnitDead", PhotonTargets.AllBuffered);
			}
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
		
		this.gameObject.GetComponentInChildren<AnimationTriggers>().DamageAnimation(newHealth);
	
	}

	[RPC]
	void UnitGather()
	{
		BaseClass unit = this.gameObject.GetComponent<BaseClass>();
		GM.instance.AddResourcesToCurrentPlayer(unit.gather_amount);
		//this.gameObject.GetComponentInChildren<AnimationTriggers>().ForceGatherAnimation();
		this.gameObject.GetComponentInChildren<AnimationTriggers>().GatherAnimation();
		unit.unit_status.Gather();
	}

	[RPC] 
	void UnitDead()
	{
		GM.instance.UnitDied(this.gameObject);
	}
}