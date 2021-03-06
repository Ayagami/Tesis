﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetworkSetup : NetworkBehaviour {

	[SerializeField] Camera FPSCharacterCam;
	[SerializeField] AudioListener audioListener;

	public GameObject Prefab;

	public Transform myTrans;

    private CamaraJugador CameraInstance;
	// Use this for initialization
	public override void OnStartLocalPlayer ()
	{
		if (isLocalPlayer) {
			GameObject.Find ("Scene Camera").SetActive (false);
			//GetComponent<CharacterController>().enabled = true;
			//GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
			CharacterControllerLogic ccl = GetComponent<CharacterControllerLogic> ();
			ccl.enabled = true;

			GetComponentInChildren<Camera> ().enabled = true;

			Player_CameraManager PCM = GetComponentInChildren<Player_CameraManager>();
			PCM.enabled = true;

			CamaraJugador cg = GetComponentInChildren<CamaraJugador> ();
			cg.enabled = true;
			CameraInstance = cg;

			ccl.gamecam = cg;

			AudioListener AL = GetComponentInChildren<AudioListener> ();
			AL.enabled = true;




			//FPSCharacterCam.enabled = true;
			//audioListener.enabled = true;

			/*GameObject Pref = Instantiate (Prefab, transform.position - (transform.forward * 10f), transform.rotation) as GameObject;
		CamaraJugador cJ = Prefab.GetComponent<CamaraJugador> ();
		cJ.CameraTarget = myTrans;
		*/
			//GetComponent<CharacterControllerLogic> ().gamecam = cJ;


			/*Renderer[] rens = GetComponentsInChildren<Renderer>();
		foreach(Renderer ren in rens)
		{
			ren.enabled = false;
		}*/
		}
		GetComponent<NetworkAnimator> ().SetParameterAutoSend (0, true);
	}

	public override void PreStartClient ()
	{
		GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
	}

    public void onDieMessage()
    {
		if(isLocalPlayer)
       		 this.CameraInstance.enabled = false;
    }


}
