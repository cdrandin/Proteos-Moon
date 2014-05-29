using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class HorizontalSlowMovement : MonoBehaviour 
{
	private UnitController _uc;

	void Start()
	{
		GetComponent<BoxCollider>().isTrigger = true;
		_uc = GameObject.FindGameObjectWithTag("UnitController").GetComponent<UnitController>();
	}
	
	// Entered the movement impairing object
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Unit" || other.tag == "Leader")
		{
			_uc.mod_movement(MOVEMENT_AFFECT.SLOWED);
		}
	}

	// Exit the movement impairing object
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Unit" || other.tag == "Leader")
		{
			_uc.mod_movement(MOVEMENT_AFFECT.NORMAL);
		}
	}
}
