/*
 * BaseClass.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

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

	void FixedUpdate()
	{
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
