using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using System.Collections.Generic;

public class Player_Shoot : NetworkBehaviour {

	public GameObject bulletPrefab;
	public Transform shootTransform;
	[SyncVar] private int bulletID = 1;
	public float cooldownToShoot = 1f;

	private float currentCooldown = 0f;

    //private List<>
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckIfShooting();
	}

	void CheckIfShooting()
	{
		if(!isLocalPlayer)
		{
			return;
		}

		currentCooldown += Time.deltaTime;

		if(currentCooldown>=cooldownToShoot && Input.GetKeyDown(KeyCode.Mouse0))
		{
			Shoot();
			currentCooldown = 0f;
		}
	}

	void Shoot()
	{
		string id = "Bullet from "+ transform.name + bulletID;
		bulletID++;
		CmdTellToServerWhereIShoot (id, shootTransform.position, shootTransform.rotation.eulerAngles);
	}

	[Command]
	void CmdTellToServerWhereIShoot (string ID, Vector3 tPos, Vector3 tRot) {
		GameObject go = Instantiate (bulletPrefab, tPos, Quaternion.Euler (tRot) ) as GameObject;
		go.GetComponent<Zombie_ID> ().zombieID = ID;
		NetworkServer.Spawn (go);
	}

    public void onDieMessage()
    {
        this.enabled = false;
    }

}
