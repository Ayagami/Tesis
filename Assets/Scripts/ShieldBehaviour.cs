using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShieldBehaviour : BulletBehaviour {

    private float currentShieldHealth;

    [SyncVar] public Transform target = null;
	// Use this for initialization
	void Start () {
        currentShieldHealth = bulletDamage;
	}

    void Update()
    {
        base.Update();
        if (target == null)
            return;
        if (transform.parent != target)
        {
            transform.parent = target;
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Bullet")
        {
           float projDamage =  obj.GetComponent<BulletBehaviour>().bulletDamage;
           DoDamageToShield(projDamage);
        }
    }

    void DoDamageToShield(float damage)
    {
        currentShieldHealth -= damage;
        if (currentShieldHealth <= 0)
            Destroy(gameObject);
    }
}
