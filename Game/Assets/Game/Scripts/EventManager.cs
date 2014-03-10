using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

	public delegate void ClickAction();
	public static event ClickAction OnClicked;

	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 210, 5, 100, 30), "Leave Room"))
		{
			PhotonNetwork.Destroy(GameObject.FindGameObjectWithTag("Player"));
			PhotonNetwork.LeaveRoom();
			//Application.LoadLevel("TitleScene");
		}
		if (GUI.Button(new Rect(Screen.width - 105, 5, 100, 30), "Attack"))
		{
			if(OnClicked != null)
				OnClicked();
		}
		if (GUI.Button(new Rect(Screen.width - 105, 40, 100, 30), "Defend"))
		{
			if(OnClicked != null)
				OnClicked();
		}
		if (GUI.Button(new Rect(Screen.width - 105, 75, 100, 30), "Ability"))
		{
			if(OnClicked != null)
				OnClicked();
		}
		if (GUI.Button(new Rect(Screen.width - 105, 110, 100, 30), "Wait"))
		{
			if(OnClicked != null)
				OnClicked();
		}
		if (GUI.Button(new Rect(Screen.width - 105, 145, 100, 30), "Summon"))
		{
			if(OnClicked != null)
				OnClicked();
		}
	}
}
