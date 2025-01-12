using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

public class MeleeDamager : MonoBehaviour, IDamageManager
{
    private Damager damager;
    [SerializeField] private float damage = 10;
    public float Damage { get => damage; set => damage = value; }

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
        target.TakeDamage(damage);
    }
}
