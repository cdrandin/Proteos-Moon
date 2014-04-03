using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class aaa : MonoBehaviour {

	private PoolingSystem _pooling_system;
	public List<GameObject> pooled_objs;

	void Awake ()
	{
		_pooling_system = GameObject.Find("PoolingSystem").GetComponent<PoolingSystem>();
		pooled_objs     = new List<GameObject>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(pooled_objs.Count < _pooling_system.total)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				GameObject obj = _pooling_system.GetObject(new Vector3(0, 2, 0));
				pooled_objs.Add(obj);
			}
			
		}
		
		if(pooled_objs.Count > 0)
		{
			if(Input.GetKeyDown(KeyCode.Backspace))
			{
				// Remove the first object in the list
				GameObject obj = pooled_objs[0];
				pooled_objs.Remove(obj);
				_pooling_system.ReturnObject(obj);
			}
		}
	}
}
