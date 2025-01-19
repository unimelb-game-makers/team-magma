// Author : Peiyu Wang @ Daphatus
// 19 01 2025 01 54

using System.Collections;
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
            throw new System.NotImplementedException();
        }

        protected override IEnumerator FastTempo(float duration)
        {
            throw new System.NotImplementedException();
        }

        public void Guard()
        {
            throw new System.NotImplementedException();
        }
        
        public void StopGuard()
        {
            throw new System.NotImplementedException();
        }
        
    }
}