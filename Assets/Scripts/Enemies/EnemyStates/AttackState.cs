using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using Player;

public class AttackState : IEnemyState
{
    private NavMeshAgent navMeshAgent;
    private PlayerController player;
    private float attackRange;

    public void EnterState(EnemyController enemyController) {
        Debug.Log("Entering Attack State");

        navMeshAgent = enemyController.GetNavMeshAgent();
        navMeshAgent.destination = enemyController.GetPlayer().transform.position;
        navMeshAgent.speed = 0;

        player = enemyController.GetPlayer();

        attackRange = enemyController.GetAttackRange();
    }

    public void UpdateState(EnemyController enemyController) {
        if (enemyController.GetPlayer() == null) return;
        
        enemyController.Attack();

        // If the player has exited attack range, transition to chase state.
        float enemyPlayerDistance = Vector3.Distance(enemyController.transform.position, player.transform.position);
        if (enemyPlayerDistance > attackRange) {
            enemyController.TransitionToState(enemyController.GetChaseState());
        }
    }
}
