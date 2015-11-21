using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncScaleForLevel : NetworkBehaviour {

	[SyncVar]
	public Vector3 desiredScale = Vector3.zero;

	// Update is called once per frame
	void FixedUpdate () {
		if (desiredScale != transform.localScale)
			transform.localScale = desiredScale;
	}
}
