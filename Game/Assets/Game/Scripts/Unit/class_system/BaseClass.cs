﻿/*
 * BaseClass.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(UnitActions), typeof(UnitNetworking))]
public class BaseClass : MonoBehaviour 
{
	[SerializeField] 
	private BaseStat _base_stat;

	[SerializeField] 
	private Vital _vital;

	[SerializeField] 
	private MovementStat _movement;

	[SerializeField] 
	private UnitStatus _unit_status;

	[SerializeField]
	private float _attack_range;

	[SerializeField]
	private float _gather_range;

	[SerializeField]
	private int _gather_amount;

	//[SerializeField]
	//private List<StatBuff> _stat_buffs;

	public BaseStat base_stat
	{
		get { return _base_stat; }
	}

	public Vital vital
	{
		get { return _vital; }
	}

	public MovementStat movement
	{
		get { return _movement; }
	}

	public UnitStatus unit_status
	{
		get { return _unit_status; }
	}

	public float attack_range
	{
		get { return _attack_range; }
	}

	public float gather_range
	{
		get { return _gather_range; }
	}

	public int gather_amount
	{
		get { return _gather_amount; }
	}

	/// <summary>
	/// Deals the damage to this selected unit
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void DealDamage(float damage)
	{
		_vital.HP.current -= damage;
	}

	public void BuffStat(BaseStatID stat, int amount)
	{
		byte amnt = (byte)amount;

		switch(stat)
		{
		case BaseStatID.AGILITY:
			_base_stat.Agility.max += amnt;
			_base_stat.Agility.current += amnt;
			break;
		case BaseStatID.INTELLECT:
			_base_stat.Intellect.max += amnt;
			_base_stat.Intellect.current += amnt;
			break;
		case BaseStatID.MAGICAL_DEFENSE:
			_base_stat.Magical_def.max += amnt;
			_base_stat.Magical_def.current += amnt;
			break;
		case BaseStatID.PHYSICAL_DEFENSE:
			_base_stat.Physical_def.max += amnt;
			_base_stat.Physical_def.current += amnt;
			break;
		case BaseStatID.RECOVERY:
			_base_stat.Recovery.max += amnt;
			_base_stat.Recovery.current += amnt;
			break;
		case BaseStatID.STAMINA:
			_base_stat.Stamina.max += amnt;
			_base_stat.Stamina.current += amnt;
			break;
		case BaseStatID.STRENGTH:
			_base_stat.Strength.max += amnt;
			_base_stat.Strength.current += amnt;
			break;
		}
	}

	void FixedUpdate()
	{
		/*
		Status st = _unit_status.status;
		Debug.Log(string.Format(
				"Clean: {0}\nMove: {1}\nAction: {2}\nGather: {3}\nRest: {4}\nDead: {5}",
				st.Clean, st.Move, st.Action, st.Gather, st.Rest, st.Dead
			));
		*/

		if(_vital.IsDead())
		{
			Debug.Log(string.Format("{0} is DEAD!", gameObject.name));
		}
	}

	void OnDrawGizmosSelected()
	{
		// Gather range
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.transform.position, _gather_range - 0.01f);

		// Attack range
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.transform.position, _attack_range + 0.01f);

		// Movement range
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(this.transform.position, _movement.max_distance);
	}
}
