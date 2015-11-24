using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SyncScaleForLevel : NetworkBehaviour {

	[SyncVar]
	public Vector3 desiredScale = Vector3.zero;

	[SyncVar]
	public Vector3 desiredRot = Vector3.zero;

	[SyncVar]
	public Vector3 desiredPos = Vector3.zero;

	// Update is called once per frame
	void FixedUpdate () {
		if (desiredScale != transform.localScale)
			transform.localScale = desiredScale;

		if (desiredRot != transform.rotation.eulerAngles)
			transform.rotation = Quaternion.Euler (desiredRot);

		if (desiredPos != transform.position)
			transform.position = desiredPos;
	}
}
