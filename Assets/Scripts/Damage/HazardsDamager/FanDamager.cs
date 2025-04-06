// Author : William Alexander Tang Wai @ Jalapeno
// 12/01/2025 14:33

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.AI;
using System.Data;

public class FanDamager : MonoBehaviour, IDamageManager
{
    private Damager damager;

    private float damage;
    private float knockbackForce;
    private float enemyKnockbackForce;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetKnockbackForce(float knockbackForce, float enemyKnockbackForce)
    {
        this.knockbackForce = knockbackForce;
        this.enemyKnockbackForce = enemyKnockbackForce;
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
            Vector3 knockbackDirection = (target.position - transform.position).normalized;
            Vector3 currentknockbackDirection = new(knockbackDirection.x, 0, knockbackDirection.z);

            // Apply knockback using rigidbody
            if (target.CompareTag("Player"))
            {
                rb.velocity = Vector3.zero; // Reset velocity for consistency
                rb.AddForce(currentknockbackDirection * knockbackForce, ForceMode.Impulse);
            }
            // Apply knockback using NavMeshAgent Warp
            else if (target.CompareTag("Enemy"))
            {
                var agent = target.GetComponent<NavMeshAgent>();
                var originalDestination = agent.destination;

                // Calculate new position after knockback
                Vector3 newPos = agent.transform.position + currentknockbackDirection * enemyKnockbackForce;

                // If the enemy can land on the navmesh at its new position, then move to its new position
                if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position); // Teleport the agent safely to the new position
                    agent.SetDestination(originalDestination);  // Restore pathfinding
                }
            }
        }
    }
}
