﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetworkSetup : NetworkBehaviour {

	[SerializeField] Camera FPSCharacterCam;
	[SerializeField] AudioListener audioListener;

	public GameObject Prefab;

	public Transform myTrans;

    private CamaraJugador CameraInstance;

	[SyncVar] public Color TeamColor = Color.white;
	private Renderer rendererReference = null;

	public Transform FlagPosition = null;

	public bool GameStarted = false;

	// Use this for initialization
	public override void OnStartLocalPlayer () {
		if (isLocalPlayer) {
			GameObject.Find ("Scene Camera").SetActive (false);

			CharacterControllerLogic ccl = GetComponent<CharacterControllerLogic> ();
			ccl.enabled = false;

			ccl.PNS = this;

			GetComponentInChildren<Camera> ().enabled = true;

			Player_CameraManager PCM = GetComponentInChildren<Player_CameraManager>();
			PCM.enabled = true;

			CamaraJugador cg = GetComponentInChildren<CamaraJugador> ();
			cg.enabled = true;
			CameraInstance = cg;

			ccl.gamecam = cg;

			AudioListener AL = GetComponentInChildren<AudioListener> ();
			AL.enabled = true;

			Invoke ("InvokeMe", 5f);
		}
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (0, true);
	}

	public override void PreStartClient () {
		GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
	}

    public void onDieMessage() {
		if(isLocalPlayer)
       		 this.CameraInstance.enabled = false;
    }

	public void onAliveMessage(){
		if (isLocalPlayer) {

			CharacterControllerLogic ccl = GetComponent<CharacterControllerLogic> ();
			ccl.gamecam = CameraInstance;

			this.CameraInstance.enabled = true;
		}
	}
	/*
	[Command]
	void CmdLobby() { 
		var lobby = NetworkLobbyManager.singleton as NetworkLobbyManager;
		if (lobby) {
			NetworkManager.singleton.ServerChangeScene(lobby.lobbyScene);
			NetworkManager.singleton.StopHost();
		}
	}*/

	void InvokeMe(){
		Vector3 resp = GameManager_References.instance.GetRandomSpawnPoint ();
		DebugConsole.Log ("SPAWN RETURN " + resp);
		transform.position = resp;

		CharacterControllerLogic ccl = GetComponent<CharacterControllerLogic> ();
		ccl.enabled = true;

		GameStarted = true;
	}

	/*
	[ClientCallback]
	void OnGUI() {
		if (!isLocalPlayer) {
			return;
		}

		if (GUI.Button(new Rect(360, 30, 100, 20), "Exit")) {
				CmdLobby();
		}
	}*/

	
	void OnColorChanged(Color newColor){
		TeamColor = newColor;
		if (rendererReference == null)
			rendererReference = GetComponentInChildren<Renderer> ();
		
		if (rendererReference) {
			rendererReference.material.color = TeamColor;
		}
	}

	void FixedUpdate(){
		if (rendererReference == null) {
			rendererReference = GetComponentInChildren<Renderer> ();
		}

		if(rendererReference){
			if(rendererReference.material.color != TeamColor){
				rendererReference.material.color = TeamColor;
			}
		}
	}
}
