using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SprayBehaviour : ShieldBehaviour {
    private List<PlayerAttributes> objects = new List<PlayerAttributes>();

    private float auxTime = 0;
    public float applyDamageWhen = 0.1f;

	// Use this for initialization
	void Start () {
	}

    void Update() {
        base.Update();

        auxTime += Time.deltaTime;
        if (auxTime >= applyDamageWhen) {
            for (int i = 0; i < objects.Count; i++) {
                objects[i].TakeDamage(bulletDamage);
            }
            auxTime = 0;
        }
    }



    void OnTriggerEnter(Collider o) {
        PlayerAttributes PA = o.GetComponent<PlayerAttributes>();
        if (PA != null)    // Es player
        {
            objects.Add(PA);
        }
    }

    void OnTriggerExit(Collider o) {
        PlayerAttributes PA = o.GetComponent<PlayerAttributes>();
        if (PA == null)
            return;

        if (objects.Contains(PA)) {
            objects.Remove(PA);
        }
    }
}
