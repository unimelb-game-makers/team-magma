using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using UnityEngine.Diagnostics;
using Player;

public class PatrolState : IEnemyState
{
    private NavMeshAgent navMeshAgent;
    private PlayerController player;
    private float chaseRange;

    public void EnterState(EnemyController enemyController) {
        Debug.Log("Entering Patrol State");

        navMeshAgent = enemyController.GetNavMeshAgent();
        navMeshAgent.destination = enemyController.GetCurrentPatrolPoint().position;
        navMeshAgent.speed = enemyController.GetPatrolSpeed();

        player = enemyController.GetPlayer();

        chaseRange = enemyController.GetChaseRange();
    }

    public void UpdateState(EnemyController enemyController) {
        enemyController.Patrol();

        // y is set to 0 so that the patrol point can be at any height.
        var destinationPos = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        var currentPos = new Vector3(navMeshAgent.transform.position.x, 0, navMeshAgent.transform.position.z);

        // If the patrol point was reached, transition to idle state.
        if (Vector3.Distance(destinationPos, currentPos) <= navMeshAgent.stoppingDistance) {
            enemyController.TransitionToState(enemyController.GetIdleState());
            enemyController.NextPatrolPoint();
        }

        if (enemyController.GetPlayer() == null) return;
        // If the player is within chase distance, transition to chase state.
        float enemyPlayerDistance = Vector3.Distance(enemyController.transform.position, player.transform.position);
        if (enemyPlayerDistance <= chaseRange) {
            enemyController.TransitionToState(enemyController.GetChaseState());
        }
    }
}
