using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This simple chat example showcases the use of RPC targets and targetting certain players via RPCs.
/// </summary>
public class Chat : Photon.MonoBehaviour
{
	public GUISkin skin;
    public static Chat SP;
    List<Message> messages = new List<Message>();

    class Message
    {
        public string text;
        public PhotonPlayer player;
    }

    private int chatHeight = (int)155;
    private Vector2 scrollPos = Vector2.zero;
    private string chatInput = "";
    private float lastUnfocusTime = 0;

    void Awake()
    {
        SP = this;
    }

    void OnGUI ()
	{        
		
		GUI.skin = skin;
		GUI.SetNextControlName ("");

		GUILayout.BeginArea (new Rect (10, Screen.height - chatHeight - 10, Screen.width, chatHeight));
        
		//Show scroll list of chat messages
		scrollPos = GUILayout.BeginScrollView (scrollPos);		
        for (int i = 5; i > messages.Count; i--)
        {
            GUILayout.Space(20);
        } 
        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i].player == PhotonNetwork.player)
                GUI.color = Color.gray;
            else
                GUI.color = Color.red;
			GUILayout.Label (messages[i].text);
		}       
		GUILayout.EndScrollView ();
		GUI.color = Color.white;

		//Chat input
		GUILayout.BeginHorizontal (); 
		GUI.SetNextControlName ("ChatField");
		chatInput = GUILayout.TextField (chatInput, GUILayout.MinWidth (200));
       
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n') {
			if (GUI.GetNameOfFocusedControl () == "ChatField") {                
				SendChat (PhotonTargets.All);
				lastUnfocusTime = Time.time;
				GUI.FocusControl ("");
				GUI.UnfocusWindow ();
			} else
            {
                if (lastUnfocusTime < Time.time - 0.1f)
                {
                    GUI.FocusControl("ChatField");
                }
            }
		}

        if (GUILayout.Button("Send", GUILayout.Height(20)))
            SendChat(PhotonTargets.All);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    

        GUILayout.EndArea();
	}

    public static void AddMessage(string text, PhotonPlayer player)
    {
        Message mes = new Message();
        mes.text = text;
        mes.player = player;

        SP.messages.Add(mes);
        if (SP.messages.Count > 5)
            SP.messages.RemoveAt(0);
    }


    [RPC]
    void SendChatMessage(string text, PhotonMessageInfo info)
    {
        AddMessage("[" + info.sender + "] " + text, info.sender);
    }

    void SendChat(PhotonTargets target)
    {
        if (chatInput != "")
        {
            photonView.RPC("SendChatMessage", target, chatInput);
            chatInput = "";
        }
    }

    void SendChat(PhotonPlayer target)
    {
        if (chatInput != "")
        {
            chatInput = "[PM] " + chatInput;
            photonView.RPC("SendChatMessage", target, chatInput);
            chatInput = "";
        }
    }

    void OnLeftRoom()
    {
        this.enabled = false;
    }

    void OnJoinedRoom()
    {
        this.enabled = true;
    }
    void OnCreatedRoom()
    {
        this.enabled = true;
    }
}
