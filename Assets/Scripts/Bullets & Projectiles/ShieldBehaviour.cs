using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShieldBehaviour : BulletBehaviour {

    private float currentShieldHealth;

    [SyncVar] public Transform target = null;

	public SpellTypes WeakAgainst = SpellTypes.NULL;
	public SpellTypes strongAgainst = SpellTypes.NULL;


	// Use this for initialization
	void Start () {
        currentShieldHealth = bulletDamage;
	}

    virtual protected void Update()
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
           //float projDamage =  obj.GetComponent<BulletBehaviour>().bulletDamage;
			BulletBehaviour bB = obj.GetComponent<BulletBehaviour>();
           DoDamageToShield(CalculateDamage(bB));
        }
    }

	float CalculateDamage(BulletBehaviour bullet){
		if (bullet.projectileType == WeakAgainst)
			return bullet.bulletDamage * 2;
		if (bullet.projectileType == strongAgainst)
			return bullet.bulletDamage * 0.5f;
		if (bullet.projectileType == this.projectileType)	// No te hago daño si me tiras mismo elemento... obviamente.
			return 0;

		return bullet.bulletDamage;
	}

    void DoDamageToShield(float damage)
    {
        currentShieldHealth -= damage;
        if (currentShieldHealth <= 0)
            Destroy(gameObject);
    }
}
