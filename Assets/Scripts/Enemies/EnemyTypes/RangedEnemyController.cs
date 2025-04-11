using System.Collections;
using System.Collections.Generic;
using Enemies.EnemyStates;
using Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.EnemyTypes
{
    public class RangedEnemyController : EnemyController
    {
        [Header("Audio SFX")]
        [SerializeField] private FMODUnity.EventReference fleeSoundReference;
        private FMOD.Studio.EventInstance fleeSound;

        [Header("Ranged Attack Variables")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float originalWindUpTime = 0.3f;
        [SerializeField] private float originalInaccuracyAmount = 5f;
        [SerializeField] private float originalProjectileSpeed = 20f;
        private float windUpTime;
        private float inaccuracyAmount;
        private float projectileSpeed;
        private bool isProjectileHoming;

        [Header("Flee Variables")]
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private float fleeSpeed = 10f;
        [SerializeField] private float fleeRange = 5f;
        [SerializeField] private float moveToRange = 3f;
        private bool enemyInFleeRange;
        private bool enemyMovedToFleeLocation;
        private Vector3 fleeLocation;

        public float GetFleeSpeed() { return fleeSpeed; }
        public bool EnemyIsInFleeRange() { return enemyInFleeRange; }
        public bool EnemyHasMovedToFleeLocation() { return enemyMovedToFleeLocation; }

        public FMOD.Studio.EventInstance GetFleeSound() {
            return fleeSound;
        }

        public override void Update()
        {
            // Update flee ranges
            if (Player != null)
                enemyInFleeRange = Vector3.Distance(transform.position, Player.transform.position) <= fleeRange;
            enemyMovedToFleeLocation = Vector3.Distance(transform.position, fleeLocation) <= destinationToleranceRange;
            
            base.Update();
        }

        protected override void AddStates()
        {
            _states = new Dictionary<EnemyState, BaseEnemyState>()
            {
                { EnemyState.Idle, new IdleState(this, agent, Player) },
                { EnemyState.Patrol, new PatrolState(this, agent, Player) },
                { EnemyState.Chase, new ChaseState(this, agent, Player) },
                { EnemyState.Attack, new AttackState(this, agent, Player) },
                { EnemyState.Flee, new FleeState(this, agent, Player) }
            };
        }
        
        #region Attack
        public override void Attack() {
            if (!IsAttacking())
            {
                // Trigger animation
                // GetAnimator().SetTrigger(GetAttackAnimationTrigger());

                SetIsAttacking(true);

                // If the Player is still alive.
                if (Player) {
                    StartCoroutine(PerformStrikeSequence());
                }

                /*
                * NOTE: This event audio is triggered using a StudioEventEmitter component attached to this enemy.
                * The audio gets louder closer you are to the enemy, this is because it is using the FMOD Studio Listener Component
                * attached to the camera for Attenuation (basic top-down 3d directional audio falloff).
                * However the StudioEventEmitter "Override Attenuation" setting WONT WORK because the "Wooden Collision" FMOD audio track
                * is uses a spacializer that overrides this.
                */

                // Trigger Ranged Attack Audio Event
                //StudioEventEmitter fire = GetComponent<FMODUnity.StudioEventEmitter>();
                //fire.Play();
                //FMODUnity.RuntimeManager.AttachInstanceToGameObject(fire.EventInstance, gameObject, GetComponent<Rigidbody>());
            }
        }

        private IEnumerator PerformStrikeSequence()
        {
            float timer = 0f;

            // ---------------------------------------
            // 1) WIND-UP PHASE
            // ---------------------------------------
            while (timer < windUpTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            // ---------------------------------------
            // 2) Fire projectile
            // ---------------------------------------
            SpawnProjectile();
            GetAttackSound().start();

            // ---------------------------------------
            // 3) END Attack
            // ---------------------------------------
            Invoke(nameof(EndShot), attackCooldown);
        }

        private void SpawnProjectile() {
            if (Player == null) return;
            // The projectile should move in this direction
            Vector3 direction = (Player.transform.position - transform.position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, transform.position + direction, transform.rotation);

            // Initialize the bullet's parent to this enemy
            // This is so the bullet does not damage the enemy that shot it.
            if (projectile.GetComponent<BulletDamager>())
                projectile.GetComponent<BulletDamager>().Initialize(gameObject);

            // Get the Projectile component from the projectile object.
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            // Check if the Projectile component exists.
            if (projectileComponent != null) {
                var canAttackList = new List<string> {"Player", "Enemy"};
                projectileComponent.SendMessage("EditCanAttack", canAttackList);

                projectileComponent.IsHoming = isProjectileHoming;
                projectileComponent.Speed = projectileSpeed;

                // Introduce random inaccuracy
                direction = ApplyInaccuracy(direction, inaccuracyAmount);

                projectileComponent.SetInitialDirection(new Vector3(direction.x, 0f, direction.z), Player.gameObject);
            }
        }

        // Apply innacuracy to projectiles fired.
        private Vector3 ApplyInaccuracy(Vector3 direction, float maxAngle)
        {
            // Generate a random deviation within the specified maxAngle
            float randomYaw = Random.Range(-maxAngle, maxAngle);

            // Apply rotation to the direction vector
            Quaternion inaccuracyRotation = Quaternion.Euler(0, randomYaw, 0);
            return inaccuracyRotation * direction;
        }

        private void EndShot()
        {
            SetIsAttacking(false);
        }
        #endregion

        // When flee state starts, find a location to flee to.
        public Vector3 GetFleeLocation()
        {
            // Get a random angle between -45 and +45 degrees (behind the enemy)
            float randomAngle = Random.Range(-45f, 45f);

            // Convert angle to a direction vector (behind the enemy)
            Vector3 fleeDirection = Quaternion.Euler(0, randomAngle + 180f, 0) * transform.forward;

            // Get a random distance within moveToRange
            // Avoid too close distances
            float randomDistance = Random.Range(moveToRange, moveToRange);

            // Calculate the flee position
            fleeLocation = transform.position + fleeDirection * randomDistance;

            // Ensure the position is on the ground
            if (Physics.Raycast(fleeLocation, Vector3.down, 2f, whatIsGround))
            {
                return fleeLocation;
            }
            // If invalid, return current position
            else {
                fleeLocation = transform.position;
            }
            
            return fleeLocation;
        }

        public override void SetAudioVolume(float masterVolume, float sfxVolume)
        {
            base.SetAudioVolume(masterVolume, sfxVolume);
            if (fleeSound.isValid()) {
                fleeSound.setVolume(sfxVolume * masterVolume * sfxModifier);
            }
        }

        public override void PauseAudio(bool pause)
        {
            base.PauseAudio(pause);
            fleeSound.setPaused(pause);
        }

        #region Tempo Overrides
        protected override void DefaultTempo()
        {
            base.DefaultTempo();
            windUpTime = originalWindUpTime;
            inaccuracyAmount = originalInaccuracyAmount;
            projectileSpeed = originalProjectileSpeed;
            isProjectileHoming = false;
        }
        
        protected override IEnumerator SlowTempo(float duration)
        {
            windUpTime = originalWindUpTime * 2f;
            attackCooldown = originalAttackCooldown * 2f;
            projectileSpeed = originalProjectileSpeed * 0.8f;
            isProjectileHoming = true;
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }

        protected override IEnumerator FastTempo(float duration)
        {
            windUpTime = originalWindUpTime / 2f;
            attackCooldown = originalAttackCooldown / 2f;
            inaccuracyAmount = originalInaccuracyAmount * 3f;
            projectileSpeed = originalProjectileSpeed * 1.5f;
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, destinationToleranceRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, fleeRange);
        }
    }
}
