using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using UnityEngine.Diagnostics;
using Player;

public class PatrolState : BaseEnemyState
{
    public PatrolState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : 
                        base(enemyController, navMeshAgent, playerController) { }

    public override void EnterState() {
        Debug.Log("Entering Patrol State");
        enemyController.GetAnimator().SetBool(enemyController.GetPatrolAnimationBool(), true);
        navMeshAgent.destination = enemyController.GetCurrentPatrolPoint().position;
        navMeshAgent.speed = enemyController.GetPatrolSpeed();
    }

    public override void UpdateState() {
        enemyController.Patrol();

        // If the player is within aggro range, transition to chase state.
        if (playerController != null && enemyController.IsWithinAggroRange) {
            enemyController.TransitionToState(enemyController.GetChaseState());
        }
        // If the patrol point was reached, transition to idle state.
        else if (enemyController.HasReachedPatrolPoint) {
            enemyController.HasReachedPatrolPoint = false;
            enemyController.NextPatrolPoint();
            enemyController.TransitionToState(enemyController.GetIdleState());
        }
    }

    public override void ExitState() {
        enemyController.GetAnimator().SetBool(enemyController.GetPatrolAnimationBool(), false);
    }
}
