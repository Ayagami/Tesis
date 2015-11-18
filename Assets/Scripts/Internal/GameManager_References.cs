using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager_References : NetworkBehaviour {

	public GameObject YouWin;
	public GameObject YouDie;

	public static GameManager_References instance = null;

	private List<GameObject> Players = null;
	private List<GameObject> PlayersDied = null;

	[SyncVar]
	private GameObject WhoWon = null;

	[SyncVar]
	private int teamWon = -1;

	private bool isEnabled = false;

	private string localPlayer = "";
	private int localTeam = -1;

	[SyncVar]
	public GameType mode = GameType.NORMAL;

	[SyncVar]
	public bool GameModeSetted = false; 

	[Header("References of GameModes")]
	public GameObject flagPrefab;
	public Flag_Base[] bases;


	public GameObject pointPrefab;


	private CaptureThePoint pointReference = null;

	[SyncVar]
	private bool ModeInitialized = false;
	
	private bool iAmServer = false;

	public Scrollbar sbar;
	private Image sbar_image;

	void Awake(){
		if(instance == null)
			instance = this;
		WhoWon = null;
	}

	void Start(){
		Players = new List<GameObject> ();
		PlayersDied = new List<GameObject> ();

		Time.timeScale = 1;
		WhoWon = null;

		if(sbar){
			sbar_image  = sbar.GetComponent<Image>();
		}

	}

	public void SetPointReference(CaptureThePoint re){
		pointReference = re;
		sbar.gameObject.SetActive (true);
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
				else{
					YouWin.SetActive(false);
					YouDie.SetActive(true);
				}
			}
		}

		if (pointReference) {
			sbar_image.color = pointReference.ColorReference();
			sbar.value = pointReference.Score / 100;
		}

	}
	
	public void PlayerDies(string playerName){
		for (int i=0; i < Players.Count; i++) {
			if(Players[i].name == playerName){
				GameObject player = Players[i];
				Players.Remove(player);
				PlayersDied.Add(player);
				break;
			}
		}
		CheckWinCondition ();
	}

	public void PlayerAlive(string playerName){
		for (int i=0; i < PlayersDied.Count; i++) {
			if(PlayersDied[i].name == playerName){
				GameObject player = PlayersDied[i];
				PlayersDied.Remove(player);
				Players.Add(player);
			}
		}

		CheckWinCondition ();
	}

	public int TeamWon(){
		return teamWon;
	}

	[ClientRpc]
	void RpcRecieveWhoWon(string playerName){
		for (int i=0; i < Players.Count; i++) {
			if(Players[i].name == playerName){
				WhoWon = Players[i];
				break;
			}
		}
		DebugConsole.Log("WhoWon: " + WhoWon.name + " , Me " + localPlayer);
	}

	[ClientRpc]
	void RpcRecieveWhoTeamWon(int team){
		if (localTeam == -1)
			localTeam = PlayerAttributes.playerInstance.Team;

		DebugConsole.Log ("Team Won " + team + " , my team: " + localTeam);
		this.teamWon = teamWon;
	}

	//[ClientRpc]
	/*void RpcEnablePointBase(){
		if (pointBase) {
			pointBase.gameObject.SetActive(true);
		}
	}*/

	[ServerCallback]
	void sendWhoWon(){
		if (!isServer && !ImServer())
			return;

		if (isServer) {
			if(mode == GameType.NORMAL)
				RpcRecieveWhoWon (WhoWon.name);
			else
				RpcRecieveWhoTeamWon(teamWon);
		}
	}

	public void PointModeWinner(int Team){
		if (!isServer && !ImServer())
			return;

		if (teamWon != -1)
			return;

		DebugConsole.Log ("Team Won " + Team);
		teamWon = Team;

		sendWhoWon ();
	}

	void CheckWinCondition(){
		if (!isServer)
			return;

		switch (mode) {
			case GameType.NORMAL:

				if(Players.Count == 0){
					GameObject[] Possibles = GameObject.FindGameObjectsWithTag("Player");	// esto me va a devolver los Objetos habilitados.. por ende los muertos no cuentan (?
					Players.Clear();
					for(int i=0; i < Possibles.Length; i++){
						if( PlayersDied.IndexOf(Possibles[i]) == -1)
							Players.Add(Possibles[i]);
					}
				}

				if (Players.Count > 0 && Players.Count <= 1) {
					WhoWon = Players[0];
					if(isServer){
						sendWhoWon();
					}
				}
				
			break;

			case GameType.TEAM:
			int[] PlayersInTeam = new int[4];

			if(Players.Count == 0){
				GameObject[] Possibles = GameObject.FindGameObjectsWithTag("Player");	// esto me va a devolver los Objetos habilitados.. por ende los muertos no cuentan (?
				Players.Clear();
				for(int i=0; i < Possibles.Length; i++){
					if( PlayersDied.IndexOf(Possibles[i]) == -1)
						Players.Add(Possibles[i]);
				}
			}


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

			for(int i=0; i < bases.Length; i++){
				if(bases[i].Score >= 3){	// Ese team ganó.
					teamWon = bases[i].Team;
					sendWhoWon();
					break;
				}
			}

			break;
		}

	}

	public void AddScoreToTeamFlag(int team){
		if (!isServer)
			return;

		int correctedTeam = team == 1 ? 0 : 1;

		DebugConsole.Log ("Adding score to team " + correctedTeam);
		this.bases [correctedTeam].Score+=1;
		DebugConsole.Log ("Score : " + instance.bases [correctedTeam].Score);
		this.CheckWinCondition ();
	}

	public void ReSpawnFlagWhenPlayersDisconnect(int Team, string Parent){
		if (!ImServer())
			return;

		DebugConsole.Log ("SPAWNING");

		Flag_Base currentBase = bases [Team];
		string iDFlag = "Flag_Team" + Team;

		CmdTellServerWhereToSpawnFlag (currentBase.transform.position, currentBase.transform.root.eulerAngles, currentBase.name, Team, iDFlag);

	}

	public static bool ImServer(){
		return instance.iAmServer;
	}

	public void setPlayer(string p){
		this.localPlayer = p;
	}

	public void setTeam(int team){
		this.localTeam = team;
	}
	
	protected void CmdDoModeInitialization(){
		Debug.Log ("CMD");
		if (ModeInitialized)
			return;

		iAmServer = true;

		switch (this.mode) {
			case  GameType.NORMAL:
			case  GameType.TEAM:
			break;

			case GameType.CAPTURE_FLAG:
				Debug.Log("TEST");
				GameObject[] Bases = GameObject.FindGameObjectsWithTag("Flag_Bases");
				DebugConsole.Log("HEY HAY... " + Bases.Length);
				bases = new Flag_Base[Bases.Length];

				for(int i=0; i < Bases.Length; i++){
					bases[i] = Bases[i].GetComponent<Flag_Base>();
					bases[i].Team = i;
					string iDFlag = "Flag_team" + i;
					if(isServer)
						CmdTellServerWhereToSpawnFlag(Bases[i].transform.position, Bases[i].transform.rotation.eulerAngles, Bases[i].name, i, iDFlag);
				}
			break;

			case GameType.CAPTURE_POINT:
				/*if(pointBase){
					pointBase.gameObject.SetActive(true);
					//RpcEnablePointBase();
				}*/

				GameObject point = GameObject.FindGameObjectWithTag("PointLocation");
				string id = "Point-" + Random.Range(0,10);
				if(isServer){
					CmdTellServerWhereToSpawnPoint(point.transform.position, point.transform.rotation.eulerAngles, id);
				}
			break;
		}
	}

	void CmdTellServerWhereToSpawnPoint(Vector3 tPos, Vector3 tRot, string ID){
		GameObject go = Instantiate (pointPrefab, tPos, Quaternion.Euler (tRot)) as GameObject;
		go.GetComponent<Zombie_ID> ().zombieID = ID;
		go.name = "CaptureThePoint-" + ID;
		NetworkServer.Spawn (go);

	}
	void CmdTellServerWhereToSpawnFlag(Vector3 tPos, Vector3 tRot, string parent, int team, string ID){

		Debug.Log ("Spawning " + ID);
		DebugConsole.Log ("Spawning " + ID);

		Flag_Base Base = GameObject.Find (parent).GetComponent<Flag_Base> ();

		GameObject go = Instantiate (flagPrefab, tPos, Quaternion.Euler (tRot)) as GameObject;

		Flag FC = go.GetComponent<Flag> ();

		go.GetComponent<Zombie_ID> ().zombieID = ID;

		go.name = "FlagTeam-" + ID;

		FC._base = Base;
		FC.Flag_Base = parent;

		FC.Team = team;

		Base.Team = team;

		NetworkServer.Spawn (go);
	}

	public static void FindInstance(){
		if (instance == null) {
			GameObject GO = GameObject.Find ("GameManager");
			if(GO){
				GameManager_References GMR = GO.GetComponent<GameManager_References>();
				instance = GMR;
			}
		}
	}

	public void SetMode(GameType gameMode){
		if (GameModeSetted)
			return;

		GameModeSetted = true;

		Debug.Log ("Setting Mode...");
		Debug.Log (gameMode);

		instance.mode = gameMode;

		if (isServer) {
			Debug.Log("Inside");
			CmdDoModeInitialization ();
		}


	}
	/*

	[Command]
	void CmdLobby() { //This code only runs on Server! So... we need to log-out the user.
		var lobby = NetworkLobbyManager.singleton as NetworkLobbyManager;
		if (lobby) {
			NetworkManager.singleton.ServerChangeScene(lobby.lobbyScene);
			NetworkManager.singleton.StopHost();
		}
	}

	public void Exit() {
		CmdLobby();
	}*/

	public enum GameType{
		NORMAL,
		TEAM,
		CAPTURE_FLAG,
		CAPTURE_POINT,
		Count
	}
}
