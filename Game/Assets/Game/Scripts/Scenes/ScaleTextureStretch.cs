using UnityEngine;
using System.Collections;

public class ScaleTextureStretch : MonoBehaviour {

	private GUITexture background;
	private TitleGUI titleGUI;
	
	void Awake()
	{
		background = GetComponent<GUITexture>();
		titleGUI = GetComponent<TitleGUI>();
	}

	// Use this for initialization
	void Start()
	{
		background.pixelInset = new Rect(0, 0, Screen.width, Screen.height);

		// Enable Buttons
		titleGUI.enabled = true;
	}
}
