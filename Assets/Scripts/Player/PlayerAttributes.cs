﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerAttributes : NetworkBehaviour {

	[SyncVar (hook = "OnHealthChanged")] private int health = 100;
	private Text healthText;

	[SyncVar]
	public int Team = -1;

	private bool initialized = false;


	public static PlayerAttributes playerInstance = null;

	void Start(){
		if (isLocalPlayer) {
			healthText = GameObject.Find("Health Text").GetComponent<Text>();
			SetHealthText();
			playerInstance = this;
		}
	}

	/*public override void OnStartLocalPlayer () {

	}
	*/
	
	// Update is called once per frame
	void Update () {
		if (!initialized && isLocalPlayer) {
			if(Team != -1){
				GameManager_References.instance.setTeam (Team);
				playerInstance = this;
				initialized = true;
			}
		}
	}


	void CheckIfINeedToDie(){
		if (health <= 0) {
		  	Debug.Log("Die");

            /*
                Reviso si tengo que deshabilitar componentes... Que obviamente debo hacer. xD!
             *  Posibilidad de usar sendMessage();
             */

            SendMessage("onDieMessage", SendMessageOptions.DontRequireReceiver);

			CmdTellToServerPlayerDies(this.name);

            if(isLocalPlayer)
			    GameManager_References.instance.YouDie.SetActive(true);

            this.gameObject.SetActive(false);
		
			if(GameManager_References.instance.mode == GameManager_References.GameType.CAPTURE_FLAG || GameManager_References.instance.mode == GameManager_References.GameType.CAPTURE_POINT && isLocalPlayer){
				Invoke("AliveEvent", 3f);
			}
		}
	}

	[Command]
	public void CmdTellToServerPlayerDies(string playerName){
		GameManager_References.instance.PlayerDies (playerName);
	}

	[Command]
	public void CmdTellToServerPlayerAlive(string playerName){
		GameManager_References.instance.PlayerAlive (playerName);
	}

	void SetHealthText() {
		if(isLocalPlayer) {
			healthText.text = "Health " + health.ToString();
		}
	}

	public void TakeDamage(int damage){
		health -= damage;
	}

	void OnHealthChanged(int hlth) {
		health = hlth;
		if (health <= 0)
			health = 0;
        if (health >= 100)
            health = 100;
		SetHealthText();
        CheckIfINeedToDie();
	}

	void AliveEvent(){
		this.gameObject.SetActive (true);
		SendMessage ("onAliveMessage", SendMessageOptions.DontRequireReceiver);
		TakeDamage (-100);

		if (GameManager_References.instance.TeamWon () == -1 && isLocalPlayer)
			GameManager_References.instance.YouDie.SetActive (false);

		CmdTellToServerPlayerAlive (this.name);
	}
}
