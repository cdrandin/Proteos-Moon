using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using ExitGames.Client.Photon.LoadBalancing;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class DemoGame2 : LoadBalancingClient
{
	public string ErrorMessageToShow { get; set; }
	
	//internal protected string[] playerProps = new string[] { "skill", "rank", "guild" };
	//internal protected string[] roomProps = new string[] { "map", "skill", "mode" };
	
	private const byte MaxPlayers = 2;
	
	public int turnNumber = 1;
	public int lastTurnPlayer;
	
	//public const byte EvTileClick = 1;
	//public List<List<int>> lastTilesClicked = new List<List<int>>();
	//public int evCount = 0;
	
	public Dictionary<string, int> SavedGames = new Dictionary<string, int>();
	
	
	// overriding the CreatePlayer "factory" provides us with custom DemoPlayers (that also know their position)
	protected internal override Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
	{
		return new DemoPlayer(actorName, actorNumber, isLocal, actorProperties);
	}
	
	public override void OnOperationResponse(OperationResponse operationResponse)
	{
		
		base.OnOperationResponse(operationResponse);
		this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull());    // log as ERROR to make sure it's not filtered out due to log level
		
		switch (operationResponse.OperationCode)
		{
		case (byte)OperationCode.WebRpc:
                if (operationResponse.ReturnCode == 0)
                {
                    this.OnWebRpcResponse(new WebRpcResponse(operationResponse));
                }
                break;
		case (byte)OperationCode.JoinGame:
		case (byte)OperationCode.CreateGame:
			if (this.Server == ServerConnection.GameServer)
			{
				if (operationResponse.ReturnCode == 0)
				{
					this.UpdateBoard();
				}
			}
			break;
		case (byte)OperationCode.JoinRandomGame:
			if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
			{
				// no room found: we create one!
				this.CreateTurnbasedRoom();
			}
			break;
		}
	}
	
	private void OnWebRpcResponse(WebRpcResponse response)
	{
		if (response.ReturnCode == 0)
		{
			this.SavedGames.Clear();
			if (response.Name.Equals("GetGameList") && response.Parameters != null)
			{
				foreach (KeyValuePair<string, object> pair in response.Parameters)
				{
					this.SavedGames.Add(pair.Key, int.Parse(pair.Value as string));
				}
			}
		}
	}
	
	/*public void SendTileClickEv(int index)
	{
		Debug.Log("Send Tile Click");
		Hashtable content = new Hashtable();
		content[(byte)1] = this.turnNumber;
		content[(byte)2] = index;
		this.loadBalancingPeer.OpRaiseEvent(EvTileClick, true, content, 0, EventCaching.AddToRoomCache, null, ReceiverGroup.Others, 0, false);
		
		while (turnNumber >= this.lastTilesClicked.Count)
		{
			this.lastTilesClicked.Add(new List<int>());
		}
		this.lastTilesClicked[turnNumber].Add(index);
	}
	
	public void ClearTileClickEvForTurn(int turnToDelete)
	{
		Debug.Log("Clean Tile Click for Turn " + turnToDelete);
		Hashtable content = new Hashtable();
		content[(byte)1] = turnToDelete;
		this.loadBalancingPeer.OpRaiseEvent(EvTileClick, content, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.Others);
		this.lastTilesClicked[turnToDelete].Clear();
	}
	
	public void ClearAllTileClickEv()
	{
		Debug.Log("Clean All Tile Click");
		this.loadBalancingPeer.OpRaiseEvent(EvTileClick, null, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.Others);
		this.lastTilesClicked.Clear();
	}*/
	
	public override void OnEvent(EventData photonEvent)
	{
		base.OnEvent(photonEvent);
		
		switch (photonEvent.Code)
		{
		//case (byte)EvTileClick:
			//object content = photonEvent.Parameters[ParameterCode.CustomEventContent];
			//Hashtable turnClick = content as Hashtable;
			//if (turnClick != null)
			//{
				//int turnNumber = (int)turnClick[(byte)1];
				/*int clickedTile = (int)turnClick[(byte)2];
				
				
				while (turnNumber >= this.lastTilesClicked.Count)
				{
					this.lastTilesClicked.Add(new List<int>());
				}
				this.lastTilesClicked[turnNumber].Add(clickedTile);
				this.evCount++;
				Debug.Log("got click ev. tile: " + clickedTile + " turn: " + turnNumber);*/
			//}
			//break;
			
		case EventCode.PropertiesChanged:
			DebugReturn(DebugLevel.ALL, "Got Properties via Event. Update Board by room props.");
			this.UpdateBoard();
			break;
		}
	}
	
	public override void DebugReturn(DebugLevel level, string message)
	{
		base.DebugReturn(level, message);
		Debug.Log(message);
	}
	
	public override void OnStatusChanged(StatusCode statusCode)
	{
		base.OnStatusChanged(statusCode);
		
		switch (statusCode)
		{
		case StatusCode.Exception:
		case StatusCode.ExceptionOnConnect:
			Debug.LogWarning("Exception on connection level. Is the server running? Is the address (" + this.MasterServerAddress+ ") reachable?");
			break;
		case StatusCode.Disconnect:
			HideBoard();
			break;
		}
	}
	
	public void SaveBoardAsProperty()
	{
		//CubeBoard board = GameObject.FindObjectOfType<CubeBoard>();
		this.turnNumber = this.turnNumber + 1;
		this.lastTurnPlayer = this.LocalPlayer.ID;
		
		//Hashtable boardProps = board.GetBoardAsCustomProperties();
		//boardProps.Add("lt", this.lastTurnPlayer);  // "lt" is for "last turn" and contains the ID/actorNumber of the player who did the last one
		//boardProps.Add("t#", this.turnNumber);
		
		//this.OpSetCustomPropertiesOfRoom(boardProps);
		
		//Debug.Log("saved board to props " + SupportClass.DictionaryToString(boardProps));
	}
	
	public void UpdateBoard()
	{
		// we set properties "lt" (last turn) and "t#" (turn number). those props might have changed
		// it's easier to use a variable in gui, so read the latter property now
		if (this.CurrentRoom.CustomProperties.ContainsKey("t#"))
		{
			this.turnNumber = (int) this.CurrentRoom.CustomProperties["t#"];
		}
		else
		{
			this.turnNumber = 1;
		}
		if (this.CurrentRoom.CustomProperties.ContainsKey("lt"))
		{
			this.lastTurnPlayer = (int) this.CurrentRoom.CustomProperties["lt"];
		}
		else
		{
			this.lastTurnPlayer = 0;    // unknown
		}
		
		//CubeBoard board = GameObject.FindObjectOfType<CubeBoard>();
		//if (!board.enabled)
		//{
		//    board.enabled = true;
		//    board.ResetTileValues();
		//}
		
		//Hashtable roomProps = this.CurrentRoom.CustomProperties;
		//bool success = board.SetBoardByCustomProperties(roomProps);
		//Debug.Log("loaded board from room props. Success: " + success);
		
		//board.ShowCubes();
	}
	
	public void HideBoard()
	{
		//CubeBoard board = GameObject.FindObjectOfType<CubeBoard>();
		//if (board.enabled) board.enabled = false;
		//this.lastTilesClicked.Clear();
	}
	
	public void CreateTurnbasedRoom()
	{
		string newRoomName = this.PlayerName + "-" +Random.Range(0,1000).ToString("D4");    // for int, Random.Range is max-exclusive!
		Debug.Log("CreateTurnbasedRoom(): "+newRoomName);
		
		this.OpCreateRoom(newRoomName, true, true, MaxPlayers, null, new string[] {"t#"}, null, LobbyType.Default, int.MaxValue, 5000);
	}
}
