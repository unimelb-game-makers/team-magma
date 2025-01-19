// Author : Peiyu Wang @ Daphatus
// 19 01 2025 01 54

using System.Collections;
using System.Collections.Generic;
using Enemies.EnemyStates;
using Enemy;

namespace Enemies.EnemyTypes
{
    //guarding
    
    public class EliteEnemyController : MeleeEnemyController
    {
        protected override bool CanDamage => CurrentState is GuardState;
        
        protected override IEnumerator SlowTempo(float duration)
        {
            return base.SlowTempo(duration);
        }
        
        protected override IEnumerator FastTempo(float duration)
        {
            return base.FastTempo(duration);
        }
        
        protected virtual void AddStates()
        {
            _states = new Dictionary<EnemyState, BaseEnemyState>()
            {
                { EnemyState.Idle, new IdleState(this, navMeshAgent, player) },
                { EnemyState.Patrol, new PatrolState(this, navMeshAgent, player) },
                { EnemyState.Chase, new ChaseState(this, navMeshAgent, player) },
                { EnemyState.Attack, new EliteAttackState(this, navMeshAgent, player) },
                { EnemyState.Guard, new GuardState(this, navMeshAgent, player) }
            };
        }
        public void Guard()
        {
            //Animation
        }
        
        public void StopGuard()
        {
            //Animation
        }
        
    }
}