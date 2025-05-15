using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using UnityEngine.Diagnostics;
using Player;

public class PatrolState : BaseEnemyState
{
    private float stepInterval = 1f;
    private readonly float lowStepInterval = 1.2f;
    private readonly float highStepInterval = 0.8f;

    private float stepElapsedTime = 0;

    public PatrolState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : 
                        base(enemyController, navMeshAgent, playerController) { }

    public override void EnterState() {
        // Debug.Log("Entering Patrol State");
        if(enemyController.GetAnimator()){
            enemyController.GetAnimator().SetBool("Patrol", true);
        }
        navMeshAgent.destination = enemyController.GetCurrentPatrolPoint();
        navMeshAgent.speed = enemyController.GetPatrolSpeed();
    }

    public override void UpdateState() {
        enemyController.Patrol();

        // If the player is within sight range, transition to chase state.
        if (playerController != null && enemyController.PlayerIsInSightRange()) {
            enemyController.TransitionToState(EnemyState.Chase);
        }
        // If the patrol point was reached, transition to idle state.
        else if (enemyController.EnemyIsInDestinationRange()) {
            enemyController.NextPatrolPoint();
            enemyController.TransitionToState(EnemyState.Idle);
        }

        // Footsteps sound.
        // If the enemy has no patrol points, it does not move and must not make sound.
        if (enemyController.GetPresetPatrolPoints() == true) {
            stepElapsedTime += Time.deltaTime;
            if (stepElapsedTime > stepInterval) {
                stepElapsedTime = 0;
                enemyController.GetPatrolSound().start();
                stepInterval = Random.Range(lowStepInterval, highStepInterval);
            }
        }
    }

    public override void ExitState() {
        enemyController.GetPatrolSound().stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if(enemyController.GetAnimator()){
            enemyController.GetAnimator().SetBool("Patrol", false);
        }

    }
}
