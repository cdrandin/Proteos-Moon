﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunPoolingSystem : MonoBehaviour 
{
	public List<GameObject> objects_to_pool; 

	public List<GameObject> objects_in_the_pool;

	// Use this for initialization
	void Start () 
	{
		objects_in_the_pool = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GameObject obj = PoolingSystem.instance.Instantiate(objects_to_pool[Random.Range(0, objects_to_pool.Count)]);
			if(obj == null) return;

			obj.transform.position = new Vector3(Random.Range(-5.0f, 5.0f), 3.0f, Random.Range(-5.0f, 5.0f));
			objects_in_the_pool.Add(obj);
		}

		else if(Input.GetKeyDown(KeyCode.Backspace))
		{
			if(objects_in_the_pool.Count > 0)
			{
				int i = Random.Range(0, objects_in_the_pool.Count);
				PoolingSystem.instance.Destroy(objects_in_the_pool[i]);
				objects_in_the_pool.RemoveAt(i);
			}
		}
	}
}