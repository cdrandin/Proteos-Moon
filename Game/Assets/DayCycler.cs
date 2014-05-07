using UnityEngine;
using System.Collections;

public class DayCycler : MonoBehaviour 
{
	private TOD_Sky sky;

	// Update is called once per frame
	void Update () 
	{
		if(GM.instance.IsOn)
		{
			if(sky == null)
				sky = GameObject.FindGameObjectWithTag("SkyDome").GetComponent<TOD_Sky>();

			sky.Cycle.Hour = (2*((float)GM.instance.CurrentRound/12.0f))%24.0f;
			Debug.Log("DayCycler: Current round: " + GM.instance.CurrentRound);
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
