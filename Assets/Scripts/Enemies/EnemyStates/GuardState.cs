// Author : Peiyu Wang @ Daphatus
// 19 01 2025 01 26

using Enemies.EnemyTypes;
using Enemy;
using Player;
using UnityEngine.AI;

namespace Enemies.EnemyStates
{
    public class GuardState : BaseEnemyState
    {
        public GuardState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : base(enemyController, navMeshAgent, playerController)
        {
        }

        public override void EnterState()
        {
            if(enemyController is EliteEnemyController eliteEnemyController)
            {
                eliteEnemyController.Guard();
            }
        }

        public override void UpdateState()
        {
            //check if ready to leave guard state
            if (enemyController is not EliteEnemyController eliteEnemyController) return;
            
            if (!eliteEnemyController.IsAttackInCooldown)
            {
                eliteEnemyController.TransitionToState(EnemyState.Attack);
            }
        }

        public override void ExitState()
        {
            if(enemyController is EliteEnemyController eliteEnemyController)
            {
                eliteEnemyController.StopGuard();
            }
        }
    }
}