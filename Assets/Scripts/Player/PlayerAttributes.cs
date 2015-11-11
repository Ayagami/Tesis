using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerAttributes : NetworkBehaviour {

	[SyncVar (hook = "OnHealthChanged")] private int health = 100;
	private Text healthText;

	[SyncVar]
	public int Team = -1;

	private bool initialized = false;


	void Start(){
		if (isLocalPlayer) {
			healthText = GameObject.Find("Health Text").GetComponent<Text>();
			SetHealthText();
		}
	}

	/*public override void OnStartLocalPlayer () {

	}
	*/
	
	// Update is called once per frame
	void Update () {
		if (!initialized && isLocalPlayer) {
			if(Team != -1){
			GameManager_References.setTeam (Team);
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
		}
	}

	[Command]
	public void CmdTellToServerPlayerDies(string playerName){
		GameManager_References.instance.PlayerDies (playerName);
		Debug.Log ("I'm Running");
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
}
