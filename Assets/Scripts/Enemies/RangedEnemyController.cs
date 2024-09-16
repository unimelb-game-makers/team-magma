using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class RangedEnemyController : EnemyController
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;

    private float currentAttackCooldown = 0;

    public override void Attack() {
        RotateTowardsPlayer();
        animator.Play(idleAttackAnimation.name);

        currentAttackCooldown -= Time.deltaTime;
        if (currentAttackCooldown <= 0) {
            currentAttackCooldown = GetAttackCooldown();

            Debug.Log("Ranged Attack!");
            animator.Play(attackAnimation.name);

            // Spawn a projectile.
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            // Get the Projectile component from the projectile object.
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            // Check if the Projectile component exists.
            if (projectileComponent != null) {
                // Set the initial direction of the projectile.
                Vector3 direction = (player.transform.position - transform.position).normalized;
                projectileComponent.SetInitialDirection(new Vector3(direction.x, 0f, direction.z));
            }
        }
    }
}
