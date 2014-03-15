using UnityEngine;
using System.Collections;

public class CombatSystem : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool CanHitUnit(Entity attacker, Entity defender){

		return attacker.attack_range < Vector3.Distance (defender.transform.position, attacker.transform.position);
	}

	bool CanHitUnit(GameObject attacker, GameObject defender){

		return attacker.GetComponent<Entity> ().attack_range < Vector3.Distance (defender.transform.position, attacker.transform.position);
	}

	void AttackUnit(Entity attacker, Entity defender){

		while (true) {
			yield return new WaitForSeconds (5.0f);
			break;
		}
		defender.hp = defender.hp - attacker.damage;
	}
}
