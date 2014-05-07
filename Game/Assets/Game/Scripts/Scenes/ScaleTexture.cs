using UnityEngine;
using System.Collections;

public class ScaleTexture : MonoBehaviour {

	public GUITexture background;
	private TitleGUI titleGUI;
	
	void Awake()
	{
		background = GetComponent<GUITexture>();
		//titleGUI = GetComponent<TitleGUI>();
	}

	// Use this for initialization
	void Start()
	{
		// Position the billboard in the center, 
		// but respect the picture aspect ratio
		int textureHeight = guiTexture.texture.height;
		int textureWidth = guiTexture.texture.width;
		int screenHeight = Screen.height;
		int screenWidth = Screen.width;
		
		int screenAspectRatio = (screenWidth / screenHeight);
		int textureAspectRatio = (textureWidth / textureHeight);
		
		int scaledHeight;
		int scaledWidth;
		if (textureAspectRatio <= screenAspectRatio)
		{
			// The scaled size is based on the height
			scaledHeight = screenHeight;
			scaledWidth = (screenHeight * textureAspectRatio);
		}
		else
		{
			// The scaled size is based on the width
			scaledWidth = screenWidth;
			scaledHeight = (scaledWidth / textureAspectRatio);
		}
		float xPosition = screenWidth / 2 - (scaledWidth / 2);
		background.pixelInset = 
			new Rect(xPosition, (float)( screenHeight - scaledHeight ) / 2.0f,  
			         scaledWidth, scaledHeight);

		// Enable Buttons
		//titleGUI.enabled = true;
	}
}
