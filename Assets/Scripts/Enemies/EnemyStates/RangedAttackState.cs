// Author : Peiyu Wang @ Daphatus
// 19 01 2025 01 53

using Enemies.EnemyTypes;
using Enemy;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.EnemyStates
{
    public class RangedAttackState :AttackState
    {
        public RangedAttackState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : base(enemyController, navMeshAgent, playerController)
        {
        }
        
        public override void UpdateState() {
        
            // If the enemy is currently attacking/knockbacked, wait for the attack/knockback to finish first before checking anything.
            
            if (enemyController.IsAttacking()) return;
            // Temporary code until knockback is included in base class.
            else if (enemyController is AssassinEnemyController && ((AssassinEnemyController) enemyController).isKnockback) {
                return;
            }
            // If the player was killed, transition to patrol state.
            else if (!playerController) {
                enemyController.TransitionToState(EnemyState.Patrol);
            }
            // If the player has exited attack range, transition to chase state.
            else if (!enemyController.IsWithinAttackRange) {
                outsideAttackRangeTime -= Time.deltaTime;
                if (outsideAttackRangeTime <= 0) {
                    // If I disable the nav mesh agent for ranged enemy, this comes here? Why? Do the colliders disappear or something???
                    enemyController.TransitionToState(EnemyState.Chase);
                }
            }

            if(enemyController is RangedEnemyController rangedEnemyController)
            {
                if (rangedEnemyController.IsTooCloseToPlayer)
                {
                    rangedEnemyController.TransitionToState(EnemyState.Flee);
                }
            }
            
            //if all the above conditions are false, then attack the player
            enemyController.Attack();
        }
    }
}