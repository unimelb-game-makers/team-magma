using System.Collections;
using System.Collections.Generic;
using Damage;
using UnityEngine;

public class BulletDamager : MonoBehaviour, IDamageManager
{
    [SerializeField] private float damage = 10;

    /**
     * Deal damage to the object.
     */
    public void DealDamage(Damageable target)
    {
        target.TakeDamage(damage);
        Destroy(gameObject);
    }
}
