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
	
		if(Input.GetKeyDown(KeyCode.R)){
		
			anim.ReadyAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.E)){
		
			anim.IdleAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.T)){
			
			anim.GatherAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.Y)){
			
			anim.AttackAnimation();
		}
		else if(Input.GetKeyDown(KeyCode.U)){
			
			anim.DamageAnimation(50);
		}
		else if(Input.GetKeyDown(KeyCode.I)){
			
			anim.DamageAnimation(0);
		}
	}
}
