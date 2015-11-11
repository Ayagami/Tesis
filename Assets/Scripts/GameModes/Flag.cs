﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Flag : NetworkBehaviour {

	public int Team = -1;

	//[SyncVar]
	public Transform Parent = null;

	private Transform _transform = null;

	public Flag_Base _base = null;


	// Use this for initialization
	void Start () {
		_transform = this.transform;
		//Parent = null;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Parent != null && Parent != _transform.parent) {
			_transform.parent = Parent;
			_transform.localPosition = Vector3.zero;
			_transform.localRotation = Quaternion.identity;
		}
	}

	void OnTriggerEnter(Collider obj){
		if (obj.tag == "Player") {
			if(Parent == null){
				Player_NetworkSetup PA = obj.gameObject.GetComponent<Player_NetworkSetup>();
				Parent = PA.FlagPosition;
			}
		}
	}
}
