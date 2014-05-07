using UnityEngine;
using System.Collections;

public class AnimationTriggers : MonoBehaviour {

	Animator anim;
	
	int attack_hash = Animator.StringToHash("Attack");
	int damage_hash = Animator.StringToHash("Damage");
	int gather_hash = Animator.StringToHash("Gather");
	int idle_hash = Animator.StringToHash("Idle");
	int ready_hash = Animator.StringToHash("Ready");
	
	// temp for testing
	int temp = 0;
	
	int idle_state_hash = Animator.StringToHash("Base Layer.Idle");
	int ready_state_hash = Animator.StringToHash("Base Layer.Ready");
	int num_of_attacks_hash = Animator.StringToHash("num_of_attacks");
	int attack_style_hash = Animator.StringToHash("attack_style");
	int speed_hash = Animator.StringToHash("Speed");
	int health_hash = Animator.StringToHash("health_hash");
	
	AnimatorStateInfo stateInfo;
	int number_of_attacks;
	
	
	void Start ()
	{
		
		anim = GetComponent<Animator>();
		number_of_attacks = anim.GetInteger(num_of_attacks_hash);
	}
	
	
	void Update ()	{
	
		stateInfo = anim.GetCurrentAnimatorStateInfo(0);
	}
	
	public void MoveAnimation(float move_scalar){
	
		anim.SetFloat(speed_hash, move_scalar);		
		
	
	}
	
	public AnimatorStateInfo AttackAnimation(){
		
		if( stateInfo.nameHash == ready_state_hash){
		
			int	attack_value = Random.Range(0, number_of_attacks);
			/*
			++temp;
			if(temp >= number_of_attacks){
				temp = 0;
			}
			
			int attack_value = temp;
			*/
			
			anim.SetInteger(attack_style_hash, attack_value);
			Debug.Log(attack_value);
			anim.SetTrigger (attack_hash);		
		}
		return anim.GetCurrentAnimatorStateInfo(0);;
	}
	
	public void DamageAnimation(int newHealth){

		if( stateInfo.nameHash == idle_state_hash){
			
			anim.SetInteger(health_hash, newHealth);
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

