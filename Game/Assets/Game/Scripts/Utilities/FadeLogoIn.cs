using UnityEngine;
using System.Collections;

public class FadeLogoIn : MonoBehaviour {
	public GUITexture Logo;

	void Start () {
		StartCoroutine("ChangeTransparency");
	}

	IEnumerator ChangeTransparency(){
		float startTime = Time.time;
		float duration = 5.0f;
		float elapsed;

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
