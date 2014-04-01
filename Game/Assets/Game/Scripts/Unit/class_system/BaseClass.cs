using UnityEngine;
using System.Collections;
using System;

public class BaseClass : MonoBehaviour
{
	private BaseStat[] _base_stat;
	private Vital[] _vital;

	void Awake()
	{

		_base_stat = new BaseStat[Enum.GetValues(typeof(BaseStat.StatName)).Length];
		_vital = new Vital[Enum.GetValues(typeof(Vital.VitalName)).Length];

		BaseStatsInit(100, 100, 100, 100, 100, 100);
		VitalsInit(1000.0f, 100.0f);

		SetUpBaseStatsValues(100, 20, 5, 10, 30, 30);
		SetUpVitalValues(1000.0f, 100.0f);

		/*
		foreach(string vital in GetVitalNames())
		{
			Debug.Log(string.Format("Name: {0}    Value: {1}", vital, (int)Enum.Parse((typeof(Vital.VitalName)), vital)));
		}
		*/
	}

	public BaseStat[] base_stats
	{
		get{ return _base_stat; }
	}

	public Vital[] vitals
	{
		get{ return _vital; }
	}

	/// <summary>
	/// Max values for BaseStat is the default=Byte.MaxValue.
	/// </summary>
	public void BaseStatsInit()
	{
		for(int i=0;i<_base_stat.Length;++i)
			_base_stat[i] = new BaseStat(Byte.MaxValue);
	}

	/// <summary>
	/// Max values for BaseStat.
	/// max_values[6] = {stamina, strength, intellect, recovery, physical defense, magical defense}
	/// </summary>
	/// <param name="max_values">Max_values.</param>
	public void BaseStatsInit(params byte[] max_values)
	{
		if(max_values.Length != _base_stat.Length)
		{
			Debug.LogError("BaseStatsInit(): The number of values does not match the number of _base_stat arguments.");
			return;
		}

		BaseStatsInit();

		/* list currently contains:
		 * 
		 * Stamina,
		 * Strength,
		 * Intellect, 
		 * Recovery,
		 * Physical_Defense,
		 * Magical_Defense
		 */
		// Given a number of arguments, plug them into the BaseStat list
		byte i=0;
		foreach(BaseStat.StatName stat in Enum.GetValues(typeof(BaseStat.StatName)))
		{
			GetBaseStat(stat).Max = max_values[i];
			++i;
		}
	}

	/// <summary>
	/// Max values for Vital is the default=9999.99f.
	/// </summary>
	public void VitalsInit()
	{
		for(int i=0;i<_vital.Length;++i)
			_vital[i] = new Vital(9999.99f);
	}

	/// <summary>
	/// Max values for Vital.
	/// max_values[2] = {hp, exhaust}
	/// </summary>
	/// <param name="max_values">Max_values.</param>
	public void VitalsInit(params float[] max_values)
	{
		if(max_values.Length != _vital.Length)
		{
			Debug.LogError("VitalsInit(): The number of values does not match the number of _base_stat arguments.");
			return;
		}

		VitalsInit();

		/* list currently contains:
		 * 
		 * Hp,
		 * Exhaust
		 */
		// Given a number of arguments, plug them into the Vital list
		byte i=0;
		foreach(Vital.VitalName v in Enum.GetValues(typeof(Vital.VitalName)))
		{
			GetVital(v).Max = max_values[i];
			++i;
		}
	}

	/// <summary>
	/// Sets up base stats values by the list of stats: 
	/// {stamina, strength, intellect, recovery, physical defense, magical defense}
	/// </summary>
	/// <param name="list">List.</param>
	public void SetUpBaseStatsValues(params byte[] list)
	{
		if(list.Length != _base_stat.Length)
		{
			Debug.LogError("SetUpBaseStats(): The number of values does not match the number of _base_stat arguments.");
			return;
		}

		/* list currently contains:
		 * 
		 * Stamina,
		 * Strength,
		 * Intellect, 
		 * Recovery,
		 * Physical_Defense,
		 * Magical_Defense
		 */
		// Given a number of arguments, plug them into the BaseStat list
		byte i=0;
		foreach(BaseStat.StatName stat in Enum.GetValues(typeof(BaseStat.StatName)))
		{
			GetBaseStat(stat).Value = list[i];
			++i;
		}
	}

	/// <summary>
	/// Sets up vital values by the list of vitals: 
	/// {hp, exhaust}
	/// </summary>
	/// <param name="list">List.</param>
	public void SetUpVitalValues(params float[] list)
	{
		if(list.Length != _vital.Length)
		{
			Debug.LogError("SetUpVital(): The number of values does not match the number of _vital arguments.");
			return;
		}

		/* list currently contains:
		 * 
		 * Hp,
		 * Exhaust
		 */
		// Given a number of arguments, plug them into the Vital list
		byte i=0;
		foreach(Vital.VitalName v in Enum.GetValues(typeof(Vital.VitalName)))
		{
			GetVital(v).Value = list[i];
			++i;
		}
	}

	/// <summary>
	/// Gets the base stat.
	/// </summary>
	/// <returns>The base stat.</returns>
	/// <param name="stat">Stat.</param>
	public BaseStat GetBaseStat(BaseStat.StatName stat)
	{
		return _base_stat[(int)stat];
	}

	/// <summary>
	/// Gets the vital.
	/// </summary>
	/// <returns>The vital.</returns>
	/// <param name="v">V.</param>
	public Vital GetVital(Vital.VitalName v)
	{
		return _vital[(int)v];
	}

	public string[] GetVitalNames()
	{
		return Enum.GetNames((typeof(Vital.VitalName)));
	}

	public string[] GetStatNames()
	{
		return Enum.GetNames((typeof(BaseStat.StatName)));
	}
}
