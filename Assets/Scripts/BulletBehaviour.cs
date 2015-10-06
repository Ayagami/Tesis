using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletBehaviour : NetworkBehaviour {

	public int bulletDamage = 10;
	public float timeThatCanBeAlive = 4f;
	private float timeAlive = 0f;
	// Use this for initialization
	void Start () {
		Rigidbody rb = this.GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * 20f;
	}
	
	// Update is called once per frame
	void Update () {
		timeAlive += Time.deltaTime;
		if (timeAlive >= timeThatCanBeAlive)
			Destroy (gameObject);
	}

	void OnTriggerEnter(Collider obj){
		if (obj.tag == "Player") {
			string uIdentity = obj.transform.name;
			CmdTellServerWhoWasShot(uIdentity, bulletDamage);
		}
		if (obj.tag == "Destruible") {
			obj.SendMessage("Explode");
			Destroy(obj.gameObject);
		}
		Destroy(gameObject);
	}

	[Command]
	void CmdTellServerWhoWasShot (string uniqueID, int dmg) {
		GameObject go = GameObject.Find(uniqueID);
		go.GetComponent<PlayerAttributes>().TakeDamage(dmg);
	}
}