/*
(* GameManager.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

public class DayCycler : MonoBehaviour 
{
	public int round_per_day = 12;
	public float day_period  = 30.0f;

	private float current_hour_cap;

	private TOD_Sky sky;
	private float _current_time;

	void Start ()
	{
		_current_time = Time.fixedTime;
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if(GM.instance.IsOn)
		{
			if(sky == null)
				sky = GameObject.FindGameObjectWithTag("SkyDome").GetComponent<TOD_Sky>();

			sky.Cycle.Hour = Time.time/40.0f;;

			// How much the time should progress for current round
			current_hour_cap = (24.0f*((float)GM.instance.CurrentRound/(float)round_per_day))%24.0f;

			// Change time of day over time
			_current_time += Time.fixedDeltaTime;

			// If we have same cap, don't go over it
			if(sky.Cycle.Hour >= current_hour_cap)
				_current_time = current_hour_cap;
				//_current_time = Time.fixedTime;

			// Update time of day
			sky.Cycle.Hour = Mathf.Clamp((_current_time/day_period)%24.0f, 0, current_hour_cap);
		}
	}

	void OnEnable()
	{
		if(GM.instance.IsOn)
		{
			if (!sky)
			{
				Debug.LogError("Sky instance reference not set. Disabling script.");
				this.enabled = false;
			}
		}
	}
}