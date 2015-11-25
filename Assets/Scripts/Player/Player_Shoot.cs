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

	[SerializeField]
	private int maxSpells = 5;

	private bool isShooting = false;

	private PlayerAttributes playerAttr;

	// Use this for initialization
	void Start () {
		currentSpells = new List<SpellTypes> ();
		playerAttr = GetComponent<PlayerAttributes>();
		//Cursor.visible = false;
		//Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		CheckInputs ();
		CheckIfShooting();
	}

	void CheckIfShooting() {
		if(!isLocalPlayer) {
			return;
		}

		if (GameManager_References.instance.isGameEnded ())
			return;

		currentCooldown += Time.deltaTime;
		if (Input.GetKeyDown (KeyCode.F1)) {
			cleanSpells();
		}

		if(currentCooldown>=cooldownToShoot && Input.GetKeyDown(KeyCode.Mouse0) && !isShooting) {
			StartCoroutine(Shoot());
			currentCooldown = 0f;
		}
	}

	private void CheckInputs(){
		if (!isLocalPlayer)
			return;

		/*if (Input.GetKeyDown (KeyCode.Escape)) {
			Cursor.visible = !Cursor.visible;
			Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.Confined : CursorLockMode.Locked;
		}*/
		CheckSpellTypeChanges ();
		CheckAddSpell ();
	}

	private void CheckSpellTypeChanges(){
		if (Input.GetKeyDown (KeyCode.Q)) {
			setTypeSpell(SpellTypes.SHIELD);
		}
		
		if (Input.GetKeyDown (KeyCode.E)) {
			setTypeSpell(SpellTypes.THROW);
		}
		
		if (Input.GetKeyDown (KeyCode.Tab)) {
			setTypeSpell(SpellTypes.RAY);
		}
		
		if (Input.GetKeyDown (KeyCode.R)) {
			setTypeSpell(SpellTypes.DROP);
		}
	}

	private void CheckAddSpell(){
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

		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			addSpell(SpellTypes.LIGHT);
		}
	}

	private void addSpell(SpellTypes spell){
		if (currentSpells.Count >= maxSpells) {
			return;
		}

		currentSpells.Add (spell);

		if (currentSpells.Count == 1) {
			SkillGUIManager.Singleton.UpdateGraphics (currentSpells);
			return;
		}

		int index = 0;
		while (index < currentSpells.Count - 1) {
			SpellTypes result = SpellManager.instance.Combination (currentSpells[index], currentSpells[index+1] );
			if( result != SpellTypes.NULL){
				currentSpells.RemoveAt(index);
				currentSpells.RemoveAt(index);
				currentSpells.Insert(index, result);
                index = 0;
			} else {
				index++;
			}
		}

		SkillGUIManager.Singleton.UpdateGraphics (currentSpells);
	}

	void cleanSpells(){
		currentSpells.Clear ();
		SkillGUIManager.Singleton.UpdateGraphics (currentSpells);
	}

	void setTypeSpell(SpellTypes type){
		if (type == CurrentTypeSpell || isShooting)
			return;

		Debug.Log ("SWITCHING TO " + type);
		cleanSpells ();

		CurrentTypeSpell = type;
	}

	IEnumerator Shoot() {
		isShooting = true;
        if (currentSpells.Count <= 0)
            yield return false;

		switch (CurrentTypeSpell) {
			case SpellTypes.SHIELD:
				string id = "Bullet from " + transform.name + bulletID;
				bulletID++;
				if(currentSpells.Count > 0)
					CmdTellToServerWhereIShoot(id, transform.position, transform.rotation.eulerAngles, (int)currentSpells[0], (int)CurrentTypeSpell, transform.name);
				break;
			case SpellTypes.THROW:
				for (int i = 0; i < currentSpells.Count; i++) {
					string idT = "Bullet from " + transform.name + bulletID;
					bulletID++;
					CmdTellToServerWhereIShoot(idT, shootTransform.position, shootTransform.rotation.eulerAngles, (int)currentSpells[i], (int)CurrentTypeSpell, transform.name);
					yield return new WaitForSeconds(0.3f);
				}
				break;
            case SpellTypes.DROP:
                RaycastHit hit;
                if (Physics.Raycast(shootTransform.position, shootTransform.forward, out hit, 100f) && currentSpells.Count > 0) {
                    Vector3 spawn = hit.point;
                    spawn.y += 10f;

                    Vector3 Rotation = new Vector3(90, 0, 0);

                    for (int i = 0; i < currentSpells.Count; i++) {
                        Vector3 randomSpawnPos = spawn + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                        string idD = "Bullet from " + transform.name + bulletID;
                        bulletID++;
                        CmdTellToServerWhereIShoot(idD, randomSpawnPos, Rotation, (int)currentSpells[i], (int)CurrentTypeSpell, transform.name);
                        yield return new WaitForSeconds(0.3f);
                    }
                }
                break;
            case SpellTypes.RAY:
                if (currentSpells.Count > 0)
                {
                    string idR = "Bullet from " + transform.name + bulletID;
                    bulletID++;
                    CmdTellToServerWhereIShoot(idR, shootTransform.position, shootTransform.rotation.eulerAngles, (int)currentSpells[0], (int)CurrentTypeSpell, transform.name);
                }
                break;
			default:
				Debug.LogError("SPELL NOT IMPLEMENTED!");
				break;
		}
		isShooting = false;
	}

	[Command]
	void CmdTellToServerWhereIShoot (string ID, Vector3 tPos, Vector3 tRot, int spell, int type, string who) {

		SpellTypes parsedSpell = (SpellTypes) spell;
		SpellTypes parsedType =  (SpellTypes)type;

		GameObject prefab = SpellManager.instance.getGraphic (parsedSpell, parsedType);

		GameObject go = Instantiate (prefab, tPos, Quaternion.Euler (tRot) ) as GameObject;
		go.GetComponent<Zombie_ID> ().zombieID = ID;
		NetworkServer.Spawn (go);

		go.GetComponent<BulletBehaviour> ().Team = playerAttr.Team;

        if ((SpellTypes)type == SpellTypes.SHIELD || (SpellTypes)type == SpellTypes.RAY) {
            GameObject parent = GameObject.Find(who) as GameObject;
            go.GetComponent<ShieldBehaviour>().target = parent.transform;
        }
	}

    public void onDieMessage() {
        this.enabled = false;
    }

	public void onAliveMessage(){
		this.enabled = true;
	}
}
