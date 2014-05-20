using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class HorizontalSlowMovement : MonoBehaviour 
{
	[Range(0.1f,0.999f)]
	public float slow_amount;

	void Start()
	{
		GetComponent<BoxCollider>().isTrigger = true;
	}
	
	// Entered the movement impairing object
	void OnTriggerEnter(Collider other)
	{
		Debug.Log("OnTriggerEnter");
		BaseClass unit = other.gameObject.GetComponent<BaseClass>();
		if(unit != null)
		{
			unit.ModMovement(slow_amount);
		}
	}

	// Exit the movement impairing object
	void OnTriggerExit(Collider other)
	{
		BaseClass unit = other.gameObject.GetComponent<BaseClass>();
		if(unit != null)
		{
			unit.ModMovement(1.0f);
		}
	}
}
