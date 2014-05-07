using UnityEngine;
using System.Collections;

public class FadeLogoIn : MonoBehaviour {
	public GUITexture Logo;
	//public float delay = 1.0f;
	// Use this for initialization
	void Start () {
		//float timer = 0.0f;
		//if (timer <= delay){
			//timer += Time.deltaTime;
			//return;
		//}
		//else{
			StartCoroutine("ChangeTransparency");
		//}
	}

	IEnumerator ChangeTransparency(){
		float startTime = Time.time;
		float duration = 3.0f;
		float elapsed;
		//float alpha;

		do
		{  // calculate how far through we are
			elapsed = Time.time - startTime;
			float normalisedTime = Mathf.Clamp(elapsed / duration, 0, 1);
			Logo.color = new Vector4(Logo.color.r,Logo.color.g, Logo.color.b, Mathf.Lerp(0.0f, 1.0f, normalisedTime));
			// wait for the next frame
			yield return null;
		}while(elapsed < duration);
	}
}
