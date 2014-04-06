/*
 * PoolingSystem.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;

public class PoolingSystem : MonoBehaviour
{
	[SerializeField]
	// Keeping track of which units to create and put into a pool
	private GameObject _object;
	
	// Root container for the spawned objects given in the inspector
	private GameObject _root;
	
	[SerializeField]
	// Number of objects to keep track for in the pool
	private int _num_obj;
	
	// Number of objects currently in use
	private int _num_used;
	
	// Keep track of _pooled objects
	private GameObject[] _pool;
	
	// Keep track of which index in the _pool is being used
	private bool[] _in_use;
	
	// Default vector to move objects away from the scene
	private Vector3 _default;
	
	/// <summary>
	/// Gets the object in which the pooling system is focusing on
	/// </summary>
	/// <value>The object.</value>
	public GameObject pool_obj
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
	
	/// <summary>
	/// Gets the current number of objects in use by the pool system for the specific object
	/// </summary>
	/// <value>The number_in_use.</value>
	public int number_in_use
	{
		get { return _num_used; }
	}
	
	void Awake ()
	{
		_pool      = new GameObject[_num_obj];
		_in_use    = new bool[_num_obj];
		
		_root      = new GameObject();
		_root.name = string.Format("PoolingSystem: {0}", _object.name);
		
		_num_used  = 0;
		
		_default   = new Vector3(9999,9999,9999);
		
		for(int i=0;i<_num_obj;++i)
		{
			// Create specified object
			_pool [i]  = Instantiate(_object, Vector3.zero, Quaternion.identity) as GameObject;
			
			// Mark object not in use
			_in_use[i] = false; 
			
			// Parent the pooled objects into root
			_pool[i].transform.parent = _root.transform;
			
			// Turned off specific components of the object
			Shutoff(ref _pool[i]);
		}
	}
	
	// Just some components to turn on
	void ReadyToDeploy(ref GameObject unit, Vector3 location)
	{
		unit.SetActive(true);
		unit.transform.position = location;
	}
	
	// Just some components to turn off
	void Shutoff(ref GameObject unit)
	{
		unit.transform.position = _default;
		unit.SetActive(false);
	}
	
	/// <summary>
	/// Gets an object of the desired type. If there is an object to spare return the object, else return null.
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="location">Location.</param>
	public GameObject GetObject(Vector3 location)
	{
		GameObject obj = null; 
		
		for(int i = 0; i < _num_obj; ++i)
		{
			if(_in_use[i] == false)
			{
				obj = _pool[i]; 
				_in_use[i] = true;
				_num_used  += 1;
				ReadyToDeploy (ref obj, location);
				i = _num_obj;
			}
		}
		
		if(obj == null)
		{
			Debug.LogWarning(string.Format("PoolSystem: object -> {0} has met max limit.", obj));
		}
		
		return obj;
	}
	
	/// <summary>
	/// Returns the object back to the pooling system. Returns true if it was sucessful in putting it back, else false.
	/// </summary>
	/// <returns><c>true</c>, if object was returned, <c>false</c> otherwise.</returns>
	/// <param name="obj">Object.</param>
	public bool ReturnObject(GameObject obj)
	{
		bool recieved = false;
		
		if(obj.tag == this._object.tag)
		{
			for(int i = 0; i < _num_obj; i++)
			{
				if(_in_use[i])
				{
					_pool[i] = obj; 
					Shutoff (ref _pool[i]);
					_in_use[i] = false; 
					_num_used -= 1;
					i = _num_obj;
					recieved = true;
				}
			}
		}
		else
		{
			Debug.LogError(string.Format("Parameter: {0} is type {1} which is not the same type as '{1}'", obj.name, obj.tag, this._object.tag));
		}
		
		return recieved;
	}
}