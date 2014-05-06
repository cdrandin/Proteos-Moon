using UnityEngine;
using System.Collections;

public class AnimationTriggers : MonoBehaviour {

	Animator anim;
	
	int attack_hash = Animator.StringToHash("Attack");
	int damage_hash = Animator.StringToHash("Damage");
	int death_hash = Animator.StringToHash("Death");
	int gather_hash = Animator.StringToHash("Gather");
	int idle_hash = Animator.StringToHash("Idle");
	int ready_hash = Animator.StringToHash("Ready");
	
	
	int idle_state_hash = Animator.StringToHash("Base Layer.Idle");
	int ready_state_hash = Animator.StringToHash("Base Layer.Ready");
	AnimatorStateInfo stateInfo;
	int health = 100;
	int number_of_attacks;
	void Start ()
	{
		
		anim = GetComponent<Animator>();
		number_of_attacks = anim.GetInteger("num_of_attacks");
	}
	
	
	void Update ()
	{
		stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		
		//float move = Input.GetAxis ("Vertical");
		//anim.SetFloat("Speed", move);		
	}
	
	public void AttackAnimation(){
		
		if( stateInfo.nameHash == ready_state_hash){
		
			int	attack_value = Random.Range(0, number_of_attacks);
			anim.SetInteger("attack_style", attack_value);
			Debug.Log(attack_value);
			anim.SetTrigger (attack_hash);		
		}
		
	}
	
	public void DamageAnimation(int newHealth){

		if( stateInfo.nameHash == idle_state_hash){
			
			anim.SetInteger("Health", newHealth);
			anim.SetTrigger (damage_hash);
		}
	}
	
	public void ReadyAnimation(){
	
		if( stateInfo.nameHash == idle_state_hash){
			
			anim.SetTrigger (ready_hash);
		}
	}
	
	public void IdleAnimation(){
		if( stateInfo.nameHash == ready_state_hash){
			
			anim.SetTrigger (idle_hash);
		}
	}
	
	public void GatherAnimation(){
		
		if( stateInfo.nameHash == ready_state_hash){
			anim.SetTrigger (gather_hash);
		}
	}
}

