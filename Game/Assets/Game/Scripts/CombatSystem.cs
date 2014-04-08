using UnityEngine;
using System.Collections;

public static class CombatSystem{

	// Event Handler
	private delegate void WithinRangeEvent(GameObject currentFocus);
	public static event WithinRangeEvent WithinRange;
	private static Player currentPlayer;
	// Use this for initialization
	public static void Start () {

		currentPlayer = Player.NONE;
	}
	
	// Update is called once per frame
	public static void Update () {
	
	}
	
	public static void UpdateWithinRangeDelegate(){
 
		//HACK: this will only work for two players
		if (currentPlayer != GM.instance.CurrentPlayer) {
			
			if(WithinRange != null)
				CleanDelegateBeforeSwitch();
			
			AddDelegates();
			
			currentPlayer = GM.instance.CurrentPlayer;
		}
		
	}

	private static void AddDelegates(){
		
		Game [] otherPlayerUnits = GM.instance.GetUnitsFromPlayer (!GM.instance.CurrentPlayer);
		
		for( uint i = 0 ; i < otherPlayerUnits.Length; ++i){
			
			WithinRange += otherPlayerUnits[i].GetComponent<UnitActions>().WithinRange;
		}

	}

	private static void CleanDelegateBeforeSwitch(){

		Game [] otherPlayerUnits = GM.instance.GetUnitsFromPlayer (!GM.instance.CurrentPlayer);

		for (uint i = 0; i < otherPlayerUnits.Length; ++i){

			WithinRange -= otherPlayerUnits[i].GetComponent<UnitActions>().WithinRange;
			}

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
