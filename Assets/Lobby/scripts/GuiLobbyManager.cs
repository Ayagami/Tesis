using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class GuiLobbyManager : NetworkLobbyManager
{
	public LobbyCanvasControl lobbyCanvas;
	public OfflineCanvasControl offlineCanvas;
	public OnlineCanvasControl onlineCanvas;
	//public ExitToLobbyCanvasControl exitToLobbyCanvas;
	public ConnectingCanvasControl connectingCanvas;
	public PopupCanvasControl popupCanvas;
	public MatchMakerCanvasControl matchMakerCanvas;
	public JoinMatchCanvasControl joinMatchCanvas;

	public string onlineStatus;
	static public GuiLobbyManager s_Singleton;


	private bool needToSendGameMode = false;
	private GameManager_References.GameType gType = GameManager_References.GameType.NORMAL;

	void Start() {
		s_Singleton = this;
		offlineCanvas.Show();
	}


	void FixedUpdate(){
		if (needToSendGameMode) {
			if(GameManager_References.instance != null){
				GameManager_References.instance.SetMode(gType);
				needToSendGameMode = false;
			}
		}
	}

	void OnLevelWasLoaded()
	{
		if (lobbyCanvas != null) lobbyCanvas.OnLevelWasLoaded();
		if (offlineCanvas != null) offlineCanvas.OnLevelWasLoaded();
		if (onlineCanvas != null) onlineCanvas.OnLevelWasLoaded();
		//if (exitToLobbyCanvas != null) exitToLobbyCanvas.OnLevelWasLoaded();
		if (connectingCanvas != null) connectingCanvas.OnLevelWasLoaded();
		if (popupCanvas != null) popupCanvas.OnLevelWasLoaded();
		if (matchMakerCanvas != null) matchMakerCanvas.OnLevelWasLoaded();
		if (joinMatchCanvas != null) joinMatchCanvas.OnLevelWasLoaded();
	}

	public void SetFocusToAddPlayerButton()
	{
		if (lobbyCanvas == null)
			return;

		lobbyCanvas.SetFocusToAddPlayerButton();
	}

	// ----------------- Server callbacks ------------------

	public override void OnLobbyStopHost()
	{
		lobbyCanvas.Hide();
		offlineCanvas.Show();
	}

	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		//This hook allows you to apply state data from the lobby-player to the game-player
		//var cc = lobbyPlayer.GetComponent<ColorControl>();
		//var playerX = gamePlayer.GetComponent<Player>();
		//playerX.myColor = cc.myColor;


		var cc = lobbyPlayer.GetComponent<ColorControl> ();
		if ((GameManager_References.GameType)cc.currentMode != GameManager_References.GameType.NORMAL) {
			var playerAttributes = gamePlayer.GetComponent<PlayerAttributes> ();
			int team = 0;
			for (int i=0; i < ColorControl.colors.Length; i++) {
				if (ColorControl.colors [i] == cc.myColor) {
					team = i;
					break;
				}
			}
			playerAttributes.Team = team;
			gamePlayer.GetComponent<Player_NetworkSetup> ().TeamColor = cc.myColor;
			gamePlayer.GetComponentInChildren<Renderer> ().material.color = cc.myColor;
		}

		if (GameManager_References.instance == null) {
			GameManager_References.FindInstance();
		}
		if (GameManager_References.instance)
			GameManager_References.instance.SetMode ((GameManager_References.GameType)cc.currentMode);
		else {
			needToSendGameMode = true;
			gType = (GameManager_References.GameType)cc.currentMode;
		}
		return true;
	}

	// ----------------- Client callbacks ------------------

	public override void OnLobbyClientConnect(NetworkConnection conn)
	{
		connectingCanvas.Hide();
	}

	public override void OnClientError(NetworkConnection conn, int errorCode)
	{
		connectingCanvas.Hide();
		StopHost();

		popupCanvas.Show("Client Error", errorCode.ToString());
	}

	public override void OnLobbyClientDisconnect(NetworkConnection conn)
	{
		lobbyCanvas.Hide();
		offlineCanvas.Show();
	}

	public override void OnLobbyStartClient(NetworkClient client)
	{
		if (matchInfo != null)
		{
			connectingCanvas.Show(matchInfo.address);
		}
		else
		{
			connectingCanvas.Show(networkAddress);
		}
	}

	public override void OnLobbyClientAddPlayerFailed()
	{
		popupCanvas.Show("Error", "No more players allowed.");
	}

	public override void OnLobbyClientEnter()
	{
		lobbyCanvas.Show();
		onlineCanvas.Show(onlineStatus);

		//exitToLobbyCanvas.Hide();

	}

	public override void OnLobbyClientExit()
	{
		lobbyCanvas.Hide();
		onlineCanvas.Hide();

		if (Application.loadedLevelName == base.playScene)
		{
			//exitToLobbyCanvas.Show();
		}
	}
}
