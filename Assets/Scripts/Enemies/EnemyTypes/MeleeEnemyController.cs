using System.Collections;
using Damage;
using Enemy;
using UnityEngine;

namespace Enemies.EnemyTypes
{
    public class MeleeEnemyController : EnemyController
    {

        [Header("Wind-up & Damage")]
        [SerializeField] private float windUpTime = 0.3f;
        [SerializeField] private float damageRadius = 1f;
        [SerializeField] private float damageAngle = 90f;

        [SerializeField] private GameObject damageAreaPrefab;

        private bool isStriking = false;
        private bool hasCollidedWithPlayer = false;

        private Vector3 strikeDirection;
        private AreaDamage areaDamage;

        public override void Attack()
        {
            if (isStriking)
            {
                // If we're already in the middle of striking, just continue.
                return;
            }
            hasCollidedWithPlayer = false;
            RotateTowardsPlayer();

            // Handle attack cooldown
            SetCurrentAttackCooldown(GetCurrentAttackCooldown() - Time.deltaTime); 
            if (GetCurrentAttackCooldown() <= 0)
            {
                // Reset the cooldown
                SetCurrentAttackCooldown(GetAttackCooldown());

                // Trigger animation
                GetAnimator().SetTrigger(GetAttackAnimationTrigger());
                SetIsAttacking(true);

                // If the player is still valid, begin the strike sequence
                if (GetPlayerController())
                {
                    StartStrike();
                }
            }
        }

        private void StartStrike()
        {
            isStriking = true;
            GetNavMeshAgent().enabled = false;

            // Calculate direction on the ground only
            Vector3 dashDirectionWithY = GetPlayerController().transform.position - transform.position;
            strikeDirection = new Vector3(dashDirectionWithY.x, 0f, dashDirectionWithY.z).normalized;

            // Instantiate the damage area
            GameObject damageAreaInstance = Instantiate(
                damageAreaPrefab, 
                transform.position, 
                transform.rotation, 
                transform
            );

            // Get and initialize the AreaDamage component
            areaDamage = damageAreaInstance.GetComponent<AreaDamage>();
            areaDamage.InitializeAttack(damage, damageRadius, damageAngle, windUpTime,gameObject);

            // Start the coroutine for the entire wind-up → damage → dash flow
            StartCoroutine(PerformStrikeSequence());
        }

        /// <summary>
        /// A coroutine that handles:
        /// 1) Wind-up time
        /// 2) Dealing damage
        /// 3) Dashing for strikeDuration
        /// 4) Ending the strike
        /// </summary>
        private IEnumerator PerformStrikeSequence()
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
            EndStrike();
        }

        private void EndStrike()
        {
            isStriking = false;
            GetRigidbody().velocity = Vector3.zero;
            SetIsAttacking(false);
            GetNavMeshAgent().enabled = true;
            
            //TODO: Check if the player is still within the attack range, if does, then attack again, else chase the player 
        }
        
        #region Tempo Overrides

        protected override IEnumerator SlowTempo(float duration)
        {
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
            windUpTime = originalWindUpTime * 0.5f;
            damageAngle = originalDamageAngle * 1.5f;
            attackCooldown = originalAttackCooldown * 0.5f;
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }

        #endregion
    }
}