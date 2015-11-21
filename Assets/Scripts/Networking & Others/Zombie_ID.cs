using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Zombie_ID : NetworkBehaviour {

	[SyncVar] public string zombieID = "";
	private Transform myTransform;

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		if (zombieID != "") {
			if(myTransform.name == "" || myTransform.name == "Bullet(Clone)" || (myTransform.tag == "Flag_Bases" && myTransform.name != zombieID)) {
				myTransform.name = zombieID;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		SetIdentity();
	}

	void SetIdentity()
	{
		if(myTransform.name == "" || myTransform.name == "Bullet(Clone)" || (myTransform.tag == "Flag_Bases" && myTransform.name != zombieID))
		{
			myTransform.name = zombieID;
		}
	}
}
