using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerCanvasHooks : MonoBehaviour
{
	public delegate void CanvasHook();

	public CanvasHook OnReadyHook;
	public CanvasHook OnColorChangeHook;
	public CanvasHook OnRemoveHook;
	public CanvasHook OnModeChangeHook;
	public CanvasHook onLevelChangeHook;

	public Button playButton;
	public Button colorButton;
	public Button removeButton;
	public Button ModeButton;
	public Dropdown dropdownLevel;


	public Text modeText;
	public Text readyText;
	public Text nameText;

	public RectTransform panelPos;

	bool isLocalPlayer;

	public bool isTheServer = false;


	public ColorControl.LobbyGameMode mode = ColorControl.LobbyGameMode.Single;
	public string level = "Default";

	public static PlayerCanvasHooks ServerCanvas = null;
	public PlayerLobby LobbyData = null;

	public List<string> levels;


	void Awake() {
		removeButton.gameObject.SetActive(false);
		colorButton.interactable = false;
		ModeButton.interactable = false;
		dropdownLevel.interactable = false;

		if (ServerCanvas == null) {
			ServerCanvas = this;
			ServerCanvas.name = "ServerPlayerCanvas";
		}
	}

	public void UIReady()
	{
		if (OnReadyHook != null)
			OnReadyHook.Invoke();
	}

	public void UIColorChange()
	{
		if (OnColorChangeHook != null)
			OnColorChangeHook.Invoke();
	}

	public void UIRemove()
	{
		if (OnRemoveHook != null)
			OnRemoveHook.Invoke();
	}

	public void UIGameMode(){
		if (OnModeChangeHook != null)
			OnModeChangeHook.Invoke ();
	}

	public void UILevelMode(){
		if (onLevelChangeHook != null)
			onLevelChangeHook.Invoke ();
	}

	public void SetLocalPlayer() {
		isLocalPlayer = true;
		nameText.text = "YOU";
		readyText.text = "Play";
		if (isTheServer) {
			ModeButton.interactable = true;
			dropdownLevel.interactable = true;

			Dropdown.OptionData last = new Dropdown.OptionData ("Default");
			dropdownLevel.options.Add( last );

			this.levels = LevelParser.LoadLevelList();
			for(int i=0; i < levels.Count; i++){
				Dropdown.OptionData option = new Dropdown.OptionData (levels[i]);
				dropdownLevel.options.Add( option );
			}

			dropdownLevel.value = 0;
			dropdownLevel.GetComponentInChildren<Text>().text = "Default";
		}
		removeButton.gameObject.SetActive(false);
	}

	public void SetColor(Color color) {
		colorButton.GetComponent<Image>().color = color;
	}

	public void SetMode(int Smode){

		ColorControl.LobbyGameMode parsedMode = (ColorControl.LobbyGameMode)Smode;
		ServerCanvas.mode = parsedMode;

		switch (parsedMode) {
		case ColorControl.LobbyGameMode.Single:
			modeText.text = "Mode:Single";
			if(isTheServer)
				GuiLobbyManager.s_Singleton.setMaxPlayers(4);
			colorButton.interactable = false;
			break;
		case ColorControl.LobbyGameMode.Double:
			modeText.text = "Mode:Double";
			if(isTheServer)
				GuiLobbyManager.s_Singleton.setMaxPlayers(4);
			colorButton.interactable = true;
			break;
		case ColorControl.LobbyGameMode.Point:
			modeText.text = "Mode:Point";
			if(isTheServer)
				GuiLobbyManager.s_Singleton.setMaxPlayers(6);
			colorButton.interactable = true;
			break;
		case ColorControl.LobbyGameMode.Flag:
			modeText.text = "Mode:Flag";
			if(isTheServer)
				GuiLobbyManager.s_Singleton.setMaxPlayers(4);
			colorButton.interactable = true;
			break;
		}
	}

	public void SetLevel(string level){
		ServerCanvas.level = level;

		dropdownLevel.GetComponentInChildren<Text> ().text = level;
	}

	public void SetReady(bool ready)
	{
		if (ready)
		{
			readyText.text = "Ready";
		}
		else
		{
			if (isLocalPlayer)
			{
				readyText.text = "Play";
			}
			else
			{
				readyText.text = "Not Ready";
			}
		}
	}

	void Update(){
		if (!isLocalPlayer)
			return;

		if (ServerCanvas == null)
			return;

		if (ServerCanvas == this)
			return;

		mode = ServerCanvas.mode;
		LobbyData.CC ().currentMode = mode;

		level = ServerCanvas.level;
		LobbyData.CC ().currentLevel = level;

		SetLevel (ServerCanvas.level);

		SetMode ((int)ServerCanvas.mode);
	}
}
