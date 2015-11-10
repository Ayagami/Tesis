using UnityEngine;
using UnityEngine.Networking;

public class ColorControl : NetworkBehaviour
{
	static Color[] colors = new Color[] { Color.white, Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };

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

	public static LobbyGameMode staticMode = LobbyGameMode.Single;

	[SyncVar(hook="OnMyColor")]
	public Color myColor = Color.white;

	NetworkLobbyPlayer lobbyPlayer;
	PlayerLobby playerUI;

	private int indexMode = 0;

	void Awake()
	{
		lobbyPlayer = GetComponent<NetworkLobbyPlayer>();
		playerUI = GetComponent<PlayerLobby>();
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


	public void ClientChangeColor()
	{
		var newCol = colors[Random.Range(0,colors.Length)];
		CmdSetMyColor(newCol);
	}

	public void ServerChangeMode(){
		indexMode++;
		int q = (int)LobbyGameMode.Count;
		if (indexMode > q - 1)
			indexMode = 0;

		CmdSetMyMode (indexMode);
	}

	void OnMyColor(Color newColor)
	{
		myColor = newColor;
		playerUI.SetColor(newColor);
	}

	void OnMyMode(LobbyGameMode mode){
		currentMode = mode;
		staticMode = currentMode;
		playerUI.SetMode ((int)mode);
	}

	void Update()
	{
		if (!isLocalPlayer)
			return;
	}

	void FixedUpdate(){
		if (staticMode != currentMode)
			currentMode = staticMode;
	}
}
