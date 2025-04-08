using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using Player;

public class IdleState : BaseEnemyState
{
    private float idleTime;
    private float idleDuration;

    public IdleState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : 
                    base(enemyController, navMeshAgent, playerController) { }

    public override void EnterState() {
        // Debug.Log("Entering Idle State");
        // enemyController.GetAnimator().SetBool(enemyController.GetIdleAnimationBool(), true);
        idleDuration = enemyController.GetIdleDuration();
        idleTime = idleDuration;

        // enemyController.GetIdleSound().start();
    }

    public override void UpdateState() {
        enemyController.Idle();

        // If enough time has passed, transition to patrol state.
        idleTime -= Time.deltaTime;
        if (idleTime <= 0) {
            enemyController.TransitionToState(EnemyState.Patrol);
        }
        // If the player is within sight range, transition to chase state.
        else if (playerController != null && enemyController.PlayerIsInSightRange()) {
            enemyController.TransitionToState(EnemyState.Chase);
        }
    }

    public override void ExitState() {
        // enemyController.GetIdleSound().stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // enemyController.GetAnimator().SetBool(enemyController.GetIdleAnimationBool(), false);
    }
}
