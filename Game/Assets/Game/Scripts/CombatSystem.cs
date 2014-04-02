using UnityEngine;
using System.Collections;

public static class CombatSystem{

	// Event Handler
	private delegate void WithinRangeEvent(GameObject currentFocus);
	private static event WithinRangeEvent WithinRange;
	private int currentCount;
	// Use this for initialization
	public static void Start () {
		currentCount = 0;
	}
	
	// Update is called once per frame
	public static void Update () {
	
	}
	
	public static void AddWithinRange(){
//		if(currentCount != (GameManager.))
		
	}
	/*
	bool CanHitUnit(Entity attacker, Entity defender){

		return attacker.attack_range < Vector3.Distance (defender.transform.position, attacker.transform.position);
	}

	bool CanHitUnit(GameObject attacker, GameObject defender){

//		return attacker.GetComponent<Entity> ().attack_range < Vector3.Distance (defender.transform.position, attacker.transform.position);
		return false;
	}

	void AttackUnit(Entity attacker, Entity defender){

		//HACK need animation time
		float animationTemp = 5.0f;
		WaitForAnimation (animationTemp);
		defender.hp = defender.hp - attacker.damage;
	}
		
	IEnumerator WaitForAnimation(float time){
		yield return new WaitForSeconds (time);
	}
	*/
}
