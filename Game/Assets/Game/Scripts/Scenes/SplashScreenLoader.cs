

using UnityEngine;
using System.Collections;

public class SplashScreenLoader : MonoBehaviour {
	public float delay_time = 3;
	public bool done_loading = false;

	private float timer;


	// Use this for initialization
	void Start () {
		timer = delay_time;
		StartCoroutine("SomeFunction");
	}
	
	// Update is called once per frame
	void Update () {
		if(timer > 0){
			timer -= Time.deltaTime;
			return;
		}
		if(done_loading){
			Application.LoadLevel(Application.loadedLevel + 1);
		}
	}

	IEnumerator SomeFunction(){
		// Do Something Here

		// Some yield operation until it has completed
		yield return null;

		// if we did what we wanted and got all done without error
		done_loading = true;
	}
}
