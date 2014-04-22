using UnityEngine;
using System.Collections;

[System.Serializable]
public enum Leader_Names
{
	Altier_Seita, 
	Captain_Mena
}

public class LeaderObjects : MonoBehaviour 
{
	private static LeaderObjects _instance;

	[SerializeField]
	private GameObject altier_seita;

	[SerializeField]
	private GameObject captain_mena;

	public static LeaderObjects instance
	{
		get { return _instance; }
	}


	public GameObject Altier_Seita
	{
		get { return altier_seita; }
	}

	public GameObject Captain_Mena
	{
		get { return captain_mena; }
	}

	void Awake ()
	{
		_instance = this;
	}
}
