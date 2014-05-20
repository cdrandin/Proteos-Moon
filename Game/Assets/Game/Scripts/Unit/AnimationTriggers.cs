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
	//int temp = 0;
	
	int idle_state_hash = Animator.StringToHash("Base Layer.Idle");
	int ready_state_hash = Animator.StringToHash("Base Layer.Ready");
	//int num_of_attacks_hash = Animator.StringToHash("num_of_attacks");
	int attack_style_hash = Animator.StringToHash("attack_style");
	//int isMoving_hash = Animator.StringToHash("isMoving");
	int health_hash = Animator.StringToHash("Health");
	int speed_hash = Animator.StringToHash("Speed");
	AnimatorStateInfo stateInfo;
	//int number_of_attacks;
	
//	float hit_time;
	
	void Start ()
	{
		
		anim = GetComponent<Animator>();
		//number_of_attacks = anim.GetInteger(num_of_attacks_hash);
	}
	
	
	void Update ()	{
	
		stateInfo = anim.GetCurrentAnimatorStateInfo(0);
	}
	
	public void MoveAnimation(float move_scalar){
	
		anim.SetFloat(speed_hash, move_scalar);		
		
	
	}
	
	//An event in the script
	public void Hit_Time(float new_hit_time){
	
		//TODO: add attack Noise
//		hit_time = new_hit_time;
		
	}
	
	public void AttackAnimation(){
		
		if( stateInfo.nameHash == ready_state_hash){
		
			//int	attack_value = Random.Range(0, number_of_attacks);
			/*
			++temp;
			if(temp >= number_of_attacks){
				temp = 0;
			}
			
			int attack_value = temp;
			*/
			
			anim.SetInteger(attack_style_hash, 0);
			anim.SetTrigger (attack_hash);


			/*
			AnimationInfo[] test = anim.GetCurrentAnimationClipState(0);
			
			AnimationEvent[] events = AnimationUtility.GetAnimationEvents( test[0].clip) ;
			print ("Animation Clip name " + test[0].clip.name);
			print("Length of events " + events.Length);
			print (events[0].functionName);
			print (events[0].time);
			*/
		
		}
		
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

	public void ForceGatherAnimation()
	{
		anim.SetTrigger(gather_hash);
	}

	public void GatherAnimation(){
		
		if( stateInfo.nameHash == ready_state_hash){
			anim.SetTrigger (gather_hash);
		}
	}
}

