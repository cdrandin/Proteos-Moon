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
	
	int health = 100;
	int number_of_attacks;
	void Start ()
	{
		
		anim = GetComponent<Animator>();
		number_of_attacks = anim.GetInteger("num_of_attacks");
	}
	
	
	void Update ()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		
		float move = Input.GetAxis ("Vertical");
		anim.SetFloat("Speed", move);
		
		//gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, gameObject.transform.localPosition + (gameObject.transform.forward * move), Time.deltaTime*5f);
		
		if(Input.GetKeyDown(KeyCode.R) && stateInfo.nameHash == idle_state_hash)
		{
			print (gameObject.name + "is Ready");
			anim.SetTrigger (ready_hash);
		}
		else if(Input.GetKeyDown(KeyCode.R) && stateInfo.nameHash == ready_state_hash)
		{
			print (gameObject.name + "is Idle");
			anim.SetTrigger (idle_hash);
		}
		
		else if(Input.GetKeyDown(KeyCode.A)){
		
			print (gameObject.name + "is Attacking");
			int attack_value = Random.Range(0, number_of_attacks);
			anim.SetInteger("attack_style", attack_value);
			Debug.Log(attack_value);
			anim.SetTrigger (attack_hash);
		}
		
		else if(Input.GetKeyDown(KeyCode.D)){
			
			print (gameObject.name + "is Damaged");
			health -= 50;
			anim.SetInteger("Health", health);
			anim.SetTrigger (damage_hash);
		}
		else if(Input.GetKeyDown(KeyCode.G)){
			
			print (gameObject.name + "is Gathering");
			anim.SetTrigger (gather_hash);
		}

		
	}
}