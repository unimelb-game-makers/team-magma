using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class AssassinEnemyController : EnemyController
{
    private float currentAttackCooldown = 0;

    public override void Attack() {
        RotateTowardsPlayer();
        animator.Play(idleAttackAnimation.name);

        currentAttackCooldown -= Time.deltaTime;
        if (currentAttackCooldown <= 0) {
            currentAttackCooldown = GetAttackCooldown();

            Debug.Log("Assassin Attack!");
            animator.Play(attackAnimation.name);

            // TO IMPLEMENT:
            // Make the enemy dash into the player.

            // If hitting the player (i.e. by using ontriggerenter or distance check),
            // apply a knockback to the enemy (and player).

            // Similar to melee enemy, the damageable component will take care of dealing the damage.
            // Check MeleeEnemyController for more details.
        }
    }
}