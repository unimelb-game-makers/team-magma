using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class MeleeEnemyController : EnemyController
{
    private float currentAttackCooldown = 0;

    public override void Attack() {
        RotateTowardsPlayer();
        animator.Play(idleAttackAnimation.name);

        currentAttackCooldown -= Time.deltaTime;
        if (currentAttackCooldown <= 0) {
            currentAttackCooldown = GetAttackCooldown();

            Debug.Log("Melee Attack!");
            animator.Play(attackAnimation.name);

            // TO IMPLEMENT:
            // The damageable component will take care of dealing the damage.
            // Note: Will have to create a new damageable or damageComponent script,
            //       since the one right now destroys the enemy when touching the player.
            //       The player is also destroyed when touching the enemy.
            // This is assuming that both the player and enemy deal damage on collision.

            // Also have to remove renderer component from damageable script.
        }
    }
}
