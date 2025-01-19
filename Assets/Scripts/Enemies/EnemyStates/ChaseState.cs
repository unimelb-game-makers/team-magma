using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using Player;

public class ChaseState : BaseEnemyState
{
    private float chaseTime;
    private float chaseDuration;

    public ChaseState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : 
                    base(enemyController, navMeshAgent, playerController) { }

    public override void EnterState() {
        Debug.Log("Entering Chase State");
        enemyController.GetAnimator().SetBool(enemyController.GetChaseAnimationBool(), true);
        navMeshAgent.speed = enemyController.GetChaseSpeed();
        chaseDuration = enemyController.GetChaseDuration();
        chaseTime = chaseDuration;
    }

    public override void UpdateState() {
        enemyController.Chase();

        // If the player was killed, transition to patrol state.
        if (!playerController) {
            enemyController.TransitionToState(EnemyState.Patrol);
        }
        // If the player is within attack range, transition to attack state.
        else if (playerController && enemyController.IsWithinAttackRange) {
            enemyController.TransitionToState(EnemyState.Attack);
        }
        // If the player has exited the enemy's aggro range for chaseTime, transition to patrol state.
        else if (!enemyController.IsWithinAggroRange) {
            chaseTime -= Time.deltaTime;
            if (chaseTime <= 0) {
                enemyController.TransitionToState(EnemyState.Patrol);
            }
        }
        // else if the player is in the enemy's aggro range, reset chaseTime.
        else {
            chaseTime = chaseDuration;
        }
    }

    public override void ExitState() {
        enemyController.GetAnimator().SetBool(enemyController.GetChaseAnimationBool(), false);
    }
}
