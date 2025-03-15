using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;
using Enemy;

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

        // Apply a knockback to enemies
        EnemyController enemyController = target.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            // Get the player position
            Vector3 playerPos = transform.parent.position;

            // Apply knockback directly to position (manual movement)
            Vector3 knockbackDirection = (target.transform.position - playerPos).normalized;
            knockbackDirection.y = 0; // Keep knockback horizontal (optional)

            // The knockback distance is between 1 to 5 depending on damage
            float knockbackDistance = Mathf.Clamp(damage * 0.1f, 1f, 5f);
            enemyController.ApplyKnockback(knockbackDirection, knockbackDistance);
        }
    }
}
