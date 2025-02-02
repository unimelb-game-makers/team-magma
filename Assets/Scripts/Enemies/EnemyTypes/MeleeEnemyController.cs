using System.Collections;
using Damage;
using Enemy;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Scripting;

namespace Enemies.EnemyTypes
{
    public class MeleeEnemyController : EnemyController
    {
        protected Collider enemyCollider;

        [Header("Melee Attack Variables")]
        [SerializeField] private GameObject damageAreaPrefab;
        [SerializeField] protected float originalWindUpTime = 0.3f;
        [SerializeField] protected float originalDamageRadius = 2.5f;
        [SerializeField] protected float originalDamageAngle = 90f;
        protected float windUpTime;
        protected float damageRadius;
        protected float damageAngle;

        protected override void Awake()
        {
            enemyCollider = GetComponent<Collider>();
            base.Awake();
        }
        
        public override void Attack()
        {
            if (!IsAttacking())
            {
                // Trigger animation
                // GetAnimator().SetTrigger(GetAttackAnimationTrigger());

                SetIsAttacking(true);
                StartStrike();
            }
        }

        private void StartStrike()
        {
            // Instantiate the damage area
            GameObject damageAreaInstance = Instantiate(
                damageAreaPrefab, 
                transform.position, 
                transform.rotation, 
                transform
            );

            // Get and initialize the AreaDamage component
            AreaDamage areaDamage = damageAreaInstance.GetComponent<AreaDamage>();
            areaDamage.InitializeAttack(damage, damageRadius, damageAngle, windUpTime, gameObject);

            // Start the coroutine for the entire wind-up → damage → dash flow
            StartCoroutine(PerformStrikeSequence(areaDamage));
        }

        /// <summary>
        /// A coroutine that handles:
        /// 1) Wind-up time
        /// 2) Dealing damage
        /// 3) Dashing for strikeDuration
        /// 4) Ending the strike
        /// </summary>
        private IEnumerator PerformStrikeSequence(AreaDamage areaDamage)
        {
            float timer = 0f;

            // ---------------------------------------
            // 1) WIND-UP PHASE
            // ---------------------------------------
            while (timer < windUpTime)
            {
                timer += Time.deltaTime;
                // Call areaDamage.WindUp() to visualize or grow the area each frame
                areaDamage.WindUp();

                yield return null;
            }

            // ---------------------------------------
            // 2) DEAL DAMAGE AFTER WIND-UP
            // ---------------------------------------
            areaDamage.DealDamage();

            // ---------------------------------------
            // 4) END STRIKE
            // ---------------------------------------
            Invoke(nameof(EndStrike), attackCooldown);
        }

        protected virtual void EndStrike()
        {
            SetIsAttacking(false);
        }
        
        #region Tempo Overrides
        protected override void DefaultTempo()
        {
            base.DefaultTempo();
            windUpTime = originalWindUpTime;
            damageRadius = originalDamageRadius;
            damageAngle = originalDamageAngle;

            // Set isTrigger to true so player or other enemies can pass through.
            enemyCollider.isTrigger = true;
        }

        protected override IEnumerator SlowTempo(float duration)
        {
            // Set isTrigger to false so player or other enemies cannot pass through.
            enemyCollider.isTrigger = false;

            damage = originalDamage * 1.75f;
            windUpTime = originalWindUpTime * 2f;
            attackCooldown = originalAttackCooldown * 1.5f; 
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }

        protected override IEnumerator FastTempo(float duration)
        {
            /*
               Mostly the same as the default tempo (same damage, etc.), 
               but AoE is wider, and the attack has a higher “rate of fire”/lower cooldown 
               (the enemies attack more often at the fast tempo)
             */
            damage = originalDamage;
            damageRadius = originalDamageRadius * 1.5f;
            damageAngle = originalDamageAngle * 1.5f;
            windUpTime = originalWindUpTime * 0.5f;
            attackCooldown = originalAttackCooldown * 0.5f;
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }
        #endregion
    }
}