using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class HorizontalSlowMovement : MonoBehaviour 
{
	void Start()
	{
		GetComponent<BoxCollider>().isTrigger = true;
	}
	
	// Entered the movement impairing object
	void OnTriggerEnter(Collider other)
	{
		BaseClass unit = other.gameObject.GetComponent<BaseClass>();
		if(unit != null)
		{
			unit.GetComponent<UnitController>().mod_movement(MOVEMENT_AFFECT.SLOWED);
		}
	}

	// Exit the movement impairing object
	void OnTriggerExit(Collider other)
	{
		BaseClass unit = other.gameObject.GetComponent<BaseClass>();
		if(unit != null)
		{
			unit.GetComponent<UnitController>().mod_movement(MOVEMENT_AFFECT.NORMAL);
		}
	}
}
