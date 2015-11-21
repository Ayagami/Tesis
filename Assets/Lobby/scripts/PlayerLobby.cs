using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.EventSystems;

public class PlayerLobby : NetworkLobbyPlayer
{
	public Canvas playerCanvasPrefab;

	public Canvas playerCanvas;

	// cached components
	ColorControl cc;
	NetworkLobbyPlayer lobbyPlayer;

	public ColorControl CC() { return cc; }

	void Awake()
	{
		cc = GetComponent<ColorControl>();
		lobbyPlayer = GetComponent<NetworkLobbyPlayer>();
		playerCanvas = null;
	}

	public override void OnClientEnterLobby()
	{
		if (playerCanvas == null)
		{
			playerCanvas = (Canvas)Instantiate(playerCanvasPrefab, Vector3.zero, Quaternion.identity);
			playerCanvas.sortingOrder = 1;
		}

		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.panelPos.localPosition = new Vector3(GetPlayerPos(lobbyPlayer.slot), 0, 0);
		hooks.SetColor(cc.myColor);
		hooks.SetReady(lobbyPlayer.readyToBegin);

		EventSystem.current.SetSelectedGameObject(hooks.colorButton.gameObject);
	}

	public override void OnClientExitLobby()
	{
		if (playerCanvas != null)
		{
			Destroy(playerCanvas.gameObject);
		}
	}

	public override void OnClientReady(bool readyState)
	{
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.SetReady(readyState);
	}

	float GetPlayerPos(int slot)
	{
		var lobby = NetworkManager.singleton as GuiLobbyManager;
		if (lobby == null)
		{
			// no lobby?
			return slot * 200;
		}

		// this spreads the player canvas panels out across the screen
		var screenWidth = playerCanvas.pixelRect.width;
		screenWidth -= 200; // border padding
		var playerWidth = screenWidth / (lobby.maxPlayers-1);
		return -(screenWidth / 2) + slot * playerWidth;
	}

	public override void OnStartLocalPlayer()
	{
		if (playerCanvas == null)
		{
			playerCanvas = (Canvas)Instantiate(playerCanvasPrefab, Vector3.zero, Quaternion.identity);
			playerCanvas.sortingOrder = 1;
		}

		// setup button hooks
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.panelPos.localPosition = new Vector3(GetPlayerPos(lobbyPlayer.slot), 0, 0);
		hooks.SetColor(cc.myColor);
		hooks.OnColorChangeHook = OnGUIColorChange;
		hooks.OnReadyHook = OnGUIReady;
		hooks.OnRemoveHook = OnGUIRemove;
		hooks.OnModeChangeHook = OnGUIChangeMode;
		hooks.onLevelChangeHook = OnGUIChangeLevel;
		hooks.isTheServer = isServer;
		hooks.LobbyData = this;

		hooks.SetLocalPlayer();
	}

	void OnDestroy()
	{
		if (playerCanvas != null)
		{
			Destroy(playerCanvas.gameObject);
		}
	}

	public void SetColor(Color color)
	{
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.SetColor(color);
	}

	public void SetMode(int mode){
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks> ();
		hooks.SetMode (mode);
	}

	public void SetLevel(string level){
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks> ();
		hooks.SetLevel (level);
	}

	public void SetReady(bool ready)
	{
		var hooks = playerCanvas.GetComponent<PlayerCanvasHooks>();
		hooks.SetReady(ready);
	}

	[Command]
	public void CmdExitToLobby()
	{
		var lobby = NetworkManager.singleton as GuiLobbyManager;
		if (lobby != null)
		{
			lobby.ServerReturnToLobby();
		}
	}

	// events from UI system

	void OnGUIColorChange()
	{
		if (isLocalPlayer)
			cc.ClientChangeColor();
	}

	void OnGUIReady()
	{
		if (isLocalPlayer)
			lobbyPlayer.SendReadyToBeginMessage();
	}

	void OnGUIRemove()
	{
		if (isLocalPlayer)
		{
			ClientScene.RemovePlayer(lobbyPlayer.playerControllerId);

			var lobby = NetworkManager.singleton as GuiLobbyManager;
			if (lobby != null)
			{
				lobby.SetFocusToAddPlayerButton();
			}
		}
	}

	void OnGUIChangeMode(){
		if (isLocalPlayer && isServer) {
			cc.ServerChangeMode();
		}
	}

	void OnGUIChangeLevel(){
		if (isLocalPlayer && isServer) {
			PlayerCanvasHooks PCH = playerCanvas.GetComponent<PlayerCanvasHooks>();
			int val = PCH.dropdownLevel.value;

			string res = "Default";
			if(val != 0){
				res = PCH.levels[val-1];
			}

			cc.ServerChangeLevel(res);
		}
	}
}

