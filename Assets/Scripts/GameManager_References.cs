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

	private bool isEnabled = false;

	private string localPlayer = "";

	void Awake(){
		instance = this;
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
			}

		if (WhoWon != null) {
			if(WhoWon.name == localPlayer){
				YouWin.SetActive(true);
				//Time.timeScale = 0;
			}
		}

			
	}

	public void PlayerDies(string playerName, int GType){

		for (int i=0; i < Players.Count; i++) {
			if(Players[i].name == playerName){
				Players.Remove(Players[i]);
				break;
			}
		}

		CheckWinCondition (GType);
	}

	void CheckWinCondition(int GType){
		if (Players.Count > 0 && Players.Count <= 1) {
			WhoWon = Players[0];
		}
	}

	public static void setPlayer(string p){
		instance.localPlayer = p;
	}

	public enum GameType{
		NORMAL,
		Count
	}
}
