using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class PlayerCanvasHooks : MonoBehaviour
{
	public delegate void CanvasHook();

	public CanvasHook OnReadyHook;
	public CanvasHook OnColorChangeHook;
	public CanvasHook OnRemoveHook;
	public CanvasHook OnModeChangeHook;

	public Button playButton;
	public Button colorButton;
	public Button removeButton;
	public Button ModeButton;


	public Text modeText;
	public Text readyText;
	public Text nameText;

	public RectTransform panelPos;

	bool isLocalPlayer;

	public bool isTheServer = false;


	public ColorControl.LobbyGameMode mode = ColorControl.LobbyGameMode.Single;

	public static PlayerCanvasHooks ServerCanvas = null;
	public PlayerLobby LobbyData = null;


	void Awake() {
		removeButton.gameObject.SetActive(false);
		colorButton.interactable = false;
		ModeButton.interactable = false;

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

	public void SetLocalPlayer()
	{
		isLocalPlayer = true;
		nameText.text = "YOU";
		readyText.text = "Play";
		if (isTheServer)
			ModeButton.interactable = true;
		removeButton.gameObject.SetActive(true);
	}

	public void SetColor(Color color)
	{
		colorButton.GetComponent<Image>().color = color;
	}

	public void SetMode(int Smode){

		ColorControl.LobbyGameMode parsedMode = (ColorControl.LobbyGameMode)Smode;
		ServerCanvas.mode = parsedMode;

		switch (parsedMode) {
		case ColorControl.LobbyGameMode.Single:
			modeText.text = "Mode:Single";

			colorButton.interactable = false;
			break;
		case ColorControl.LobbyGameMode.Double:
			modeText.text = "Mode:Double";

			colorButton.interactable = true;
			break;
		case ColorControl.LobbyGameMode.Point:
			modeText.text = "Mode:Point";

			colorButton.interactable = true;
			break;
		case ColorControl.LobbyGameMode.Flag:
			modeText.text = "Mode:Flag";

			colorButton.interactable = true;
			break;
		}

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

		SetMode ((int)ServerCanvas.mode);
	}
}
