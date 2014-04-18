/*
 * MMGM.cs
 * 
 * Mark Martene
 */

using UnityEngine;
using System.Collections;

/* GameManager - keeps track of the game state and control
 * Controlling the game 
 *  X Check when next player's turn should be or when current player passes their turn
 *  X Keep track of rounds, turn order, timer, player's units resources, leaders, each player's units, who won, etc
 *  X Control recruit position, next to leader
 *  - Should pause game or unpause
 *  -(MAYBE) Keep track of network between players, if we add *networking* capabilities
 * 
 *  -(MAYBE) Keep track of buffs, debuff, passive that should be received from other units
 * 
 *  X Record stats at the end of the game
 *   	# of kills, resource collected/spent, rounds, timer?, # of units recruited,(?ranking algorithm?)
 */

public class mmGM : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
