using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class IdleState : IEnemyState
{
    private float idleTime;
    private float idleDuration;
    private float chaseRange;

    public void EnterState(EnemyController enemyController) {
        Debug.Log("Entering Idle State");
        idleDuration = enemyController.GetIdleDuration();
        idleTime = idleDuration;
        chaseRange = enemyController.GetChaseRange();
    }

    public void UpdateState(EnemyController enemyController) {
        enemyController.Idle();

        // If enough time has passed, transition to patrol state.
        idleTime -= Time.deltaTime;
        if (idleTime <= 0) {
            enemyController.TransitionToState(enemyController.GetPatrolState());
        }

        if (enemyController.GetPlayer() == null) return;
        // If the player is within chase distance, transition to chase state.
        float enemyPlayerDistance = Vector3.Distance(enemyController.transform.position, enemyController.GetPlayer().transform.position);
        if (enemyPlayerDistance <= chaseRange) {
            enemyController.TransitionToState(enemyController.GetChaseState());
        }
    }
}
