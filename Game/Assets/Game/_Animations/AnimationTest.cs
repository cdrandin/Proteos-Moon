using UnityEngine;
using System.Collections;

public class AnimationTest : MonoBehaviour {

	AnimationTriggers anim;

	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponentInChildren<AnimationTriggers>()	;
	}
	
	// Update is called once per frame
	void Update () {
	
		anim.MoveAnimation(Input.GetAxis("Vertical"));
	
		if(Input.GetKeyDown(KeyCode.R)){
		
			print ("Rest");
			anim.ReadyAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.E)){
			print ("Idle");
			anim.IdleAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.T)){
			
			print ("Gather");
			anim.GatherAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.Y)){
			
			print ("Attack");
			anim.AttackAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.U)){
			
			print ("Damage");
			anim.DamageAnimation(50);
		}
		else if(Input.GetKeyDown(KeyCode.I)){
			
			print ("Death");
			anim.DamageAnimation(0);
		}
	}
}
