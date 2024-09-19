using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class MeleeEnemyController : EnemyController
{
    public override void Attack() {
        RotateTowardsPlayer();

        // Calculate the current cooldown time. If cooldown is over, attack.
        SetCurrentAttackCooldown(GetCurrentAttackCooldown() - Time.deltaTime); 
        if (GetCurrentAttackCooldown() <= 0) {
            SetCurrentAttackCooldown(GetAttackCooldown());

            Debug.Log("Melee Attack!");
            GetAnimator().SetTrigger(GetAttackAnimationTrigger());

            // Temporary until actual logic is implemented.
            // Eg: Could wait until the animation has finished playing.
            SetIsAttacking(true);

            // TO IMPLEMENT:
            // The damageable component will take care of dealing the damage.
            // Note: Will have to create a new damageable or damageComponent script,
            //       since the one right now destroys the enemy when touching the player.
            //       The player is also destroyed when touching the enemy.
            // This is assuming that both the player and enemy deal damage on collision.

            // Also have to remove renderer component from damageable script.

            SetIsAttacking(false);
        }
    }
}
