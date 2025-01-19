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
            enemyController.GetAnimator().SetBool(enemyController.GetChaseAnimationBool(), true);
            navMeshAgent.destination = enemyController.GetCurrentPatrolPoint().position;
            navMeshAgent.speed = enemyController.GetPatrolSpeed();
        }

        public override void UpdateState() {

            //if the enemy is RangedEnemyController 
            if (enemyController is RangedEnemyController rangedEnemyController) 
            {
                //if the enemy is fleeing, return
                if(rangedEnemyController.IsFleeing) return;
                
                // If the player is within aggro range, transition to chase state.
                if (playerController != null && enemyController.IsWithinAggroRange) {
                    enemyController.TransitionToState(EnemyState.Chase);
                }
                // If the patrol point was reached, transition to idle state.
                else if (enemyController.HasReachedPatrolPoint) {
                    enemyController.HasReachedPatrolPoint = false;
                    enemyController.NextPatrolPoint();
                    enemyController.TransitionToState(EnemyState.Idle);
                }
                rangedEnemyController.Flee();
            }
        }

        public override void ExitState() {
            enemyController.GetAnimator().SetBool(enemyController.GetPatrolAnimationBool(), false);
        }
    }
}