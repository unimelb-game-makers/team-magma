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
            // Do nothing
        }

        public override void Die() {
            // Call the other instruction screen and spawn the other enemy
            GetComponent<TriggerInstructionAndSpawnEnemy>().ShowInstructionScreen();
            base.Die();
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

