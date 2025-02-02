using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damage
{
    public class EliteEnemyProximityDamage : MonoBehaviour, IDamageManager
    {
        private GameObject parent;
        private float damage;
        private float interval;

        public void Initialize(float damage, float interval, GameObject parent)
        {
            this.damage = damage;
            this.interval = interval;
            this.parent = parent;
        }

        public void DealDamage(Damageable target)
        {
            // Do not deal damage to itself
            if (target.gameObject == parent) return;

            target.TakeDamage(damage);
            Destroy(gameObject, interval);
        }
    }
}