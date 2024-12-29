using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

public class MeleeDamager : MonoBehaviour
{
    [SerializeField] private float damage = 10;

    /**
     * Deal damage to the object and then destroy itself.
     */
    public void DealDamage(Damageable target)
    {
        target.TakeDamage(damage);
    }
}
