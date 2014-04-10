using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour{

	// Event Handler
	public delegate void WithinRangeEvent(GameObject currentFocus);
	public static event WithinRangeEvent WithinRange;
	
	public delegate void ProjectorEvent();
	public static event ProjectorEvent TurnOnProjector;
	
	private static Player currentPlayer = Player.NONE;

	// Update is called once per frame
	public void Update () {
	
	}
	
	public static void UpdateWithinRangeDelegate(){
 		
		if (currentPlayer != GM.instance.CurrentPlayer) {

			if(WithinRange != null)
				CleanDelegateBeforeSwitch();
			
			AddDelegates();
			
			currentPlayer = GM.instance.CurrentPlayer;
		}
		
	}
	
	public static bool AnyNearbyUnitsToAttack(GameObject focusUnit){
	
		
		return GM.instance.GetEnemyUnitsNearPlayer(focusUnit, focusUnit.GetComponent<BaseClass>().attack_range).Count != 0;
	}

	public static List<GameObject> GetNearbyAttackableUnits(GameObject focusUnit){
	
	
		return GM.instance.GetEnemyUnitsNearPlayer(focusUnit, focusUnit.GetComponent<BaseClass>().attack_range);
	}
	
	public static void CallCombatDelegates(GameObject focusUnit){

		WithinRange(focusUnit);
		TurnOnProjector();
	}
	
	public static void CombatLookAt(ref GameObject MainCamera, GameObject focus, GameObject enemyUnit){
	
//		MainCamera.transform.LookAt();
		
	}
	
	
	public static void Attack(GameObject focusUnit, GameObject enemyUnit){
	
		//TODO: Figure out how damage is dealt		
	
		WaitForAttackAnimation(5);
		
		
		//HACK: default calculations are set
		
		enemyUnit.GetComponent<BaseClass>().vital.HP.current -= (float)focusUnit.GetComponent<BaseClass>().base_stat.Strength.current;
		enemyUnit.GetComponent<BaseClass>().vital.HP.current -= (float)focusUnit.GetComponent<BaseClass>().base_stat.Agility.current;
		
	}

	private static IEnumerator WaitForAttackAnimation(float timeInSeconds){
	
		yield return new WaitForSeconds(timeInSeconds);
	}

	private static void AddDelegates(){
		
		for(uint j = 0; j < GM.instance.NumberOfPlayers; ++j){
		
			if((Player)j == GM.instance.CurrentPlayer)
				continue;

			GameObject [] otherPlayerUnits = GM.instance.GetUnitsFromPlayer ((Player)j);
		
			for( uint i = 0 ; i < otherPlayerUnits.Length; ++i){
			
				WithinRange += otherPlayerUnits[i].GetComponent<UnitActions>().WithinRange;
				TurnOnProjector += otherPlayerUnits[i].GetComponent<UnitActions>().TurnOnProjector;
			}
		}
	}

	private static void CleanDelegateBeforeSwitch(){
	
		for(uint j = 0; j < GM.instance.NumberOfPlayers; ++j){
	
			if((Player)j == GM.instance.CurrentPlayer)
				continue;
			
			GameObject [] otherPlayerUnits = GM.instance.GetUnitsFromPlayer ((Player)j);

			for (uint i = 0; i < otherPlayerUnits.Length; ++i){

				WithinRange -= otherPlayerUnits[i].GetComponent<UnitActions>().WithinRange;
				TurnOnProjector -= otherPlayerUnits[i].GetComponent<UnitActions>().TurnOnProjector;
				
			}
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
