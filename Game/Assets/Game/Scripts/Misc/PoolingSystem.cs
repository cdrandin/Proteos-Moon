using UnityEngine;
using System.Collections;

public class PoolingSystem : MonoBehaviour
{
	[SerializeField]
	private GameObject _object;

	[SerializeField]
	private int _num_obj;

	// Keep track of _pooled objects
	private GameObject[] _pool;

	// Keep track of which index in the _pool is being used
	private bool[] _in_use;


	/// <summary>
	/// Gets the object in which the pooling system is focusing on
	/// </summary>
	/// <value>The object.</value>
	public GameObject obj
	{
		get { return _object; }
	}

	/// <summary>
	/// Gets the total amount of objects this pooling system should restrict to.
	/// </summary>
	/// <value>The total.</value>
	public int total
	{
		get { return _num_obj; }
	}

	void Awake ()
	{
		_pool   = new GameObject[_num_obj];
		_in_use = new bool[_num_obj];

		for(int i=0;i<_num_obj;++i)
		{
			_pool [i]  = Instantiate(_object, Vector3.zero, Quaternion.identity) as GameObject;
			_in_use[i] = false; 

			// turn off components for each _pooled object
			Shutoff(ref _pool[i]);
		}
	}
	
	void ReadyToDeploy(ref GameObject obj, Vector3 location)
	{
		obj.SetActive(true);
		obj.transform.position = location;
	}

	void Shutoff(ref GameObject obj)
	{
		Debug.Log("shutoff");
		obj.transform.position = new Vector3(9999, 9999, 0);
		obj.SetActive(false);
	}


	// public methods
	public GameObject GetObject(Vector3 location)
	{
		GameObject obj = null; 

		for(int i=0;i<_num_obj;++i)
		{
			if(_in_use[i] == false)
			{
				obj        = _pool[i]; 
				_in_use[i] = true;
				ReadyToDeploy(ref obj, location);
				i          = _num_obj;
			}
		}

		if(obj == null)
		{
			Debug.LogWarning(string.Format("PoolSystem: object -> {0} has met max limit.", obj));
		}

		return obj;
	}

	public void ReturnObject(GameObject obj)
	{
		if(obj.tag == this._object.tag)
		{
			for(int i=0;i<_num_obj;i++)
			{
				if(_in_use[i])
				{
					_pool[i]   = _object; 
					Shutoff(ref _pool[i]);
					_in_use[i] = false; 
					i          = _num_obj;
				}
			}
		}
		else
		{
			Debug.LogError(string.Format("Parameter: {0} is type {1} which is not the same type as '{1}'", obj.name, obj.tag, this._object.tag));
		}
	}
}
