using Enemies.EnemyTypes;
using Enemy;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.EnemyStates
{
    public class FleeState : BaseEnemyState
    {
        private float stepInterval = 0.5f;
        private readonly float lowStepInterval = 0.7f;
        private readonly float highStepInterval = 0.3f;
        private float stepElapsedTime = 0;

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
            // Footsteps sound.
            stepElapsedTime += Time.deltaTime;
            if (stepElapsedTime > stepInterval) {
                stepElapsedTime = 0;
                stepInterval = Random.Range(lowStepInterval, highStepInterval);
                if (enemyController is RangedEnemyController rController)
                {
                    rController.GetFleeSound().start();
                }
            }

            // Transition to attack state after fleeing.
            if (enemyController is RangedEnemyController rEnemyController
                && rEnemyController.EnemyHasMovedToFleeLocation()) 
                enemyController.TransitionToState(EnemyState.Attack);
        }

        public override void ExitState() {
            if (enemyController is RangedEnemyController rController)
            {
                rController.GetFleeSound().stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                // enemyController.GetAnimator().SetBool(enemyController.GetPatrolAnimationBool(), false);
            }
        }
    }
}