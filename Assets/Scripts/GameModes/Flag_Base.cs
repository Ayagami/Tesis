using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Flag_Base : NetworkBehaviour {

	[SyncVar]
	public int Team = -1;

	[SyncVar]
	public int Score = 0;

	[SyncVar]
	public string desiredName = "";

	private Transform _transform;

	void Start(){
		_transform = this.transform;
	}

	void Update(){
		if (_transform.name == "" || _transform.name != desiredName) {
			_transform.name = desiredName;
		}
	}
}
