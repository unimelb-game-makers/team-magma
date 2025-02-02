using Enemies.EnemyTypes;
using Enemy;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.EnemyStates
{
    public class FleeState : BaseEnemyState
    {
        public FleeState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : 
            base(enemyController, navMeshAgent, playerController) { }

        public override void EnterState() {
            Debug.Log("Entering Flee State");
            // enemyController.GetAnimator().SetBool(enemyController.GetChaseAnimationBool(), true);

            if (enemyController is RangedEnemyController rController)
            {
                navMeshAgent.SetDestination(rController.GetFleeLocation());
                navMeshAgent.speed = rController.GetFleeSpeed();
            }
        }

        public override void UpdateState() {
            // Transition to attack state after fleeing.
            if (enemyController is RangedEnemyController rEnemyController
                && rEnemyController.EnemyHasMovedToFleeLocation()) 
                enemyController.TransitionToState(EnemyState.Attack);
        }

        public override void ExitState() {
            // enemyController.GetAnimator().SetBool(enemyController.GetPatrolAnimationBool(), false);
        }
    }
}