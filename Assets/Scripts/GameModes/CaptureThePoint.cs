using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;

public class CaptureThePoint : NetworkBehaviour {

	public float Score = 50f;

	private List<PlayerAttributes> playersInside;

	[SyncVar(hook="OnColorChange")]
	private Color CurrentColor = Color.white;

	private Material matReference = null;

	private float initial;


	// Use this for initialization
	void Start () {
		if (!GameManager_References.ImServer () && !isServer) {
			return;
		}

		playersInside = new List<PlayerAttributes> ();
		matReference = GetComponent<Renderer> ().sharedMaterial;

		initial = Score;

		CurrentColor.a = 0.1f;
		matReference.color = CurrentColor;
	}

	void FixedUpdate(){
		if (!GameManager_References.ImServer () && !isServer)
			return;

		updateScore (Time.deltaTime);
		LerpColors (ColorControl.colors[0], ColorControl.colors[1]);
		CheckWinCondition ();
	}

	void updateScore(float delta){
		if (playersInside.Count <= 0) {
			if(Score > 49.5f && Score < 50.5f){
				Score = 50f;
				return;
			}
			
			float desiredDelta = Score > 50 ?  -delta : delta;
			Score += desiredDelta * 2f;
		
			return;
		}

		int[] teamsInside = new int[2];

		for (int i=0; i < playersInside.Count; i++) {
			teamsInside[playersInside[i].Team] += 1;
		}

		if (teamsInside [0] == teamsInside [1] && teamsInside [0] == 0) {
			return;
		}

		if (teamsInside [0] != 0 || teamsInside [1] != 0) {
			if(teamsInside[0] != 0 && teamsInside[1] != 0)
				return;

			if(teamsInside[0] != 0){
				Score -= delta * teamsInside[0];
			}else{
				Score += delta * teamsInside[1];
			}
		}
	}

	void CheckWinCondition(){
		if (!GameManager_References.ImServer () && !isServer) {
			return;
		}

		if (Score <= 0)
			GameManager_References.instance.PointModeWinner (0);
		if (Score >= 100)
			GameManager_References.instance.PointModeWinner (1);


	}

	void OnColorChange(Color newColor){
		if (matReference == null)
			matReference = GetComponent<Renderer> ().sharedMaterial;


		CurrentColor = newColor;
		matReference.color = CurrentColor;
	}

	void OnTriggerEnter(Collider col){
		if (!GameManager_References.ImServer () && !isServer)
			return;

		if (col.tag == "Player") {
			PlayerAttributes PA = col.GetComponent<PlayerAttributes>();
			if(playersInside.IndexOf(PA) == -1){
				playersInside.Add(PA);
			}
		}
	}
	
	void OnTriggerExit(Collider col){
		if (!GameManager_References.ImServer () && !isServer)
			return;

		if (col.tag == "Player") {
			PlayerAttributes PA = col.GetComponent<PlayerAttributes>();
			if(playersInside.IndexOf(PA) != -1){
				playersInside.Remove(PA);
			}
		}
	}

	void LerpColors(Color from, Color to){
		Color aux = from;

		aux.r = Mathf.Lerp (from.r, to.r, Score / 100);
		aux.g = Mathf.Lerp (from.g, to.g, Score / 100);
		aux.b = Mathf.Lerp (from.b, to.b, Score / 100);
		aux.a = 0.5f;

		CurrentColor = aux;
	}
}
