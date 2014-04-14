using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Photon.MonoBehaviour
{

    public GUISkin skin;
    public Transform playerPrefab;

    public static Transform localPlayer;
    public static List<Transform> players = new List<Transform>();

    public static Transform GetClosestPlayer(Vector3 fromPos)
    {

        Transform close = null;
        float dist = -1;
        foreach (Transform tra in players)
        {
            if (tra == null)
            {
                continue;
            }
            float thisD = Vector3.Distance(tra.position, fromPos);
            if (dist == -1 || thisD < dist)
            {
                dist = thisD;
                close = tra;
            }
        }
        return close;
    }


    public static void AddPlayer(Transform tra)
    {
        players.Add(tra);
    }
    public static void RemovePlayer(Transform tra)
    {
        players.Remove(tra);
    }

    void Awake()
    {
        if (Application.loadedLevel < 1)
        {
            Debug.LogError("Configuration error: You have not yet added any scenes to your buildsettings. The current scene should be preceded by the mainmenu scene. Please see the README file for instructions on setting up the buildsettings.");
            return;
        }
        //PhotonNetwork.sendRateOnSerialize = 10;
        //PhotonNetwork.logLevel = NetworkLogLevel.Full;

        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected || PhotonNetwork.room == null)
        {
            Application.LoadLevel(0);
            return;
        }

        PhotonNetwork.isMessageQueueRunning = true;
        //Spawn our local player
        GameObject GO = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity, 0);
        localPlayer = GO.transform;

    }

    void OnGUI()
    {
        GUI.skin = skin;
        GameGUI();
    }

    bool showDebug = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            showDebug = !showDebug;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Check if the game is allowed to click here (for starting FIRE etc.)
    /// </summary>
    /// <returns></returns>
    public static bool GameCanClickHere()
    {

        List<Rect> rects = new List<Rect>();
        rects.Add(new Rect(0, 0, 110, 55));//Topleft Button
        rects.Add(new Rect(0, Screen.height - 35, 275, 35));//Chat

        Vector2 pos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        foreach (Rect re in rects)
        {
            if (re.Contains(pos))
                return false;
        }
        return true;

    }

    void GameGUI()
    {
        GUILayout.Space(32);
        if (GUILayout.Button("Leave"))
        {
            PhotonNetwork.LeaveRoom();
            Application.LoadLevel(Application.loadedLevel + 1);
        }

        if (showDebug)
        {
            GUILayout.Label("isMasterClient: " + PhotonNetwork.isMasterClient);
            GUILayout.Label("Players: " + PhotonNetwork.playerList.Length);
            GUILayout.Label("Ping: " + PhotonNetwork.GetPing());
        }
    }
}
