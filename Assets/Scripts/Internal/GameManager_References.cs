using UnityEngine;
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
	}

	void Update(){
			if(!isEnabled){
				GameObject[] P = GameObject.FindGameObjectsWithTag("Player");
				foreach(GameObject player in P){
					Players.Add(player);
				}
				isEnabled = true;
				CheckWinCondition ();
			}

		if (mode == GameType.NORMAL) {
			if (WhoWon != null) {
				if (WhoWon.name == localPlayer) {
					YouWin.SetActive (true);
					YouDie.SetActive (false);
				}
			}
		}

		if (mode == GameType.TEAM) {	/* Should be applied on CAPTURE_FLAG, CAPTURE_POINT Too.*/
			if(teamWon != -1){
				if(teamWon == localTeam){
					YouWin.SetActive(true);
					YouDie.SetActive(false);
				}
			}
		}
	}
	
	public void PlayerDies(string playerName){

		for (int i=0; i < Players.Count; i++) {
			if(Players[i].name == playerName){
				Players.Remove(Players[i]);
				break;
			}
		}

		CheckWinCondition ();
	}

	void CheckWinCondition(){
		switch (mode) {
			case GameType.NORMAL:
				if (Players.Count > 0 && Players.Count <= 1) {
					WhoWon = Players[0];
				}
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
				return;
			}

			Debug.Log("Players Left");
			for(int i=0; i < PlayersInTeam.Length; i++){
				Debug.Log(string.Format("Team {0}: {1}", i+1, PlayersInTeam[i]));
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
