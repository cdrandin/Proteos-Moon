using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolSystemTest : MonoBehaviour 
{
	public List<GameObject> objects;

	// Use this for initialization
	void Start () 
	{
		objects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GameObject obj = PoolingSystem.instance.GetObject();
			if(obj == null) return;

			obj.transform.position = new Vector3(0.0f, 3.0f, 0.0f);
			objects.Add(obj);
		}

		if(Input.GetKeyDown(KeyCode.Backspace))
		{
			if(objects.Count > 0)
			{
				int i = Random.Range(0, objects.Count);
				PoolingSystem.instance.ReturnObject(objects[i]);
				objects.RemoveAt(i);
			}
		}
	}
}
