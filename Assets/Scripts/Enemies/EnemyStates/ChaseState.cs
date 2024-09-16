using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using Player;

public class ChaseState : IEnemyState
{
    private NavMeshAgent navMeshAgent;
    private PlayerController player;
    private float chaseTime;
    private float chaseDuration;
    private float chaseRange;

    public void EnterState(EnemyController enemyController) {
        Debug.Log("Entering Chase State");
        
        navMeshAgent = enemyController.GetNavMeshAgent();
        navMeshAgent.speed = enemyController.GetChaseSpeed();

        player = enemyController.GetPlayer();

        chaseDuration = enemyController.GetChaseDuration();
        chaseTime = chaseDuration;
        chaseRange = enemyController.GetChaseRange();
    }

    public void UpdateState(EnemyController enemyController) {
        if (enemyController.GetPlayer() == null) return;
        
        enemyController.Chase();

        float enemyPlayerDistance = Vector3.Distance(enemyController.transform.position, player.transform.position);

        // If the player has exited the enemy's chase distance for chaseTime, transition to patrol state.
        chaseTime -= Time.deltaTime;
        if (enemyPlayerDistance > chaseRange && chaseTime <= 0) {
            enemyController.TransitionToState(enemyController.GetPatrolState());
        }
        // else if the player is within chase distance, reset the chaseTime.
        else if (enemyPlayerDistance <= chaseRange) {
            chaseTime = chaseDuration;
        }

        // If the player is within attack range, transition to attack state.
        if (enemyPlayerDistance < enemyController.GetAttackRange()) {
            enemyController.TransitionToState(enemyController.GetAttackState());
        }
    }
}
