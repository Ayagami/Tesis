using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletBehaviour : NetworkBehaviour {

	public int bulletDamage = 10;
	public float timeThatCanBeAlive = 4f;
	private float timeAlive = 0f;

	public SpellTypes projectileType = SpellTypes.NULL;

	// Use this for initialization
	void Start () {
		Rigidbody rb = this.GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * 20f;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		timeAlive += Time.deltaTime;
		if (timeAlive >= timeThatCanBeAlive)
			Destroy (gameObject);
	}

	void OnTriggerEnter(Collider obj){
		if (obj.tag == "Player") {
			string uIdentity = obj.transform.name;
			CmdTellServerWhoWasShot (uIdentity, bulletDamage);
		} else if (obj.tag == "Destruible") {
			obj.SendMessage ("Explode");
			Destroy (obj.gameObject);
		} else {
			Debug.Log("CHECKING THIS");
			RaycastHit hitInfo;
			FracturedChunk chunkRaycast = FracturedChunk.ChunkRaycast(transform.position, transform.forward, out hitInfo);
			
			if(chunkRaycast) {
				chunkRaycast.Impact(hitInfo.point, 5, 1, true);
				Debug.Log("IMPACT");
			}
		}
		Destroy(gameObject);
	}

	[Command]
    void CmdTellServerWhoWasShot (string uniqueID, int dmg) {
		GameObject go = GameObject.Find(uniqueID);
		go.GetComponent<PlayerAttributes>().TakeDamage(dmg);
	}
}