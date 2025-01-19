// Author : Peiyu Wang @ Daphatus
// 19 01 2025 01 54

using System.Collections;
using Enemy;

namespace Enemies.EnemyTypes
{
    public class EliteEnemyController : EnemyController
    {
        public override void Attack()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator SlowTempo(float duration)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator FastTempo(float duration)
        {
            throw new System.NotImplementedException();
        }
    }
}