/*
 * PoolingSystem.cs
 * 
 * Christopher Randin
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolingSystem : MonoBehaviour
{
	private static PoolingSystem _instance;

	[SerializeField]
	// Keeping track of which units to create and put into a pool
	private GameObject _object;
	
	// Number of objects to keep track for in the pool
	public int num_obj;

	// Determine if the list is allowed to grow
	public bool expandable;

	// Root container for the spawned objects given in the inspector
	private GameObject _root;

	[SerializeField]
	// Keep track of _pooled objects
	private List<GameObject> _pool;

	/// <summary>
	/// Gets the instance of the script
	/// </summary>
	/// <value>The instance.</value>
	public static PoolingSystem instance
	{
		get { return _instance; }
	}

	/// <summary>
	/// Gets the object in which the pooling system is focusing on
	/// </summary>
	/// <value>The object.</value>
	public GameObject pool_obj
	{
		get { return _object; }
	}

	/// <summary>
	/// Gets the current number of objects in use by the pool system for the specific object
	/// </summary>
	/// <value>The number_in_use.</value>
	public int count
	{
		get { return _pool.Count; }
	}
	
	void Awake()
	{
		_instance  = this;
		_pool      = new List<GameObject>();
		
		_root      = new GameObject();
		_root.name = string.Format("PoolingSystem: {0}", _object.name);
		
		for(int i=0;i<num_obj;++i)
		{
			// Create specified object
			GameObject obj  = Instantiate(_object, Vector3.zero, Quaternion.identity) as GameObject;
			
			// Parent the pooled objects into root
			obj.transform.parent = _root.transform;
			
			obj.SetActive(false);

			// Throw object into pool
			_pool.Add(obj);
		}
	}
	
	/// <summary>
	/// Gets an object of the desired type. If there is an object to spare return the object, else return null.
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="location">Location.</param>
	public GameObject GetObject()
	{
		GameObject obj = null; 
		
		for(int i=0;i<_pool.Count;++i)
		{
			if(!_pool[i].activeInHierarchy)
			{
				obj         = _pool[i]; 
				// stop for loop. Way to prevent branch with break or return
				i           = _pool.Count; 
				obj.SetActive(true);
			}
		}

		if(expandable && obj == null)
		{
			obj = Instantiate(_object, Vector3.zero, Quaternion.identity) as GameObject;
			obj.SetActive(true);
			_pool.Add (obj);
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
			for(int i=0;i<_pool.Count;++i)
			{
				if(_pool[i] == obj)
				{
					//_pool[i]   = obj; 
					// stop for loop. Way to prevent branch with break or return
					recieved   = true;
					_pool[i].SetActive(false);
					obj = null;
					i          = _pool.Count;
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