using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class RangedEnemyController : EnemyController
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;

    public override void Attack() {
        RotateTowardsPlayer();

        // Calculate the current cooldown time. If cooldown is over, attack.
        SetCurrentAttackCooldown(GetCurrentAttackCooldown() - Time.deltaTime); 
        if (GetCurrentAttackCooldown() <= 0) {
            SetCurrentAttackCooldown(GetAttackCooldown());

            Debug.Log("Ranged Attack!");
            GetAnimator().SetTrigger(GetAttackAnimationTrigger());

            // Temporary until actual logic is implemented.
            // Eg: Could wait until the animation has finished playing.
            SetIsAttacking(true);

            // If the player is still alive.
            if (GetPlayerController()) SpawnProjectile();

            SetIsAttacking(false);
        }
    }

    private void SpawnProjectile() {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        // Get the Projectile component from the projectile object.
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        // Check if the Projectile component exists.
        if (projectileComponent != null) {
            // Set the initial direction of the projectile.
            Vector3 direction = (GetPlayerController().transform.position - transform.position).normalized;
            projectileComponent.SetInitialDirection(new Vector3(direction.x, 0f, direction.z));
        }
    }
}
