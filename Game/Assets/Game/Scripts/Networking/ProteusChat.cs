using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class ProteusChat : Photon.MonoBehaviour 
{
    public Rect GuiRect = new Rect(0,0, 400, 300);
    public bool IsVisible = true;
    public bool AlignBottom = true;
    public List<string> messages = new List<string>();
    private string inputLine = "";
	private char[] arr = new char[] { '\r', '\n', ' ' };
	private Vector2 scrollPos = new Vector2(Mathf.Infinity, Mathf.Infinity);
	public GUISkin skin;
	private GUIStyle chatStyle;
    public static readonly string ChatRPC = "Chat";
	public static readonly string GameChatRPC = "GameChat";

	void Awake(){
		QuickConnect qc;
		qc = this.GetComponent<QuickConnect>();
		if (qc.enabled){
			this.enabled = false;
		}
	}

    public void Start()
    {
		chatStyle = skin.FindStyle("ChatStyle");
        if (this.AlignBottom)
        {
            this.GuiRect.y = Screen.height - (this.GuiRect.height + 8);
        }

    }
	public void OnJoinedRoom(){
		//this.inputLine = PhotonNetwork.playerName + " joined the room";
		//this.messages.Add(PhotonNetwork.playerName + " joined the room");
		//this.photonView.RPC ("Chat", PhotonTargets.All, this.inputLine);
		//this.inputLine = "";
		this.photonView.RPC ("GameChat", PhotonTargets.All, "Joined");
		//
		//GUI.FocusControl("");
	}

	public void OnLeftRoom(){
		//this.photonView.RPC("GameChat", PhotonTargets.All, "LeftRoom");
	}

	/*public void OnPhotonPlayerDisconnected(){
		this.photonView.RPC("GameChat", PhotonTargets.All, "Disconnected");
	}

	public void OnPhotonPlayerConnected(){
		this.photonView.RPC("GameChat", PhotonTargets.All, "Connected");
	}*/


    public void OnGUI()
    {
		GUI.skin = skin;
        if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
        {
			if (PhotonNetwork.connectionStateDetailed == PeerState.Joined && (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))) {
				this.IsVisible = true;
				GUI.FocusControl("ChatInput");
			}
			return;
        }
        
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.inputLine))
            {
				this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
				this.inputLine = "";
                GUI.FocusControl("");
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            }
            else
			{
				GUI.FocusControl("ChatInput");
			}
        }

		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape) {
			this.inputLine = "";
			this.IsVisible = false;
			GUI.FocusControl("");
		}

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.GuiRect);
        GUILayout.BeginScrollView(scrollPos);
        GUILayout.FlexibleSpace();
        for (int i = 0; i <= messages.Count - 1; i++)
        {
			//if (messages.Count != null)
            	//GUILayout.Label(messages[i]);
			//else
				GUILayout.Label(messages[i], chatStyle);
        }
        GUILayout.EndScrollView();
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatInput");
		inputLine = inputLine.TrimStart(arr);
        inputLine = GUILayout.TextArea(inputLine, 180);
        if ((GUILayout.Button("Send", GUILayout.ExpandWidth(false))) && inputLine != "")
        {
            this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
			this.inputLine = "";
            GUI.FocusControl("");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    [RPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        string senderName = "anonymous";

        if (mi != null && mi.sender != null)
        {
            if (!string.IsNullOrEmpty(mi.sender.name))
            {
                senderName = mi.sender.name;
            }
            else
            {
                senderName = "player " + mi.sender.ID;
            }
        }

        this.messages.Add(senderName + ": " + newLine);
    }

	[RPC]
	public void GameChat(string newLine, PhotonMessageInfo mi) {
		string senderName = "anonymous";

		if (mi != null && mi.sender != null)
		{
			if (!string.IsNullOrEmpty(mi.sender.name))
			{
				senderName = mi.sender.name;
			}
			else
			{
				senderName = "player " + mi.sender.ID;
			}
		}
		if (newLine == "Joined"){
			this.messages.Add(senderName + " joined the room");
		}
		else if (newLine == "LeftRoom"){
			this.messages.Add(senderName + " has left the room");
		}
		else if (newLine == "Disconnected"){
			this.messages.Add(senderName + " has been disconnected");
		}
		else if (newLine == "Connected"){
			this.messages.Add(senderName + " has been reconnected");
		}
		else if (newLine == "Ready"){
			this.messages.Add(senderName + " is ready");
		}

	}

    public void AddLine(string newLine)
    {
        this.messages.Add(newLine);
    }
}
