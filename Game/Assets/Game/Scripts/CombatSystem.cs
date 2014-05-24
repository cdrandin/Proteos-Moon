using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour{

	// Event Handler
	#region class variables
	public delegate void WithinRangeEvent(GameObject currentFocus);
	public static event WithinRangeEvent WithinRange;
	
	public delegate void ProjectorEvent();
	public static event ProjectorEvent TurnOnHighlight;
	public delegate void TurnOffEvent();
	public static event TurnOffEvent TurnOffHighlight;

	private delegate void GUIMethod();
	private GUIMethod gui_method = null;
		
	public static CombatSystem instance;
	private Rect EnemyUnitBoxLoc, EnemyUnitBox;
	private int index = 0, previousIndex;
	private List<GameObject> enemyList;

	private BaseClass enemyBaseClass;
			
	private bool attacking = false, inCombat = false;
	
	
	private float alpha;
	
	#endregion
	
	// Update is called once per frame
	public void Update () {}
	
	public bool CheckIfAttacking(){
		return attacking;
	}
	
	public void Start () {
		alpha = 1.0f;
		instance = this;
		
	}
	
	public void StartCombat(GameObject focus){
	
		StartCoroutine(StartCombatCoroutine(focus));
		
	}
	
	private IEnumerator StartCombatCoroutine(GameObject focus){
		
		inCombat = true;
		WorldCamera.instance.MainCamera.transform.localEulerAngles = Vector3.zero;
		StartCoroutine("FadeInOut");
		StartCoroutine("CombatLookAt", focus);
		while(attacking){
		
			CheckIfButtonsPress(focus);			
			yield return null;
		}
		if(TurnOffHighlight != null)
			TurnOffHighlight();
		yield return null;
	}
	
	public bool CurrentlyInCombat(){
		return inCombat;
	}
	
	public void CheckIfButtonsPress(GameObject focus){
		
		if( Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) ) {
			previousIndex = index;
			if(index + 1 >= enemyList.Count)
				index = 0;
			else if ( index + 1 < enemyList.Count)
				++index;
				
			enemyBaseClass = enemyList[index].GetComponent<BaseClass>();	
			gui_method -= UnitEnemyBox;
			gui_method += UnitEnemyBox;
			
			if(previousIndex != index){
				StopCoroutine("CombatLookAt");
				StartCoroutine("CombatLookAt" , focus);
			}
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.D)   ) {
			previousIndex = index;
			if(index - 1 <= -1)
				index =  enemyList.Count - 1;
			else if (index - 1 >= 0)
				--index;
			
			enemyBaseClass = enemyList[index].GetComponent<BaseClass>();
			gui_method -= UnitEnemyBox;
			gui_method += UnitEnemyBox;
			
			if(previousIndex != index){
				StopCoroutine("CombatLookAt");
				StartCoroutine("CombatLookAt" , focus);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Space) ) {
			StartCoroutine("Attack", focus);
		}
	}

	void OnGUI(){
	
		GUI.skin = UnitGUI.UnitGUISkin();
		if(this.gui_method != null ){
			this.gui_method();
		}
	
	}
	
	public void FlashLabel(){
	
		GUI.contentColor = new Color( GUI.color.r, GUI.color.g, GUI.color.b,  alpha);
		GUI.skin.label.fontSize = Screen.height / 30;
		GUI.Label( new Rect( Screen.width / 2 , Screen.height / 2 , Screen.width/3, Screen.height / 32), "Press Space to Attack");
	}
	
	public IEnumerator FadeInOut(){

		while(attacking){	
			
			while( alpha > 0.01 ){
				alpha = Mathf.Lerp(alpha,0.0f , Time.deltaTime * 3.0f);
				yield return null;
			}
			
			alpha = 0.0f;
			
			while( Mathf.Abs (alpha - 1 )  > 0.01f){
				alpha = Mathf.Lerp(alpha , 1.0f , Time.deltaTime * 3.0f);
				yield return null;
			}
			
			alpha = 1.0f;
			yield return null;
		}
	}
		
	
	public void UpdateWithinRangeDelegate(){
 		
		if (GM.instance.WhichPlayerAmI == GM.instance.CurrentPlayer) {

			if(WithinRange != null)
				CleanDelegateBeforeSwitch();
			
			AddDelegates();
		}
		
	}
	
	public void ResetCombatSystem(){
	
		gui_method -= UnitEnemyBox;
		index = 0;
		if (enemyList != null)
			enemyList.Clear();
		attacking = false;
		inCombat = false;
		
		if (TurnOffHighlight != null)
			TurnOffHighlight();
		
		gui_method -= FlashLabel;
		StopAllCoroutines();
		WorldCamera.instance.ResetCamera();
		WorldCamera.instance.TurnCameraControlsOn();
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
			UnitGUI.CharacterPortrait(EnemyUnitBox, enemyList[index], GM.instance.GetPlayer( (((int)GM.instance.CurrentPlayer) + 1) % 2 ));
			UnitGUI.HealthExhaustBars(EnemyUnitBox, enemyBaseClass);
			
			
			GUI.EndGroup();
		}
	}
	
	public void AttackButtonClicked(){
		enemyBaseClass = enemyList[index].GetComponent<BaseClass>();
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

		WithinRange(focusUnit);
		TurnOnHighlight();
	}
	
	
	
	Vector3 GetFinalCameraPosition(Transform focus, float characterHeight, float characterYAngle){
	
		float DistancefromPlayer = characterHeight / 0.5f;
		Vector3 finalCameraPosition = focus.position +  focus.right * (0.25f * characterHeight);	
		
		float finalCameraHeight = focus.position.y + characterHeight +  5.0f;
		
		Quaternion characterRotation = Quaternion.Euler (0.0f, characterYAngle, 0.0f);
		
		finalCameraPosition -= characterRotation * Vector3.forward * DistancefromPlayer;
		return new Vector3 (finalCameraPosition.x, finalCameraHeight, finalCameraPosition.z);
	 	
	}
	
	public IEnumerator CombatLookAt(GameObject focus){
		
		
		
		Vector3 attacker = focus.transform.position;
		Vector3 enemyPostion = enemyList[index].transform.position;
		Vector3 cameraLookAt = enemyList[index].transform.position;
		cameraLookAt.y += (0.85f) * enemyList[index].GetComponent<CapsuleCollider>().height;
		float characterHeight = (0.85f) * focus.GetComponent<CapsuleCollider>().height;			
		
		attacker.y = 0.0f;
		enemyPostion.y = 0.0f;
		
		float smoothCamPos = 0.05f, smoothCamRot = 0.05f, smoothFocRot = 0.05f;
		
		//Get the final positions of each component, i.e. the camera, position and rotation, and the character rotation
		Quaternion finalfocusRotation  = Quaternion.LookRotation(enemyPostion - attacker);
		Vector3 finalCameraPosition = GetFinalCameraPosition(focus.transform, characterHeight, finalfocusRotation.eulerAngles.y);
		Quaternion finalCameraRotation = Quaternion.LookRotation(cameraLookAt - finalCameraPosition);
		
		
		while ( !GM.WithinEpsilon(WorldCamera.instance.transform.position, finalCameraPosition, 0.0001f) ){
			
			WorldCamera.instance.transform.position = Vector3.Lerp(WorldCamera.instance.transform.position, finalCameraPosition, Time.deltaTime + smoothCamPos);				
			WorldCamera.instance.cameraY = WorldCamera.instance.transform.position.y;
			WorldCamera.instance.transform.rotation = Quaternion.Slerp(WorldCamera.instance.transform.rotation, finalCameraRotation, Time.deltaTime + smoothCamRot);
			focus.transform.rotation = Quaternion.Slerp(focus.transform.rotation, finalfocusRotation, Time.deltaTime +smoothFocRot);
			
			Debug.Log("1");
			yield return null;
		}
		
		WorldCamera.instance.transform.position = finalCameraPosition;
		
		while ( !GM.WithinEpsilon(WorldCamera.instance.transform.eulerAngles, finalCameraRotation.eulerAngles, 0.0001f) ){
			
			WorldCamera.instance.transform.rotation = Quaternion.Slerp(WorldCamera.instance.transform.rotation, finalCameraRotation, Time.deltaTime +smoothCamRot);
			focus.transform.rotation = Quaternion.Slerp(focus.transform.rotation, finalfocusRotation, Time.deltaTime + smoothFocRot);
			Debug.Log("2");
			yield return null;
		}
		
		WorldCamera.instance.transform.rotation = finalCameraRotation;
		
		while ( !GM.WithinEpsilon(focus.transform.eulerAngles, finalfocusRotation.eulerAngles, 0.0001f) ){
			
			WorldCamera.instance.transform.rotation = Quaternion.Slerp(WorldCamera.instance.transform.rotation, finalCameraRotation, Time.deltaTime +smoothCamRot);
			focus.transform.rotation = Quaternion.Slerp(focus.transform.rotation, finalfocusRotation, Time.deltaTime + smoothFocRot);
			Debug.Log("3");
			yield return null;
		}
		
		focus.transform.rotation = finalfocusRotation;
		
		focus.GetPhotonView().RPC("UpdateUnitTransformation", PhotonTargets.OthersBuffered, focus.transform.position, focus.transform.rotation);				
	}
	
	
	public IEnumerator Attack(GameObject focusUnit){
	
			attacking = false;
			gui_method -= FlashLabel;

			//TODO: Figure out how damage is dealt		
			//HACK: default calculations are set

			float damage = (float)focusUnit.GetComponent<BaseClass>().base_stat.Strength.current + 
					       (float)focusUnit.GetComponent<BaseClass>().base_stat.Agility.current;	
			
			//float newHealth = enemyList[index].GetComponent<BaseClass>().vital.HP.current - damage;
			
			focusUnit.GetComponentInChildren<AnimationTriggers>().AttackAnimation();
			
			
			enemyList[index].GetComponent<PhotonView>().RPC("DamageAnimation", PhotonTargets.AllBuffered, damage);
			
			yield return new WaitForSeconds ( 1.0f );
			
			enemyList[index].GetComponent<PhotonView>().RPC("DealDamage", PhotonTargets.AllBuffered, damage);
			
			
			yield return new WaitForSeconds ( 1.0f );
			
			gui_method -= UnitEnemyBox;
			
			if(enemyList[index].GetComponent<BaseClass>().vital.HP.current == 0)
			{
				enemyList[index].GetComponent<BaseClass>().unit_status.Dead();
				GM.instance.UnitDied(enemyList[index]);
			}
			ResetCombatSystem();
			yield return null;

	}


	private void AddDelegates(){
		
		for(uint j = 0; j < GM.instance.NumberOfPlayers; ++j){
		
			//If I am this player, skip to next player
			if((Player)j == GM.instance.WhichPlayerAmI)
				continue;

			GameObject [] otherPlayerUnits = GM.instance.GetUnitsFromPlayer ((Player)j);
		
			for( uint i = 0 ; i < otherPlayerUnits.Length; ++i){
			
				WithinRange += otherPlayerUnits[i].GetComponent<UnitActions>().WithinRange;
				TurnOnHighlight += otherPlayerUnits[i].GetComponent<UnitActions>().TurnOnHighlight;
				TurnOffHighlight += otherPlayerUnits[i].GetComponent<UnitActions>().TurnOffHighlight;
			}
		}
	}

	private void CleanDelegateBeforeSwitch(){
	
		WithinRange = null;
		TurnOnHighlight = null;
		TurnOffHighlight = null;
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
