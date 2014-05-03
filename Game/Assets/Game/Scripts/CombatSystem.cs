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

	private delegate void GUIMethod();
	private GUIMethod gui_method = null;
	
	private static Player currentPlayer = Player.NONE;
	
	public static CombatSystem instance;
	private Rect EnemyUnitBoxLoc, EnemyUnitBox;
	private int index = 0;
	private List<GameObject> enemyList;
	
	private bool attacking = false, showGUI = false, isLabelOn = false;
	
	
	private float alpha;
	
	private float wantedRotationAngle, wantedHeight, currentRotationAngle, currentHeight, height = 10.0f, heightDamping = 0.5f , rotationDamping = 0.5f;
	private Quaternion currentRotation;
	#endregion
	
	// Update is called once per frame
	public void Update () {
		
		if( isLabelOn ){
			FadeInOut ();
		}

	}
	public void FixedUpdate(){
	
	}
	public bool CheckIfAttacking(){
		return attacking;
	}
	
	public void Start () {
		instance = this;
		
	}
	
	public void CheckIfChangingTarget(){
		
		if( Input.GetKeyDown(KeyCode.LeftArrow) ) {
			
			if(index + 1 >= enemyList.Count)
				index = 0;
			else if ( index + 1 < enemyList.Count)
				++index;
				
			gui_method -= UnitEnemyBox;
			gui_method += UnitEnemyBox;
				
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow) ) {
			print (index);
			if(index - 1 < 0)
				index = 0;
			else if (index - 1 >= 0)
				--index;
			
			gui_method -= UnitEnemyBox;
			gui_method += UnitEnemyBox;
			
		}	
	}

	void OnGUI(){
		
		
		GUI.skin = UnitGUI.UnitGUISkin();
		if(this.gui_method != null ){
			
			this.gui_method();
		}
	
	}
	
	public void FlashLabel(){
	
		//GUIContent color = new Color( GUI.color.r, GUI.color.g, GUI.color.b,  alpha);
		//GUIStyle newStyle = new GUIStyle(GUI.skin.label);
		GUI.contentColor = new Color( GUI.color.r, GUI.color.g, GUI.color.b,  alpha);
		GUI.Label( new Rect( Screen.width / 2 , Screen.height / 2 , Screen.width/3, Screen.height / 16), "Press Space to Attack");
		//print ("Stuff");		
	}
	
	public void FadeInOut(){
	
		if( showGUI ){	
			alpha = Mathf.Lerp(alpha,0.0f ,Time.time*0.01f);
			if (Mathf.Abs (alpha - 0 ) < 0.0001){
				showGUI = !showGUI;
			}
		}
		else{
			alpha = Mathf.Lerp(alpha , 1.0f ,Time.time*0.01f);
			if (Mathf.Abs (alpha - 1 ) < 0.0001f){
				showGUI = !showGUI;
			}
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
	
	public static void ResetCombatSystem(){
	
		CombatSystem.instance.gui_method -= CombatSystem.instance.UnitEnemyBox;
		CombatSystem.instance.index = 0;
		CombatSystem.instance.enemyList.Clear();
		CombatSystem.instance.attacking = false;
		CombatSystem.instance.isLabelOn = false;
		CombatSystem.instance.gui_method -= CombatSystem.instance.FlashLabel;
		CombatSystem.instance.StopCoroutineProcess();
	}
	
	public void StopCoroutineProcess(){
	
		StopCoroutine("Attack");
	}
	
	public void UnitEnemyBox(){
	
	
		if( enemyList.Count != 0){
			
			EnemyUnitBoxLoc = UnitGUI.UnitBoxLocation()	;
			EnemyUnitBoxLoc.y += EnemyUnitBox.height;
			EnemyUnitBox = new Rect( 0, 0, EnemyUnitBoxLoc.width, EnemyUnitBoxLoc.height);
			
			GUI.BeginGroup(EnemyUnitBoxLoc);
			
			GUI.depth = 1	;
			GUI.Box( EnemyUnitBox, "");
			GUI.contentColor = new Color( GUI.color.r, GUI.color.g, GUI.color.b, 1.0f  );
			UnitGUI.CharacterPortrait(EnemyUnitBox, enemyList[index], UnitGUI.UnitGUISkin().FindStyle("red_box") , GM.instance.GetPlayer( (((int)GM.instance.CurrentPlayer) + 1) % 2 ));
			UnitGUI.HealthExhaustBars(EnemyUnitBox, enemyList[index]);
			
			
			GUI.EndGroup();
		}
	}
	
	public void AttackButtonClicked(){
		isLabelOn = true;
		gui_method += FlashLabel;
		gui_method += UnitEnemyBox;
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

		print ("This is the current focus unit: " + focusUnit);

		WithinRange(focusUnit);
		TurnOnProjector();
	}
	
	public void CombatLookAt(GameObject focus){
	
//		MainCamera.transform.LookAt();
		if(gui_method == null){
			
			gui_method += UnitEnemyBox;
		}
		
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
	
	public IEnumerator Attack(GameObject focusUnit){
	
		if(Input.GetKeyDown(KeyCode.Space) ) {	
			isLabelOn = false;
			gui_method -= FlashLabel;

			//TODO: Figure out how damage is dealt		
			//HACK: default calculations are set
			
			enemyList[index].GetComponent<BaseClass>().vital.HP.current -= (float)focusUnit.GetComponent<BaseClass>().base_stat.Strength.current;
			enemyList[index].GetComponent<BaseClass>().vital.HP.current -= (float)focusUnit.GetComponent<BaseClass>().base_stat.Agility.current;
			yield return new WaitForSeconds ( 5.0f );
			gui_method -= UnitEnemyBox;
			
			print ("Health" + enemyList[index].GetComponent<BaseClass>().vital.HP.current);
			
			if(enemyList[index].GetComponent<BaseClass>().vital.HP.current == 0)
			{
				enemyList[index].GetComponent<BaseClass>().unit_status.Dead();
				GM.instance.UnitDied(enemyList[index]);
			}
			ResetCombatSystem();
		}
		yield return null;
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
