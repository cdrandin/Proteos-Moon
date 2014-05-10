using UnityEngine;
using System.Collections;

public class MoonShatter : MonoBehaviour {

	/*void Awake(){
		MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}*/
	public GameObject explosion;
	public GameObject moon;
	public Transform explosionTransform;
	public GameObject brokenMoon;
	public GameObject mainMenu;
	public GameObject gameLogo;
	private bool skipped = false;

	void Awake(){
		//explosion.SetActive(false);
	}

	void Start () {
		StartCoroutine("RumbleMoon");
		StartCoroutine("MainMenuScene");
	}

	void Update(){
		if(Input.anyKey && !skipped){
			StopCoroutine("MainMenuScene");
			transform.localPosition = GameObject.Find("EndPosition").transform.localPosition;
			if(!brokenMoon.activeSelf){
				brokenMoon.SetActive(true);
				moon.SetActive(false);
			}
			mainMenu.SetActive(true);
			gameLogo.SetActive(true);
			skipped = true;
		}
	}

	IEnumerator RumbleMoon(){
		bool shake = true;
		float timer = 0.0f;
		//Quaternion from = transform.rotation * Quaternion.Euler(transform.forward * 20);
		//Quaternion to = transform.rotation * Quaternion.Euler(transform.forward * -20);
		do{
			//timer = Mathf.PingPong(Time.time * .05f, 1.0f);
			//transform.rotation = Quaternion.Slerp (transform.rotation * Quaternion.Euler(transform.forward - 5), transform.rotation * Quaternion.Euler(transform.forward + 5), timer);
			timer += Time.deltaTime;
			if (shake && timer >= .05f){
				moon.transform.localPosition = new Vector3(moon.transform.localPosition.x + 2.5f, moon.transform.localPosition.y, moon.transform.localPosition.z);
				shake = false;
				timer = 0.0f;
			}
			else if (!shake && timer >= .05f){
				moon.transform.localPosition = new Vector3(moon.transform.localPosition.x - 2.5f, moon.transform.localPosition.y, moon.transform.localPosition.z);
				shake = true;
				timer = 0.0f;
			}
			yield return null;
		}while(true);
	}

	IEnumerator MainMenuScene(){
		float startTime = Time.time;
		float duration = 10.0f;
		float elapsed;
		bool done = false;
		bool changed = false;
		Vector3 endPosition = GameObject.Find("EndPosition").transform.localPosition;
		Vector3 startPosition = GameObject.Find("StartPosition").transform.localPosition;
		
		do
		{ 	
			// calculate how far through we are
			elapsed = Time.time - startTime;
			if (elapsed >= 3.0f && !done){
				//StartCoroutine("ExplodeMoon");
				Instantiate(explosion, explosionTransform.localPosition, explosionTransform.localRotation);
				done = true;
			}
			if (elapsed >= 3.5f && !changed){
				moon.SetActive(false);
				brokenMoon.SetActive(true);
				changed = true;
				StopCoroutine("RumbleMoon");
			}
			float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
			this.transform.localPosition = Vector3.Lerp(startPosition, endPosition, normalisedTime);
			// wait for the next frame
			yield return null;
		}while(elapsed < duration);
		mainMenu.SetActive(true);
		gameLogo.SetActive(true);
	}
}
