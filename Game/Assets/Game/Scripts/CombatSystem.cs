using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour{

	// Event Handler
	#region class variables
	public delegate void WithinRangeEvent(GameObject currentFocus);
	public static event WithinRangeEvent WithinRange;
	
	public delegate void ProjectorEvent();
	public static event ProjectorEvent TurnOnProjector;
	
	private static Player currentPlayer = Player.NONE;
	
	public static CombatSystem instance;
	
	private int index = 0;
	private List<GameObject> enemyList;
	
	private bool attacking = false;
	
	private float wantedRotationAngle, wantedHeight, currentRotationAngle, currentHeight, height = 10.0f, heightDamping = 0.5f , rotationDamping = 0.5f;
	private Quaternion currentRotation;
	#endregion
	
	// Update is called once per frame
	public void Update () {	}
	
	public bool CheckIfAttacking(){
		return attacking;
	}
	
	public void Start () {
		instance = this;	
	}
	
	public void CheckIfChangingTarget(){
		
		if( Input.GetKeyDown(KeyCode.LeftArrow) ) {
		
			if(index + 1 > enemyList.Count)
				index = 0;
			else if ( index + 1 < enemyList.Count)
				++index;
				
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow) ) {
			
			if(index - 1 < 0)
				index = 0;
			else if (index - 1 > 0)
				--index;
			
		}	
	}

	
	public void UpdateWithinRangeDelegate(){
 		
		if (currentPlayer != GM.instance.CurrentPlayer) {

			if(WithinRange != null)
				CleanDelegateBeforeSwitch();
			
			AddDelegates();
			
			currentPlayer = GM.instance.CurrentPlayer;
		}
		
	}
	
	public void ResetCombatSystem(){
	
		index = 0;
		enemyList.Clear();
		attacking = false;
	}
	
	public void AttackButtonClicked(){
	
		attacking = true;
	}
	
	public bool AnyNearbyUnitsToAttack(GameObject focusUnit){
	
		GetNearbyAttackableUnits(focusUnit);
		
		return enemyList.Count != 0;
	}
	
	public void GetNearbyAttackableUnits(GameObject focusUnit){
	
		
		enemyList = GM.instance.GetEnemyUnitsNearPlayer(focusUnit, focusUnit.GetComponent<BaseClass>().attack_range);
	}
	
	public void CallCombatDelegates(GameObject focusUnit){

		WithinRange(focusUnit);
		TurnOnProjector();
	}
	
	

	public void CombatLookAt(GameObject focus){
	
//		MainCamera.transform.LookAt();
	
		//print (target.localPosition);
		
		wantedRotationAngle = enemyList[index].transform.eulerAngles.y;
		//print (wantedRotationAngle);
		wantedHeight = enemyList[index].transform.position.y + height;
		
		currentRotationAngle = WorldCamera.instance.transform.eulerAngles.y;
		currentHeight = WorldCamera.instance.transform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		Vector3 worldCameraPosition =  focus.transform.position;
		worldCameraPosition -= currentRotation * Vector3.Normalize(enemyList[index].transform.position - focus.transform.position) * 10;	
		//print (currentRotation);
		//print (Vector3.Normalize(enemyList[index].transform.position - focus.transform.position));
		// Set the height of the camera
		WorldCamera.instance.transform.position = new Vector3 (worldCameraPosition.x, currentHeight, worldCameraPosition.z);
		
		//TODO: interpolate
		WorldCamera.instance.MainCamera.transform.LookAt(enemyList[index].transform);
	}
	
	
	public void Attack(GameObject focusUnit){
	
		if(Input.GetKeyDown(KeyCode.Return) ) {	

			//TODO: Figure out how damage is dealt		
			WaitForAttackAnimation(5);
					
			//HACK: default calculations are set
			enemyList[index].GetComponent<BaseClass>().vital.HP.current -= (float)focusUnit.GetComponent<BaseClass>().base_stat.Strength.current;
			enemyList[index].GetComponent<BaseClass>().vital.HP.current -= (float)focusUnit.GetComponent<BaseClass>().base_stat.Agility.current;
			print ("Health" + enemyList[index].GetComponent<BaseClass>().vital.HP.current);
			if(enemyList[index].GetComponent<BaseClass>().vital.HP.current == 0)
			{
				enemyList[index].GetComponent<BaseClass>().unit_status.status = Status.Dead;
				GM.instance.UnitIsDead(enemyList[index]);
			}

			ResetCombatSystem();
		}
	}

	private IEnumerator WaitForAttackAnimation(float timeInSeconds){
	
		yield return new WaitForSeconds(timeInSeconds);
	}

	private void AddDelegates(){
		
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

	private void CleanDelegateBeforeSwitch(){
	
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
