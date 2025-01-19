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
        [Header("Projectile")]
        [SerializeField] private GameObject projectilePrefab;

        [SerializeField] private float tooCloseDistance = 5f;
        public bool IsTooCloseToPlayer { get => Vector3.Distance(transform.position, GetPlayerController().transform.position) < tooCloseDistance; }
        [SerializeField] private float fleeSpeed = 10f; public float FleeSpeed => fleeSpeed;


        protected override void AddStates()
        {
            _states = new Dictionary<EnemyState, BaseEnemyState>()
            {
                { EnemyState.Idle, new IdleState(this, navMeshAgent, player) },
                { EnemyState.Patrol, new PatrolState(this, navMeshAgent, player) },
                { EnemyState.Chase, new ChaseState(this, navMeshAgent, player) },
                { EnemyState.Attack, new RangedAttackState(this, navMeshAgent, player) },
                { EnemyState.Flee, new FleeState(this, navMeshAgent, player) }
            };
        }
        
        public override void Attack() {
            RotateTowardsPlayer();

            // Calculate the current cooldown time. If cooldown is over, attack.
            SetCurrentAttackCooldown(GetCurrentAttackCooldown() - Time.deltaTime); 
            //if (GetCurrentAttackCooldown() <= 0 && GetMusicTimeline().GetOnBeat()) {
            if (GetCurrentAttackCooldown() <= 0) {
                SetCurrentAttackCooldown(GetAttackCooldown());

                Debug.Log("Ranged Attack!");
                GetAnimator().SetTrigger(GetAttackAnimationTrigger());

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

            


                // Temporary until actual logic is implemented.
                // Eg: Could wait until the animation has finished playing.
                SetIsAttacking(true);

                // If the player is still alive.
                if (GetPlayerController()) SpawnProjectile();

                SetIsAttacking(false);
            }
        }

        protected override void DefaultTempo()
        {
            base.DefaultTempo();
        }

        private bool _isFleeing = false; public bool IsFleeing => _isFleeing;
        private float _fleeDuration = 5f;
        
        /**
        * Flee from the player by finding a random point on the NavMesh and moving to it.
        */
        public void Flee()
        {
            RotateTowardsMovementDirection();
            float fleeDistance = 20f;
            _isFleeing = true;
            Vector3 randomDirection = Random.insideUnitSphere * fleeDistance;

            randomDirection += transform.position;

            NavMeshHit hit;
            
            if (NavMesh.SamplePosition(randomDirection, out hit, fleeDistance, NavMesh.AllAreas))
            {
                // Set the destination to the position we found on the NavMesh.
                navMeshAgent.destination = hit.position;
        
                // Optionally adjust the movement speed while fleeing.
                // For instance, you might want to use chaseSpeed, or define a new fleeSpeed.
                navMeshAgent.speed = FleeSpeed;
            }
            else
            {
                Debug.LogWarning("Flee: Could not find a valid NavMesh position to flee to.");
            }
            StartCoroutine(FleeTimer(_fleeDuration));
        }
        private IEnumerator FleeTimer(float duration)
        {
            yield return new WaitForSeconds(duration);

            CurrentState.ExitState();
            _isFleeing = false;
        }
        
        private void SpawnProjectile() {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            // Get the Projectile component from the projectile object.
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            // Check if the Projectile component exists.
            if (projectileComponent != null) {
                var canAttackList = new List<string> { "Player", "Enemy" };
                projectileComponent.SendMessage("EditCanAttack", canAttackList);

                // Set the initial direction of the projectile.
                Vector3 direction = (GetPlayerController().transform.position - transform.position).normalized;
                projectileComponent.SetInitialDirection(new Vector3(direction.x, 0f, direction.z));
            }
        }
        
        protected override IEnumerator SlowTempo(float duration)
        {
            attackCooldown = originalAttackCooldown * 2;
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }

        protected override IEnumerator FastTempo(float duration)
        {
            yield return new WaitForSeconds(duration);
            DefaultTempo();
        }
    }
}
