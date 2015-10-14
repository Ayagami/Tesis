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

	private List<SpellTypes> currentSpells;

	private SpellTypes CurrentTypeSpell = SpellTypes.THROW;

	private int maxSpells = 4;


	// Use this for initialization
	void Start () {
		currentSpells = new List<SpellTypes>();
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

		if (Input.GetKeyDown (KeyCode.Escape)) {
			cleanSpells();
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			setTypeSpell(SpellTypes.SHIELD);
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			setTypeSpell(SpellTypes.THROW);
		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			addSpell(SpellTypes.FIRE);
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			addSpell(SpellTypes.WATER);
		}

		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			addSpell(SpellTypes.ROCK);
		}

		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			addSpell(SpellTypes.AIR);
		}

		if(currentCooldown>=cooldownToShoot && Input.GetKeyDown(KeyCode.Mouse0)) {
			StartCoroutine(Shoot());
			currentCooldown = 0f;
		}
	}

	private void addSpell(SpellTypes spell){
		if (currentSpells.Count >= maxSpells) {
			return;
		}

		currentSpells.Add (spell);

		if (currentSpells.Count == 1) {
			return;
		}

		int index = 0;
		while (index < currentSpells.Count - 1) {
			SpellTypes result = SpellManager.instance.Combination (currentSpells[index], currentSpells[index+1] );
			if( result != SpellTypes.NULL){
				currentSpells.RemoveAt(index);
				currentSpells.RemoveAt(index);
				currentSpells.Insert(index, result);
			} else {
				index++;
			}
		}
	}

	void cleanSpells(){
		currentSpells.Clear ();
	}

	void setTypeSpell(SpellTypes type){
		CurrentTypeSpell = type;
	}

	IEnumerator Shoot() {

        if (currentSpells.Count <= 0)
            yield return false;

        if (CurrentTypeSpell == SpellTypes.SHIELD)
        {
            string id = "Bullet from " + transform.name + bulletID;
            bulletID++;
            CmdTellToServerWhereIShoot(id, transform.position, transform.rotation.eulerAngles, (int)currentSpells[0], (int)CurrentTypeSpell, transform.name);
        }
        else
        {
            for (int i = 0; i < currentSpells.Count; i++)
            {
                string id = "Bullet from " + transform.name + bulletID;
                bulletID++;
                CmdTellToServerWhereIShoot(id, shootTransform.position, shootTransform.rotation.eulerAngles, (int)currentSpells[i], (int)CurrentTypeSpell, transform.name);

                yield return new WaitForSeconds(0.3f);
            }
        }
	}

	[Command]
	void CmdTellToServerWhereIShoot (string ID, Vector3 tPos, Vector3 tRot, int spell, int type, string who) {

		SpellTypes parsedSpell = (SpellTypes) spell;
		SpellTypes parsedType =  (SpellTypes)type;

		GameObject prefab = SpellManager.instance.getGraphic (parsedSpell, parsedType);

		GameObject go = Instantiate (prefab, tPos, Quaternion.Euler (tRot) ) as GameObject;
		go.GetComponent<Zombie_ID> ().zombieID = ID;
		NetworkServer.Spawn (go);

        if ((SpellTypes)type == SpellTypes.SHIELD)
        {
            GameObject parent = GameObject.Find(who) as GameObject;
            go.GetComponent<ShieldBehaviour>().target = parent.transform;
        }
	}

    public void onDieMessage()
    {
        this.enabled = false;
    }

}
