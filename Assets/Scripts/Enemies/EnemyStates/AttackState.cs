using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using Player;

public class AttackState : BaseEnemyState
{
    private float outsideAttackRangeTime;
    private float outsideAttackRangeDuration = 0.5f;
    
    public AttackState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : 
                        base(enemyController, navMeshAgent, playerController) { }

    public override void EnterState() {
        Debug.Log("Entering Attack State");
        enemyController.GetAnimator().SetBool(enemyController.GetIdleAttackAnimationBool(), true);
        navMeshAgent.destination = playerController.transform.position;
        navMeshAgent.speed = 0;
        enemyController.SetCurrentAttackCooldown(enemyController.GetAttackCooldown());
        outsideAttackRangeDuration = enemyController.GetOutsideAttackRangeDuration();
        outsideAttackRangeTime = outsideAttackRangeDuration;
    }

    public override void UpdateState() {
        enemyController.Attack();

        // If the enemy is currently attacking/knockbacked, wait for the attack/knockback to finish first before checking anything.
        if (enemyController.IsAttacking()) return;
        // Temporary code until knockback is included in base class.
        else if (enemyController is AssassinEnemyController && ((AssassinEnemyController) enemyController).isKnockback) {
            return;
        }
        // If the player was killed, transition to patrol state.
        else if (playerController == null) {
            enemyController.TransitionToState(enemyController.GetPatrolState());
        }
        // If the player has exited attack range, transition to chase state.
        else if (!enemyController.IsWithinAttackRange) {
            outsideAttackRangeTime -= Time.deltaTime;
            if (outsideAttackRangeTime <= 0) {
                // If I disable the nav mesh agent for ranged enemy, this comes here? Why? Do the colliders disappear or something???
                Debug.Log("HUHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH???????????");
                enemyController.TransitionToState(enemyController.GetChaseState());
            }
        }
    }

    public override void ExitState() {
        enemyController.GetAnimator().SetBool(enemyController.GetIdleAttackAnimationBool(), false);
    }
}
