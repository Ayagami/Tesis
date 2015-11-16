using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Flag_Base : NetworkBehaviour {

	[SyncVar]
	public int Team = -1;

	[SyncVar]
	public int Score = 0;
}
