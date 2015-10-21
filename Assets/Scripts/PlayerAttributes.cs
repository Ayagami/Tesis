using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class PlayerAttributes : NetworkBehaviour {

	[SyncVar (hook = "OnHealthChanged")] private int health = 100;
	private Text healthText;

	public override void OnStartLocalPlayer ()
	{
		healthText = GameObject.Find("Health Text").GetComponent<Text>();
		SetHealthText();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//CheckIfINeedToDie ();

	}

	void CheckIfINeedToDie(){
		if (health <= 0) {
		  	Debug.Log("Die");

            /*
                Reviso si tengo que deshabilitar componentes... Que obviamente debo hacer. xD!
             *  Posibilidad de usar sendMessage();
             */

            SendMessage("onDieMessage");
			CmdTellToServerPlayerDies(this.name, (int) GameManager_References.GameType.NORMAL);
            if(isLocalPlayer)
			    GameManager_References.instance.YouDie.SetActive(true);
            this.gameObject.SetActive(false);
		}
	}

	[Command]
	public void CmdTellToServerPlayerDies(string playerName, int GType){
		GameManager_References.instance.PlayerDies (playerName, GType);
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
