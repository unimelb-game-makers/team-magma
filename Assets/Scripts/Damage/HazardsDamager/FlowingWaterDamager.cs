// Author : William Alexander Tang Wai @ Jalapeno
// 12/01/2025 18:19

using System.Collections;
using System.Collections.Generic;
using Damage;
using UnityEngine;

public class FlowingWaterDamager : MonoBehaviour, IDamageManager
{
    private Damager damager;

    private float damage;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void Awake()
    {
        damager = GetComponent<Damager>();
    }

    /**
     * Damage characters when they enter the area.
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
