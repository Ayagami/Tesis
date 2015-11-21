using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Flag : NetworkBehaviour {
	[SyncVar] public int Team = -1;

 	public Transform Parent = null;

	private Transform _transform = null;

	public Flag_Base _base = null;

	[SyncVar] public string Flag_Base = "";

	/*
		Color and stuffs.
	 */

	private Color currentColor;
	private Renderer RendererReference = null;

	private Vector3 originalScale;
	private Vector3 AloneScale = new Vector3(5,5,5);

	// Use this for initialization
	void Start () {
		_transform = this.transform;
		RendererReference = this.gameObject.GetComponent<Renderer> ();
		currentColor = RendererReference.material.color;
		originalScale = _transform.localScale;
	}

	void OnDestroy(){
		if (GameManager_References.ImServer()) {
			DebugConsole.Log ("CALLING-DESTROY");
			CmdSendReSpawnWhenDisconnect(this.Team, this.Flag_Base);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(this.Team != -1){
			if(currentColor != ColorControl.colors[this.Team]){
				currentColor = ColorControl.colors[this.Team];
				currentColor.a = 0.5f;
				RendererReference.material.color = currentColor;
			}
		}

		if (Parent != null && Parent != _transform.parent) {
			_transform.parent = Parent;
			_transform.localPosition = Vector3.zero;
			_transform.localRotation = Quaternion.identity;
			//_transform.localScale = originalScale;
		}

		/*if (Parent == null && _transform.localScale != AloneScale)
			_transform.localScale = AloneScale;*/
	}

	void OnTriggerEnter(Collider obj){
		if (obj.tag == "Player") {
			if(Parent == null){
				PlayerAttributes PLA = obj.gameObject.GetComponent<PlayerAttributes>();

				if(PLA && !PLA.hasFlag){
					Player_NetworkSetup PA = obj.gameObject.GetComponent<Player_NetworkSetup>();
					Debug.Log(PLA.Team + " " + this.Team);
					if(PLA.Team != this.Team){
						Parent = PA.FlagPosition;
						PLA.hasFlag = true;

						Debug.Log("YUP");
					}
				}
			}
		}

		if (obj.tag == "Flag_Bases" && Parent!=null) {
			Flag_Base Base = obj.GetComponent<Flag_Base>();

			if(Base.name == this.Flag_Base)
				return;

			Parent.GetComponentInParent<PlayerAttributes> ().hasFlag = false;

			ReturnToBase();
			if(Base.Team != this.Team){
				if(isServer){
					CmdAddScoreToTeamFlag(this.Team);
				}
			}
		}
	}

	void ReturnToBase (){
		Parent = null;
		_transform.parent = Parent;
		
		if (_base == null) {
			GameObject g = GameObject.Find(Flag_Base);
			if(g)
				_base = g.GetComponent<Flag_Base>();
		}
		
		if (_base) {
			_transform.position = _base.transform.position;
		}
	}

	public void onDieMessage(){
		ReturnToBase ();
	}



	[Command]
	void CmdAddScoreToTeamFlag(int Theteam){
		GameManager_References.instance.AddScoreToTeamFlag (Theteam);
	}

	//[Command]
	void CmdSendReSpawnWhenDisconnect(int team, string parent){
		GameManager_References.instance.ReSpawnFlagWhenPlayersDisconnect (team, parent);
	}


}
