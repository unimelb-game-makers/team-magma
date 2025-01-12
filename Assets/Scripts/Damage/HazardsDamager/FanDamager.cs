// Author : William Alexander Tang Wai @ Jalapeno
// 12/01/2025 14:33

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class FanDamager : MonoBehaviour, IDamageManager
{
    private Damager damager;

    private float damage;
    private float knockbackForce;
    private Vector3 knockbackDirection;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetKnockbackForce(float knockbackForce)
    {
        this.knockbackForce = knockbackForce;
    }

    public void SetKnockbackDirection(Vector3 knockbackDirection)
    {
        // Take the absolute value of each component of the Vector3.
        this.knockbackDirection = new Vector3(
            Mathf.Abs(knockbackDirection.x),
            Mathf.Abs(knockbackDirection.y),
            Mathf.Abs(knockbackDirection.z)
        );
    }

    public void Awake()
    {
        damager = GetComponent<Damager>();
    }

    /**
     * Damage characters when they collide with the blade and apply a knockback
     * irrespective of whether the objects can be damaged.
     */
    private void OnTriggerEnter(Collider other)
    {
        damager.Damage(other);
        Knockback(other.transform);
    }

    /**
     * Deal damage to the object.
     */
    public void DealDamage(Damageable target)
    {
        target.TakeDamage(damage);
    }

    private void Knockback(Transform target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Calculate knockback direction
            Vector3 knockbackDirectionWithY = target.position - transform.position;
            Vector3 currentknockbackDirection = new Vector3(knockbackDirectionWithY.x * knockbackDirection.x,
                                                    0,
                                                    knockbackDirectionWithY.z * knockbackDirection.z)
                                                    .normalized;
        
            // Apply knockback
            rb.velocity = Vector3.zero; // Reset velocity for consistency
            rb.AddForce(currentknockbackDirection * knockbackForce, ForceMode.VelocityChange);
        }
    }
}
