using System.Collections;
using System.Collections.Generic;
using Damage;
using UnityEngine;

public class BulletDamager : MonoBehaviour, IDamageManager
{
    private GameObject parent;
    [SerializeField] private float damage = 10;

    public void Initialize(GameObject parent)
    {
        this.parent = parent;
    }

    /**
     * Deal damage to the object.
     */
    public void DealDamage(Damageable target)
    {
        // Do not damage the object that spawned it.
        if (target.gameObject == parent) return;

        target.TakeDamage(damage);
        Destroy(gameObject);
    }
}
