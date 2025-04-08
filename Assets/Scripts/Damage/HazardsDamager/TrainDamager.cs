// Author : Peiyu Wang @ Daphatus
// 19 03 2025 03 58

using System;
using Player;
using UnityEngine;

namespace Damage.HazardsDamager
{
    public class TrainDamager : MonoBehaviour
    {
        [SerializeField] private float damage = 10;

        public float Damage
        {
            get => damage;
            set => damage = value;
        }

        public void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<Damageable>()==null) return;
            DealDamage(other.gameObject.GetComponent<Damageable>());
        }

        /**
         * Deal damage to the object.
         */
        public void DealDamage(Damageable target)
        {
            target.TakeDamage(damage);

            // Apply a knockback to enemies
            var enemyController = target.GetComponent<PlayerCharacter>();
            if (enemyController != null)
            {
                // Get the player position
                Vector3 playerPos = transform.parent.position;

                // Apply knockback directly to position (manual movement)
                Vector3 knockbackDirection = (target.transform.position - playerPos).normalized;
                knockbackDirection.y = 0; // Keep knockback horizontal (optional)

                // The knockback distance is between 1 to 5 depending on damage
                float knockbackDistance = Mathf.Clamp(damage * 0.1f, 1f, 5f);
            }
        }
    }
}