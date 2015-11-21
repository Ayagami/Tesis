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
	public string level = "Default";

	[SyncVar]
	public bool GameModeSetted = false; 

	[Header("References of GameModes")]
	public GameObject flagPrefab;
	public List<Flag_Base> bases;


	public GameObject pointPrefab;


	private CaptureThePoint pointReference = null;

	[SyncVar]
	private bool ModeInitialized = false;
	
	private bool iAmServer = false;

	public Scrollbar sbar;
	private Image sbar_image;


	[Header("Objects from Level editor")]
	public GameObject[] prefabsFromLevelEditor;

	//[SyncVar]
	private List<Vector3> spawnsFromLevel;

	void Awake(){
		if(instance == null)
			instance = this;
		WhoWon = null;
		spawnsFromLevel = new List<Vector3> ();
		bases = new List<Flag_Base> ();
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

	void SpawnPlayersProperly(){
		/*if (Players.Count <= 0) {
			Invoke("SpawnPlayersProperly", 3f);
			return;
		}

		if (spawnsFromLevel.Count <= 0) {
			DebugConsole.Log("NO SPAWNS FOUND");
			return;
		}

		DebugConsole.Log ("Players..." + Players.Count);

		for (int i=0; i < Players.Count; i++) {
			Players[i].transform.position = spawnsFromLevel[Random.Range(0, spawnsFromLevel.Count)];
		}*/
	}

	/*
	public Vector3 GetRandomSpawnPoint(){
		if (spawnsFromLevel.Count <= 0) {
			DebugConsole.Log("NO SPAWNS");
			return new Vector3(0,5,0);
		}

		return spawnsFromLevel [Random.Range (0, spawnsFromLevel.Count)];
	}*/

	public Vector3 GetRandomSpawnPoint(){
		GameObject[] Spawns = GameObject.FindGameObjectsWithTag ("PlayerSpawnPoints");
		if (Spawns != null) {
			if(Spawns.Length > 0)
				return Spawns[Random.Range(0,Spawns.Length)].transform.position;
		}

		return new Vector3(0,10,0);
	}

	void Update(){

		if(!isEnabled){
				GameObject[] P = GameObject.FindGameObjectsWithTag("Player");
				foreach(GameObject player in P){
					Players.Add(player);
				}
				//SpawnPlayersProperly();
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

			for(int i=0; i < bases.Count; i++){
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
		if (!instance)
			return false;

		return instance.iAmServer;
	}

	public void setPlayer(string p){
		this.localPlayer = p;
	}

	public void setTeam(int team){
		this.localTeam = team;
	}

	protected void CmdDoLevelInitialization(){
		Level levelToLoad = null;

		switch (instance.level) {
			case "Default":
				levelToLoad = new Level();
				levelToLoad.LevelName = "Default";
				levelToLoad.obj = new List<SceneObject>();	

				SceneObject newObject = new SceneObject();
				newObject.prefab = SceneTypePrefab.COLLISEUM;
				newObject.pos = Vector3.zero;
				newObject.scale = new Vector3(0.5638874f, 0.5638874f, 0.5638874f);
				newObject.rot = Vector3.zero;
				
				levelToLoad.obj.Add(newObject);

				SceneObject PointObj = new SceneObject();
				PointObj.prefab = SceneTypePrefab.BASE_POINT;
				PointObj.pos = new Vector3(3.749407f, 1.8f, 2.18429f);
				PointObj.scale = new Vector3(10f, 2f, 10f);
				PointObj.rot = Vector3.zero;

				levelToLoad.obj.Add(PointObj);

				SceneObject Flag1 = new SceneObject();
				Flag1.prefab = SceneTypePrefab.BASE_FLAG;
				Flag1.pos = new Vector3(101.58f, 1f, 4.42f);
				Flag1.scale = new Vector3(15f, 15f, 15f);
				Flag1.rot = Vector3.zero;
				
				levelToLoad.obj.Add(Flag1);

				SceneObject Flag2 = new SceneObject();
				Flag2.prefab = SceneTypePrefab.BASE_FLAG;
				Flag2.pos = new Vector3(-96.5f, 1f, 2.77f);
				Flag2.scale = new Vector3(15f, 15f, 15f);
				Flag2.rot = Vector3.zero;
				
				levelToLoad.obj.Add(Flag2);

				SceneObject Spawn1 = new SceneObject();
				Spawn1.prefab = SceneTypePrefab.SPAWN_POINT;
				Spawn1.pos = new Vector3(0f, 10.0f, -8.5f);
				Spawn1.scale = new Vector3(1,1,1);
				Spawn1.rot = Vector3.zero;
				levelToLoad.obj.Add(Spawn1);
					

				break;
			default:
				levelToLoad = LevelParser.Load(instance.level);
				break;
		}

		CmdDoLevelSpawns (levelToLoad);

	}



	private void CmdDoLevelSpawns(Level newLevel){
		if (newLevel == null) {
			DebugConsole.Log("NO LEVEL FOUND");
			return;
		}

		for(int i=0; i < newLevel.obj.Count; i++){
				CmdSpawnObjectFromLevel((int)newLevel.obj[i].prefab, newLevel.obj[i].pos, newLevel.obj[i].rot, newLevel.obj[i].scale);
		}

	}

	void CmdSpawnObjectFromLevel(int type, Vector3 pos, Vector3 rot, Vector3 scale){
		GameObject go = null;
		switch ((SceneTypePrefab)type) {
			case SceneTypePrefab.COLLISEUM:
				go = Instantiate(prefabsFromLevelEditor[0]) as GameObject;
				go.name = "Colliseum";
			break;
			case SceneTypePrefab.BASE_FLAG:
				go = Instantiate(prefabsFromLevelEditor[1]) as GameObject;
				bases.Add(go.GetComponent<Flag_Base>());
				bases[bases.Count-1].desiredName = "Flag-Base " + Random.Range(0,100);
				go.name = bases[bases.Count-1].desiredName;
			break;
			case SceneTypePrefab.BASE_POINT:
				go = Instantiate(prefabsFromLevelEditor[2]) as GameObject;
				go.name = "Point-Base";
			break;
			case SceneTypePrefab.SPAWN_POINT:
				go = Instantiate(prefabsFromLevelEditor[3]) as GameObject;
				go.name = "Spawn-Point";
				spawnsFromLevel.Add(pos);
			break;
		}


		if (go) {
			go.transform.position = pos;
			go.transform.rotation = Quaternion.Euler (rot);
			go.transform.localScale = scale;
			go.GetComponent<SyncScaleForLevel>().desiredScale = scale;
			NetworkServer.Spawn (go);
		}

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
				for(int i=0; i < bases.Count; i++){
					DebugConsole.Log("BASE " + bases[i].desiredName);
					bases[i].Team = i;
					string iDFlag = "Flag_team" + i;
					CmdTellServerWhereToSpawnFlag(bases[i].transform.position, bases[i].transform.rotation.eulerAngles, bases[i].desiredName, i, iDFlag);
				}
			break;

			case GameType.CAPTURE_POINT:
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
		GameObject point = GameObject.FindGameObjectWithTag("PointLocation");
		go.transform.localScale = point.transform.localScale;
		go.GetComponent<CaptureThePoint> ().desiredScale = point.transform.localScale;
		go.GetComponent<Zombie_ID> ().zombieID = ID;
		go.name = "CaptureThePoint-" + ID;
		NetworkServer.Spawn (go);
	}

	void CmdTellServerWhereToSpawnFlag(Vector3 tPos, Vector3 tRot, string parent, int team, string ID){

		Flag_Base Base = GameObject.Find (parent).GetComponent<Flag_Base> ();

		GameObject go = Instantiate (flagPrefab, tPos, Quaternion.Euler (tRot)) as GameObject;

		Flag FC = go.GetComponent<Flag> ();

		go.GetComponent<Zombie_ID> ().zombieID = ID;

		go.name = "FlagTeam-" + ID;

		FC._base = Base;

		DebugConsole.Log (parent);

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

	public void SetMode(GameType gameMode, string levelName){
		if (GameModeSetted)
			return;

		GameModeSetted = true;

		instance.mode = gameMode;
		instance.level = levelName;

		if (isServer) {
			Debug.Log("Inside");
			CmdDoLevelInitialization();
			CmdDoModeInitialization ();
		}

	}

	public enum GameType{
		NORMAL,
		TEAM,
		CAPTURE_FLAG,
		CAPTURE_POINT,
		Count
	}
}
