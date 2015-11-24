using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScreenLockState : NetworkBehaviour {
	private bool Locked = true;

	[Command]
	void CmdLobby() { 
		var lobby = NetworkLobbyManager.singleton as NetworkLobbyManager;
		if (lobby) {
			NetworkManager.singleton.ServerChangeScene(lobby.lobbyScene);
			NetworkManager.singleton.StopHost();
		}
	}

	void Update(){
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Locked = !Locked;
		}

	}

	[ClientCallback]
	void OnGUI() {
		if (!isLocalPlayer) {
			return;
		}

		if (!Locked) {
			if (GUI.Button (new Rect (360, 30, 100, 20), "Exit")) {
				CmdLobby ();
			}
		
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;

		} else {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
