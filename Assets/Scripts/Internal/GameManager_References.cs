﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameManager_References : NetworkBehaviour {

	public GameObject YouWin;
	public GameObject YouDie;

	public static GameManager_References instance = null;

	private List<GameObject> Players = null;

	[SyncVar]
	private GameObject WhoWon = null;


	[SyncVar]
	private int teamWon = -1;

	private bool isEnabled = false;

	private string localPlayer = "";
	private int localTeam = -1;

	public GameType mode = GameType.NORMAL;

	void Awake(){
		instance = this;
		WhoWon = null;
	}

	void Start(){
		Players = new List<GameObject> ();
		Time.timeScale = 1;
		WhoWon = null;
	}

	void Update(){
			if(!isEnabled){
				GameObject[] P = GameObject.FindGameObjectsWithTag("Player");
				foreach(GameObject player in P){
					Players.Add(player);
				}
				isEnabled = true;
			}

		if (mode == GameType.NORMAL) {
			if (WhoWon != null) {
				if (WhoWon.name == localPlayer) {
					YouWin.SetActive (true);
					YouDie.SetActive (false);
				}
			}
		}

		if (mode == GameType.TEAM || mode == GameType.CAPTURE_FLAG || mode == GameType.CAPTURE_POINT) {	/* Should be applied on CAPTURE_FLAG, CAPTURE_POINT Too.*/
			if(teamWon != -1 && localTeam != -1){
				if(teamWon == localTeam){
					YouWin.SetActive(true);
					YouDie.SetActive(false);
				}
			}
		}
	}
	
	public void PlayerDies(string playerName){
		Debug.Log ("Player Dies");
		for (int i=0; i < Players.Count; i++) {
			if(Players[i].name == playerName){
				Debug.Log("REMOVING PLAYER");
				Players.Remove(Players[i]);
				break;
			}
		}

		CheckWinCondition ();
	}

	[ClientRpc]
	void RpcRecieveWhoWon(string playerName){
		for (int i=0; i < Players.Count; i++) {
			if(Players[i].name == playerName){
				WhoWon = Players[i];
				break;
			}
		}
	}
	[ClientRpc]
	void RpcRecieveWhoTeamWon(int team){
		this.teamWon = teamWon;
	}

	[ServerCallback]
	void sendWhoWon(){
		if (isServer) {
			if(mode == GameType.NORMAL)
				RpcRecieveWhoWon (WhoWon.name);
			else
				RpcRecieveWhoTeamWon(teamWon);
		}
	}

	void CheckWinCondition(){
		Debug.Log ("CHECKING CONDITION");
		switch (mode) {
			case GameType.NORMAL:
				Debug.Log("NORMAL");
				Debug.Log(Players.Count);
				if (Players.Count > 0 && Players.Count <= 1) {
					WhoWon = Players[0];
				}
				if(isServer)
					sendWhoWon();
			break;

			case GameType.TEAM:
			int[] PlayersInTeam = new int[4];
			for(int i=0; i < Players.Count; i++){
				PlayersInTeam[Players[i].GetComponent<PlayerAttributes>().Team]++;
			}

			int howMuchTeamsAreWith1OrMore = 0;
			for(int i=0; i < PlayersInTeam.Length; i++) {
				if(PlayersInTeam[i] > 0)
					howMuchTeamsAreWith1OrMore++;
			}

			if(howMuchTeamsAreWith1OrMore==1){	// Hay un equipo ganador.
				teamWon = Players[0].GetComponent<PlayerAttributes>().Team;
				Debug.Log("TEAM WON" + teamWon);
				sendWhoWon();
				return;
			}

			Debug.Log("Players Left");
			for(int i=0; i < PlayersInTeam.Length; i++){
				Debug.Log(string.Format("Team {0}: {1}", i+1, PlayersInTeam[i]));
			}
			break;

			case GameType.CAPTURE_FLAG:
			for(int i=0; i < Players.Count; i++){
				Debug.Log("Player " + i + " team = " + Players[i].GetComponent<PlayerAttributes>().Team);
			}
			break;
		}

	}

	public static void setPlayer(string p){
		instance.localPlayer = p;
	}

	public static void setTeam(int team){
		instance.localTeam = team;
	}

	public enum GameType{
		NORMAL,
		TEAM,
		CAPTURE_FLAG,
		CAPTURE_POINT,
		Count
	}
}
