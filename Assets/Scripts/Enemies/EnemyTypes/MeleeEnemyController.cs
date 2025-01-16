using System.Collections;
using Damage;
using Enemy;
using UnityEngine;

namespace Enemies.EnemyTypes
{
    public class MeleeEnemyController : EnemyController
    {
        [Header("Strike")]
        [SerializeField] private float strikeSpeed = 5f;
        [SerializeField] private float strikeDuration = 0.5f;
        private bool isStriking = false;
        private float currentStrikeTime;
        private Vector3 strikeDirection;
        private bool hasCollidedWithPlayer = false;

        public override void Attack() {
            if (isStriking) {
                UpdateStrike();
            }
            else {
                hasCollidedWithPlayer = false;
                RotateTowardsPlayer();

                // Calculate the current cooldown time. If cooldown is over, attack.
                SetCurrentAttackCooldown(GetCurrentAttackCooldown() - Time.deltaTime); 
                if (GetCurrentAttackCooldown() <= 0) {
                    SetCurrentAttackCooldown(GetAttackCooldown());

                    Debug.Log("Melee Attack!");
                    GetAnimator().SetTrigger(GetAttackAnimationTrigger());

                    // Temporary until actual logic is implemented.
                    // Eg: Could wait until the animation has finished playing.
                    SetIsAttacking(true);

                    // If player is still alive.
                    if (GetPlayerController()) StartStrike();
                }
            }
        }

        private void StartStrike() {
            isStriking = true;
            GetNavMeshAgent().enabled = false;
            // The strike cannot change direction midway.
            // Calculate the direction without y, so enemy stays on the ground.
            Vector3 dashDirectionWithY = GetPlayerController().transform.position - transform.position;
            strikeDirection = new Vector3(dashDirectionWithY.x, 0f, dashDirectionWithY.z).normalized;
            currentStrikeTime = strikeDuration;
        }

        private void UpdateStrike() {
            currentStrikeTime -= Time.deltaTime;

            // If the strike's duration has ended or the player was hit, end the strike.
            if (currentStrikeTime <= 0 || hasCollidedWithPlayer) {
                isStriking = false;
                GetRigidbody().velocity = Vector3.zero;
                SetIsAttacking(false);
                GetNavMeshAgent().enabled = true;

                if (hasCollidedWithPlayer) {
                    // Player has taken damage
                    GetPlayerController().GetComponent<Damageable>().TakeDamage(GetDamage());
                }
            } else {
                GetRigidbody().velocity = strikeDirection * strikeSpeed;
            }
        }

        // Temporary methods for checking for collisions with the player.
        // Checks if the enemy has collided with the player.
        // Could implement this for all enemy types and apply a knockback to all of them, interrupting their attack.
        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Player")) {
                Debug.Log(gameObject + " Melee Enemy has collided with the player!");
                hasCollidedWithPlayer = true;
            }
        }
        
        protected override IEnumerator SlowTempo(float duration)
        {
            //https://docs.google.com/document/d/1K65hq-uRl9L1aO6luQ29W-MZi8Z0XvnxqSZyk1JGLyA/edit?tab=t.0

            damage = originalDamage * 1.75f;

            attackCooldown = originalAttackCooldown * 1.5f; 

            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }

        protected override IEnumerator FastTempo(float duration)
        {
            attackCooldown = originalAttackCooldown * 0.5f;
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }
    }
}
