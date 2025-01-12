using System.Collections;
using System.Collections.Generic;
using Damage;
using UnityEngine;

public class BulletDamager : MonoBehaviour, IDamageManager
{
    private GameObject parent;
    [SerializeField] private float damage = 10;
    private Damager damager;

    public void Initialize(GameObject parent)
    {
        this.parent = parent;
    }

    public void Awake()
    {
        damager = GetComponent<Damager>();
    }

    /**
     * Damage characters when it collides.
     */
    private void OnTriggerEnter(Collider other)
    {
        damager.Damage(other);
    }

    /**
     * Deal damage to the object.
     */
    public void DealDamage(Damageable target)
    {
        // Do not damage the object that spawned it.
        if (target.gameObject == parent) return;

        target.TakeDamage(damage);

        // Try to put this in OnTriggerEnter.
        // Destroy itself unless it is colliding with the object which fired it.
        Destroy(gameObject);
    }
}
