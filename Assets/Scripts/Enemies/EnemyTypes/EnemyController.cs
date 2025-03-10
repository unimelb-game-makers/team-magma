using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.EnemyStates;
using Enemies.EnemyTypes;
using Platforms;
using Player;
using Tempo;
using Timeline;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyController : MonoBehaviour, ISyncable
    {
        #region Enemy Variables
        protected PlayerController player;
        protected NavMeshAgent agent;

        [SerializeField] private float health = 100f;

        // Idle Variables
        [Header("Idle Variables")]
        [SerializeField] private float idleDuration = 3f;
        
        // Patrol Variables
        [Header("Patrol Variables")]
        [SerializeField] private bool presetPatrolRoute = false;
        [SerializeField] private float patrolSpeed = 5f;
        private Vector3 currentPatrolPoint;

        // Manually set the patrol points.
        [SerializeField] private List<Transform> patrolPoints;
        private int patrolIndex = 0;
        
        // Chase Variables
        [Header("Chase Variables")]
        [SerializeField] private float chaseSpeed = 12f;
        [SerializeField] private float chaseDuration = 3f;
        [SerializeField] private float playerLocationCheckInterval = 0.1f;
        private float currentPlayerLocationCheckTime = 0f;
        
        // Attack Variables
        [Header("Attack Variables")]
        [SerializeField] protected float originalDamage;
        [SerializeField] protected float originalAttackCooldown;
        [SerializeField] protected float outsideAttackRangeDuration;
        private bool isAttacking;
        
        // Current Damage Variables for the current tempo.
        protected float damage;
        protected float attackCooldown;

        // Destination, Sight, Attack Ranges
        [Header("Destination Tolerance, Sight, Attack Ranges")]
        [SerializeField] protected float destinationToleranceRange = 1.5f;
        [SerializeField] protected float sightRange = 10f;
        [SerializeField] protected float attackRange = 2f;
        private bool enemyInDestinationRange, playerInSightRange, playerInAttackRange;

        // States
        [Header("States")]
        protected Dictionary<EnemyState, BaseEnemyState> _states;
        private BaseEnemyState _currentState;
        protected BaseEnemyState CurrentState => _currentState;
        #endregion

        /*
        #region Music Timeline, Animations and Audio
        private MusicTimeline timeline;
        private Animator animator;
        private AudioSource audioSource;

        [Header("Animations and Audio")]
        [SerializeField] private string idleAnimationBool = "isIdle";
        [SerializeField] private string patrolAnimationBool = "isPatrol";
        [SerializeField] private string chaseAnimationBool = "isChase";
        [SerializeField] private string attackAnimationTrigger = "isAttack";
        [SerializeField] private string idleAttackAnimationBool = "isAttackIdle";
        
        [SerializeField] private AudioClip idleAudio;
        [SerializeField] private AudioClip patrolAudio;
        [SerializeField] private AudioClip chaseAudio;
        [SerializeField] private AudioClip attackAudio;
        [SerializeField] private AudioClip idleAttackAudio;

        public MusicTimeline GetMusicTimeline() {return timeline; }

        public Animator GetAnimator() { return animator; }
        public string GetIdleAnimationBool() { return idleAnimationBool; }
        public string GetPatrolAnimationBool() { return patrolAnimationBool; }
        public string GetChaseAnimationBool() { return chaseAnimationBool; }
        public string GetAttackAnimationTrigger() { return attackAnimationTrigger; }
        public string GetIdleAttackAnimationBool() { return idleAttackAnimationBool; }

        public AudioSource GetAudioSource() { return audioSource; }
        public AudioClip GetIdleAudio() { return idleAudio; }
        public AudioClip GetPatrolAudio() { return patrolAudio; }
        public AudioClip GetChaseAudio() { return chaseAudio; }
        public AudioClip GetAttackAudio() { return attackAudio; }
        public AudioClip GetIdleAttackAudio() { return idleAttackAudio; }
        #endregion
        */

        #region Enemy State Getters and Setters
        public PlayerController GetPlayerController() { return player; }
        public NavMeshAgent GetNavMeshAgent() { return agent; }
        
        // Health
        public float GetHealth() { return health; }

        public void SetHealth(float newHealth)
        {
            health = newHealth;
        }
        
        // State Variables
        public float GetIdleDuration() { return idleDuration; }
        public float GetPatrolSpeed() { return patrolSpeed; }
        public Vector3 GetCurrentPatrolPoint() { return currentPatrolPoint; }
        public float GetChaseDuration() { return chaseDuration; }
        public float GetChaseSpeed() { return chaseSpeed; }
        public float GetOutsideAttackRangeDuration() { return outsideAttackRangeDuration; }
        public float GetAttackCooldown() { return attackCooldown; }
        public bool IsAttacking() { return isAttacking; }
        public void SetIsAttacking(bool isAttacking)
        {
            this.isAttacking = isAttacking;
        }

        public bool EnemyIsInDestinationRange() { return enemyInDestinationRange; }
        public bool PlayerIsInSightRange() { return playerInSightRange; }
        public bool PlayerIsInAttackRange() { return playerInAttackRange; }
        
        public BaseEnemyState GetState(EnemyState state)
        {
            if (_states.TryGetValue(state, out var state1))
            {
                return state1;
            }
            return null;
        }
        #endregion
        
        protected virtual void Awake() {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
            agent = GetComponent<NavMeshAgent>();
            agent.speed = patrolSpeed;

            // timeline = FindObjectOfType<MusicTimeline>();
            // animator = GetComponent<Animator>();
            // audioSource = GetComponent<AudioSource>();
            
            AddStates();
            
            _currentState = GetState(EnemyState.Idle);
            _currentState.EnterState();

            if (patrolPoints.Count > 0)
                currentPatrolPoint = patrolPoints[patrolIndex].position;
            else
                currentPatrolPoint = transform.position;
            DefaultTempo();
        }
        
        protected virtual void AddStates()
        {
            _states = new Dictionary<EnemyState, BaseEnemyState>()
            {
                { EnemyState.Idle, new IdleState(this, agent, player) },
                { EnemyState.Patrol, new PatrolState(this, agent, player) },
                { EnemyState.Chase, new ChaseState(this, agent, player) },
                { EnemyState.Attack, new AttackState(this, agent, player) },
            };
        }

        // Temporary variables for testing
        [SerializeField] private bool slowTempo = false;
        [SerializeField] private bool fastTempo = false;
        
        public virtual void Update() {
            // Update ranges
            enemyInDestinationRange = Vector3.Distance(transform.position, currentPatrolPoint) <= destinationToleranceRange;
            if (player != null)
            {
                playerInSightRange = Vector3.Distance(transform.position, player.transform.position) <= sightRange;
                playerInAttackRange = Vector3.Distance(transform.position, player.transform.position) <= attackRange;
            }

            _currentState.UpdateState();

            // Temporary code for testing
            if (slowTempo)
            {
                slowTempo = false;
                Affect(TapeType.Slow, 30f, 0);
            } else if (fastTempo)
            {
                fastTempo = false;
                Affect(TapeType.Fast, 30f, 0);
            }
        }

        // Exit the current state, and enter the new state.
        public void TransitionToState(EnemyState stateType) {
            var newState = GetState(stateType);
            if (newState == null)
            {
                throw new Exception("State not found ++++++++++++++++++");
            }
            _currentState.ExitState();
            _currentState = newState;
            newState.EnterState();
        }

        public virtual void Idle() {}

        public virtual void Patrol() {}

        // Set the next patrol point. If the last patrol point was reached, reset.
        public void NextPatrolPoint() {
            if (patrolPoints.Count == 0) return;
            patrolIndex++;
            if (patrolIndex >= patrolPoints.Count) patrolIndex = 0;
            currentPatrolPoint = patrolPoints[patrolIndex].position;
        }

        public virtual void Chase() {    
            // No need to check player location every frame.
            currentPlayerLocationCheckTime -= Time.deltaTime;
            if (currentPlayerLocationCheckTime > 0) return;
            currentPlayerLocationCheckTime = playerLocationCheckInterval;
            agent.SetDestination(player.transform.position);
        }

        public abstract void Attack();

        // Apply a knockback to the enemy
        public void ApplyKnockback(Vector3 direction, float distance)
        {
            // Apply knockback, then resume enemy AI after
            agent.isStopped = true;
            StartCoroutine(KnockbackRoutine(direction, distance));
        }

        private IEnumerator KnockbackRoutine(Vector3 direction, float distance)
        {
            float knockbackTime = 0.2f;
            float timer = 0f;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + direction * distance;

            while (timer < knockbackTime)
            {
                timer += Time.deltaTime;
                float t = timer / knockbackTime;

                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                yield return null;
            }

            agent.isStopped = false;
        }

        #region Tempo Overrides
        public void Affect(TapeType tapeType, float duration, float effectValue)
        {
            switch (tapeType)
            {
                case TapeType.Slow:
                    StartCoroutine(SlowTempo(duration));
                    break;
                case TapeType.Fast:
                    StartCoroutine(FastTempo(duration));
                    break;
            }
        }

        protected virtual void DefaultTempo()
        {
            // Revert to original stats
            damage = originalDamage;
            attackCooldown = originalAttackCooldown;
        }

        protected abstract IEnumerator SlowTempo(float duration);

        protected abstract IEnumerator FastTempo(float duration);
        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, destinationToleranceRange);
        }
    }

    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Flee,
        Guard
    }
}