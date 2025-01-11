using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

public class FanDamager : MonoBehaviour, IDamageManager
{
    [SerializeField] private float damage = 10;
    [SerializeField] private float knockbackForce = 100;
    [SerializeField] private Vector3 knockbackDirection;

    /**
     * Deal damage to the object.
     */
    public void DealDamage(Damageable target)
    {
        target.TakeDamage(damage);
        Knockback(target.transform);
    }

    private void Knockback(Transform target)
    {
        // Apply a knockback
        Vector3 knockbackDirectionWithY = target.position - transform.position;
        Vector3 currentknockbackDirection = new Vector3(knockbackDirectionWithY.x * knockbackDirection.x,
                                                0,
                                                knockbackDirectionWithY.z * knockbackDirection.z)
                                                .normalized;
        target.GetComponent<Rigidbody>().AddForce(currentknockbackDirection * knockbackForce, ForceMode.Impulse);
    }
}
