using UnityEngine;
using System.Collections;

public class EndScene : MonoBehaviour
{
	
	
	public GUISkin skin;
	
	void Awake ()
	{
		if (PhotonNetwork.connected)
			PhotonNetwork.Disconnect ();
		
		Screen.lockCursor = false;
		Screen.showCursor = true;

       
	}

	void OnGUI ()
	{
		GUI.skin = skin;

        GUILayout.BeginArea(new Rect((Screen.width - 350) / 2, (Screen.height - 150) / 2, 350, 150));
		
		GUILayout.Label ("End of game", "Center");
        GUILayout.Label("Thank you for playing!", "Center");	
		
		GUILayout.Space (30);
		if (GUILayout.Button ("Play again")) {
			Application.LoadLevel (0);
		}
		if (GUILayout.Button ("Get Photon for Unity")) {
			Application.OpenURL ("http://u3d.as/content/exit-games/photon-unity-networking/2ey");
		}
		
		GUILayout.EndArea ();
	}
	
	
	/*void Update ()
	{
        if (Time.timeSinceLevelLoad>=3 && !PhotonNetwork.connected && PhotonNetwork.connectionStateDetailed != PeerState.Disconnecting)
            Application.LoadLevel(0);
	}*/
}
