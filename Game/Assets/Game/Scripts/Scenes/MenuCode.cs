using UnityEngine;
using System.Collections;

public class MenuCode : Photon.MonoBehaviour
{

	void OnGUI(){
		
		GUILayout.BeginArea(new Rect(Screen.width / 2 - 225, 0, 450, Screen.height));
		
		GUILayout.FlexibleSpace();
		
		/*GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Select a scene");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();*/
		
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		
		
		GUILayout.BeginVertical();
		if (GUILayout.Button("Begin Story"))
		{
			Application.LoadLevel("Generic");
		}
		if (GUILayout.Button("Options"))
		{
			Application.LoadLevel("GameManagerTest");
		}
		if (GUILayout.Button("Multiplayer"))
		{
			Application.LoadLevel("Networking");
		}
		GUILayout.Space(10);
		

		
		
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
		
	}
}
