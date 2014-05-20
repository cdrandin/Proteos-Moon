using UnityEngine;
using System.Collections;

public class MoonShatter : MonoBehaviour {

	public GameObject explosion;
	public GameObject fireball;
	public Transform fireballTransform;
	public GameObject moon;
	public Transform explosionTransform;
	public GameObject brokenMoon;
	public GameObject mainMenu;
	public GameObject gameLogo;
	private bool skipped = false;
	private Vector3 startPosition, endPosition;
	private Transform fireballOffset;

	void Awake(){
		endPosition = GameObject.Find("EndPosition").transform.localPosition;
		startPosition = GameObject.Find("StartPosition").transform.localPosition;
	}
	void Start () {
		InvokeRepeating("ShakeLeft", 0, .1f);
		InvokeRepeating("ShakeRight", .05f, .1f);
		StartCoroutine("MainMenuScene");
		Invoke("DestroyFireballs", 25.0f);
	}

	void Update(){
		if(Input.anyKey && !skipped){
			StopCoroutine("MainMenuScene");
			StopCoroutine("ShootFireballs");
			CancelInvoke();
			transform.localPosition = endPosition;
			if(!brokenMoon.activeSelf){
				brokenMoon.SetActive(true);
				moon.SetActive(false);
			}
			mainMenu.SetActive(true);
			gameLogo.SetActive(true);
			Invoke ("DestroyFireballs", 0.0f);
			skipped = true;
		}
	}

	void DestroyFireballs(){
		GameObject[] fireballs = GameObject.FindGameObjectsWithTag("Fireball");
		if (fireballs != null){
			for (int i = 0; i < fireballs.Length; i++){
				Destroy(fireballs[i]);
			}
		}
	}

	void ShakeLeft(){
		moon.transform.localPosition = new Vector3(moon.transform.localPosition.x + 2.5f, moon.transform.localPosition.y, moon.transform.localPosition.z);
	}

	void ShakeRight(){
		moon.transform.localPosition = new Vector3(moon.transform.localPosition.x - 2.5f, moon.transform.localPosition.y, moon.transform.localPosition.z);
	}

	void DetonateExplosion(){
		Instantiate(explosion, explosionTransform.localPosition, explosionTransform.localRotation);
	}

	IEnumerator ShootFireballs(){
		fireballOffset = fireballTransform;
		for(int i = 0; i < 5; i++){
			fireballOffset.localEulerAngles = RandomizeFireballs();//new Vector3(offsetX, offsetY, offsetZ);
			Instantiate(fireball, fireballTransform.localPosition, fireballOffset.localRotation);
		}
		yield return null;
	}

	void ChangeMoon(){
		moon.SetActive(false);
		brokenMoon.SetActive(true);
		CancelInvoke("ShakeLeft");
		CancelInvoke("ShakeRight");
		Instantiate(fireball, fireballTransform.localPosition, fireballTransform.localRotation);
		StartCoroutine("ShootFireballs");
	}

	Vector3 RandomizeFireballs(){
		return (new Vector3(Random.Range(fireballTransform.localEulerAngles.x -15, fireballTransform.localEulerAngles.x + 16),
							Random.Range(fireballTransform.localEulerAngles.y -15, fireballTransform.localEulerAngles.y + 16),
							Random.Range(fireballTransform.localEulerAngles.z -15, fireballTransform.localEulerAngles.z + 16)));
	}
	IEnumerator MainMenuScene(){
		float startTime = Time.time;
		float duration = 10.0f;
		float elapsed;
		// Calls thsese functions after a delay
		Invoke("DetonateExplosion", 3.0f);
		Invoke("ChangeMoon", 3.5f);
		do
		{ 	
			// Calculate how far through we are
			elapsed = Time.time - startTime;
			float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
			// Move Camera backwards to it's final position
			this.transform.localPosition = Vector3.Lerp(startPosition, endPosition, normalisedTime);
			yield return null;
		}while(elapsed < duration);

		// Activates the MainMenu and the fade in of the logo
		mainMenu.SetActive(true);
		gameLogo.SetActive(true);
	}
}
