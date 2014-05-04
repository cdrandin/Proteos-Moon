using UnityEngine;
using System.Collections;

public class GameGUIInfo : MonoBehaviour {

	public GUISkin mySkin;

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI(){
	
		GUI.skin = mySkin;
		
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		//GUI.skin.label.fontSize = Screen.height/32;

		GUI.Label(new Rect(0, 0*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("I am {0}", (Player)GM.instance.Photon_Leader.GetPhotonView().owner.ID-1));
		GUI.Label(new Rect(0, 1*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("{0}'s Turn ", GM.instance.CurrentPlayer));
		GUI.Label(new Rect(0, 2*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("First {0} then {1} Turn ", GM.instance.TurnOrder[0], GM.instance.TurnOrder[1]));
		GUI.Label(new Rect(0, 3*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("Is it my turn {0}", GM.instance.IsItMyTurn()));
		GUI.Label(new Rect(0, 4*Screen.height/32, Screen.width, Screen.height/32 ), string.Format("IsNextPlayersTurn: {0}", GM.instance.IsNextPlayersTurn()));
		
	}
	
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
