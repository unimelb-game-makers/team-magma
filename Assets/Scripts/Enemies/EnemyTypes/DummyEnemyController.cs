using System.Collections;
using Damage;
using Enemy;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Scripting;

namespace Enemies.EnemyTypes
{
    public class DummyEnemyController : EnemyController
    {
        public override void Attack()
        {
        }

        protected override IEnumerator FastTempo(float duration)
        {
            yield return null;
        }

        protected override IEnumerator SlowTempo(float duration)
        {
            yield return null;

        }
    }
}

