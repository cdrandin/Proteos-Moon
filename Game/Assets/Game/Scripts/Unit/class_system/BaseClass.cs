using UnityEngine;
using System.Collections;
using System;

public class BaseClass : MonoBehaviour
{
	private BaseStat[] _base_stat;
	private Vital[] _vital;

	void Awake()
	{
		_base_stat = new BaseStat[Enum.GetValues(typeof(BaseStat)).Length];
		_vital = new Vital[Enum.GetValues(typeof(Vital)).Length];
	}

	public void BaseStatsInit()
	{
		for(int i=0;i<_base_stat.Length;++i)
			_base_stat[i] = new BaseStat();
	}

	public void VitalsInit()
	{
		for(int i=0;i<_vital.Length;++i)
			_vital[i] = new Vital();
	}

	public void SetUpBaseStats(params byte[] list)
	{
		if(list.Length != _base_stat.Length)
		{
			Debug.LogError("SetUpBaseStats(): The number of values does not match the number of _base_stat arguments.");
			return;
		}

		// Given a number of arguments, plug them into the BaseStat list
		byte i=0;
		foreach(BaseStat.StatName stat in Enum.GetValues(typeof(BaseStat.StatName)))
		{
			GetBaseStat(stat).Value = list[i];
			++i;
		}
	}

	public void SetUpVital(params byte[] list)
	{
		if(list.Length != _vital.Length)
		{
			Debug.LogError("SetUpVital(): The number of values does not match the number of _vital arguments.");
			return;
		}
		
		// Given a number of arguments, plug them into the Vital list
		byte i=0;
		foreach(Vital.VitalName v in Enum.GetValues(typeof(Vital.VitalName)))
		{
			GetVital(v).Value = list[i];
			++i;
		}
	}

	public BaseStat GetBaseStat(BaseStat.StatName stat)
	{
		return _base_stat[(int)stat];
	}

	public Vital GetVital(Vital.VitalName v)
	{
		return _vital[(int)v];
	}

}
