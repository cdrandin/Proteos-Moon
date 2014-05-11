/*
 * UnitController.cs
 * 
 * Christopher Randin
 */
using UnityEngine;
using System.Collections;

public class UnitNetworking : MonoBehaviour
{
	private PhotonView _my_photon_view;
	
	// Use this for initialization
	void Start ()
	{
		_my_photon_view = this.gameObject.GetPhotonView();
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