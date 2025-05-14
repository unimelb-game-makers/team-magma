using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;
using Enemy;

namespace Enemies.EnemyTypes
{
    public class DummyMeleeEnemyController : MeleeEnemyController
    {
        public override void Die() {
            // Call the other instruction screen and spawn the other enemy
            GetComponent<TriggerInstructionAndSpawnEnemy>().ShowInstructionScreen();
            base.Die();
        }
    }
}