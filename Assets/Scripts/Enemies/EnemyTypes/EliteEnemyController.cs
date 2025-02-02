// Author : Peiyu Wang @ Daphatus
// 19 01 2025 01 54

using System.Collections;
using System.Collections.Generic;
using Damage;
using Enemies.EnemyStates;
using Enemy;
using UnityEngine;

namespace Enemies.EnemyTypes
{
    public class EliteEnemyController : MeleeEnemyController
    {
        [Header("Elite Enemy Attack")]
        [SerializeField] private GameObject guardIndicator;
        [SerializeField] private GameObject proximityAreaPrefab;
        [SerializeField] private float proximityDamage = 10f;
        [SerializeField] private float proximityDamageInterval = 0.5f;
        private GameObject proximityAreaInstance;
        private Damageable damageable;
        private bool canAttack;
        
        protected override void Awake()
        {
            base.Awake();

            damageable = GetComponent<Damageable>();
            damageable.setIsInvulnerable(true);

            damageRadius = originalDamageRadius;
            damageAngle = originalDamageAngle;
        }

        public override void Update()
        {
            // Check if the proximity area instance exists and create one if it does not.
            if (proximityAreaInstance == null)
            {
                proximityAreaInstance = Instantiate(
                    proximityAreaPrefab, 
                    transform.position, 
                    transform.rotation, 
                    transform
                );
                proximityAreaInstance.GetComponent<EliteEnemyProximityDamage>()
                    .Initialize(proximityDamage, proximityDamageInterval, gameObject);
            }

            base.Update();
        }

        public override void Attack()
        {
            if (canAttack)
            {
                if (!IsAttacking())
                {
                    // Visual effect to show the enemy is not guarding.
                    guardIndicator.SetActive(false);

                    // When elite enemy starts attacking, it can be damaged.
                    damageable.setIsInvulnerable(false);
                    // Set isTrigger to true so player or other enemies can pass through.
                    enemyCollider.isTrigger = true;
                    base.Attack();
                }
            }
        }

        protected override void EndStrike()
        {
            // Visual effect to show the enemy is guarding.
            guardIndicator.SetActive(true);

            // Elite enemy guard is up and cannot be damaged.
            damageable.setIsInvulnerable(true);
            // Set isTrigger to false so player or other enemies cannot pass through.
            enemyCollider.isTrigger = false;
            base.EndStrike();
        }

        public void Guard()
        {
            //Animation
        }
        
        public void StopGuard()
        {
            //Animation
        }

        #region Tempo Overrides
        protected override void DefaultTempo()
        {
            windUpTime = originalWindUpTime;
            attackCooldown = originalAttackCooldown;
            canAttack = true;
        }

        protected override IEnumerator SlowTempo(float duration)
        {
            canAttack = false;
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }

        protected override IEnumerator FastTempo(float duration)
        {
            windUpTime = originalWindUpTime * 0.5f;
            attackCooldown = originalAttackCooldown * 0.5f;
            canAttack = true;
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }
        #endregion
    }
}