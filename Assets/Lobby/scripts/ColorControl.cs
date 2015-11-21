using UnityEngine;
using UnityEngine.Networking;

public class ColorControl : NetworkBehaviour
{
	public static Color[] colors = new Color[] { Color.white, Color.blue, Color.green, Color.yellow };

	//static bool isGameModesEnabled = false;

	public enum LobbyGameMode{
		Single,
		Double,
		Flag,
		Point,
		Count
	}

	[SyncVar(hook="OnMyMode")]
	public LobbyGameMode currentMode = LobbyGameMode.Single;

	[SyncVar(hook="OnMyLevel")]
	public string currentLevel = "Default";

	public static LobbyGameMode staticMode = LobbyGameMode.Single;
	public static string staticLevel = "Default";

	[SyncVar(hook="OnMyColor")]
	public Color myColor = Color.white;

	NetworkLobbyPlayer lobbyPlayer;
	PlayerLobby playerUI;

	private int indexMode = 0;
	private int indexColor = 0;

	void Awake()
	{
		lobbyPlayer = GetComponent<NetworkLobbyPlayer>();
		playerUI = GetComponent<PlayerLobby>();
		//myColor = Color.red;
	}

	[Command]
	void CmdSetMyColor(Color col)
	{
		// cant change color after turning ready
		if (lobbyPlayer.readyToBegin)
		{
			return;
		}

		myColor = col;
	}

	[Command]
	void CmdSetMyMode(int mode){
		currentMode = (LobbyGameMode)mode;
	}

	[Command]
	void CmdSetMyLevel(string level){
		currentLevel = level;
	}

	public void ClientChangeColor()
	{
		indexColor++;
		int whatToConsider = (currentMode == LobbyGameMode.Flag || currentMode == LobbyGameMode.Point) ? 1 : colors.Length - 1;
		if (indexColor > whatToConsider)
			indexColor = 0;

		var newCol = colors[indexColor];
		CmdSetMyColor(newCol);
	}

	public void ServerChangeMode(){
		indexMode++;
		int q = (int)LobbyGameMode.Count;
		if (indexMode > q - 1)
			indexMode = 0;

		CmdSetMyMode (indexMode);
	}

	public void ServerChangeLevel(string newLevel){

		CmdSetMyLevel (newLevel);
	}

	void OnMyColor(Color newColor)
	{
		myColor = newColor;
		playerUI.SetColor(newColor);
	}

	void OnMyMode(LobbyGameMode mode){
		currentMode = mode;

		if ( (currentMode == LobbyGameMode.Flag || currentMode == LobbyGameMode.Point) && indexColor > 1) {
			OnMyColor(colors[0]);
			CmdSetMyColor (colors [0]);
		}

		staticMode = currentMode;
		playerUI.SetMode ((int)mode);
	}

	void OnMyLevel(string level){
		currentLevel = level;

		staticLevel = currentLevel;
		playerUI.SetLevel (level);
	}

	void Update()
	{
		if (!isLocalPlayer)
			return;
	}

	void FixedUpdate(){
		if (staticMode != currentMode)
			currentMode = staticMode;

		if (staticLevel != currentLevel)
			currentLevel = staticLevel;
	}
}
