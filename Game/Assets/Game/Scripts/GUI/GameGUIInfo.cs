using UnityEngine;
using System.Collections;

public class GameGUIInfo : MonoBehaviour {

	public GUISkin mySkin;
	public GUIStyle textColor;

	// Use this for initialization
	void Start () {
		textColor.normal.textColor = Color.white;
		textColor.alignment = TextAnchor.MiddleCenter;
	}
	
	void OnGUI(){
	
		GUI.skin = mySkin;
		//GUI.skin.label.fontSize = Screen.height/32;

		GUI.Label(new Rect(0, 0*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("I am {0}", (Player)GM.instance.Photon_Leader.GetPhotonView().owner.ID-1), textColor);
		GUI.Label(new Rect(0, 1*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("{0}'s Turn ", GM.instance.CurrentPlayer),textColor);
		GUI.Label(new Rect(0, 2*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("Is it my turn {0}", GM.instance.IsItMyTurn()), textColor);
		GUI.Label(new Rect(0, 3*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("Current round: {0}", GM.instance.CurrentRound), textColor);
		//GUI.Label(new Rect(0, 4*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("Current time: {0}", GM.instance.CurrentTime), textColor);
		GUI.Label(new Rect(0, 4*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("Current resource: {0}/{1}",
		                                                                                          GM.instance.GetResourceFrom(GM.instance.WhichPlayerAmI),
		                                                                                          GM.instance.MaxResourceLimit), textColor);

		if(GM.instance.IsThereAWinner())
		{
			GUI.Label(new Rect(0, 8*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("Winner: {0}", GM.instance.Winner), textColor);
		}
		else
		{
			GUI.Label(new Rect(0, 8*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("DEFEAT!"), textColor);
		}

	}

	// Update is called once per frame
	void Update () {
		Debug.Log(GameObject.FindGameObjectsWithTag("Leader"));
	}
}
