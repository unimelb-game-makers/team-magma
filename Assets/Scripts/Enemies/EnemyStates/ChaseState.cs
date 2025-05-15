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

    private float stepInterval = 0.5f;
    private readonly float lowStepInterval = 0.7f;
    private readonly float highStepInterval = 0.3f;
    private float stepElapsedTime = 0;

    public ChaseState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : 
                    base(enemyController, navMeshAgent, playerController) { }

    public override void EnterState() {
        // Debug.Log("Entering Chase State");
        if(enemyController.GetAnimator()){
            enemyController.GetAnimator().SetBool("Chase", true);
        }

        navMeshAgent.speed = enemyController.GetChaseSpeed();
        chaseDuration = enemyController.GetChaseDuration();
        chaseTime = chaseDuration;
    }

    public override void UpdateState() {
        enemyController.Chase();

        // Footsteps sound.
        stepElapsedTime += Time.deltaTime;
        if (stepElapsedTime > stepInterval) {
            stepElapsedTime = 0;
            enemyController.GetChaseSound().start();
            stepInterval = Random.Range(lowStepInterval, highStepInterval);
        }

        // If the player was killed, transition to patrol state.
        if (playerController == null) {
            enemyController.TransitionToState(EnemyState.Patrol);
        }
        // If the player is within attack range, transition to attack state.
        else if (playerController && enemyController.PlayerIsInAttackRange()) {
            enemyController.TransitionToState(EnemyState.Attack);
        }
        // If the player has exited the enemy's sight range for chaseTime, transition to patrol state.
        else if (!enemyController.PlayerIsInSightRange()) {
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
        enemyController.GetChaseSound().stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if(enemyController.GetAnimator()){
            enemyController.GetAnimator().SetBool("Chase", false);
        }
    }
}
