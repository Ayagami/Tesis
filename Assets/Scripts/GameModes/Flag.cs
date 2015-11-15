using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Flag : NetworkBehaviour {
	[SyncVar]
	public int Team = -1;

	[SyncVar]
	public Transform Parent = null;

	private Transform _transform = null;

	public Flag_Base _base = null;

	/*
		Color and stuffs.
	 */

	private Color currentColor;
	private Renderer RendererReference = null;

	// Use this for initialization
	void Start () {
		_transform = this.transform;
		RendererReference = this.gameObject.GetComponent<Renderer> ();
		currentColor = RendererReference.material.color;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (Parent != null && Parent != _transform.parent) {
			_transform.parent = Parent;
			_transform.localPosition = Vector3.zero;
			_transform.localRotation = Quaternion.identity;
		}

		if(this.Team != -1){
			if(currentColor != ColorControl.colors[this.Team]){
				currentColor = ColorControl.colors[this.Team];
				currentColor.a = 0.5f;
				RendererReference.material.color = currentColor;
			}
		}
	}

	void OnTriggerEnter(Collider obj){
		if (obj.tag == "Player") {
			if(Parent == null){
				Player_NetworkSetup PA = obj.gameObject.GetComponent<Player_NetworkSetup>();
				if(isServer)
					Parent = PA.FlagPosition;
			}
		}
	}
}
