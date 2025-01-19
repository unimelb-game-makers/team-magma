using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.EnemyStates;
using Platforms;
using Player;
using Tempo;
using Timeline;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class EnemyController : MonoBehaviour, ITriggerCheckable, ISyncable
    {
        protected PlayerController player;
        private MusicTimeline timeline;
        private Rigidbody rigidbody;
        protected NavMeshAgent navMeshAgent;
        private Animator animator;
        private AudioSource audioSource;

        [Header("Health and Damage")]
        [SerializeField] private float health = 100;
        [SerializeField] protected float damage = 10;

        [Header("Idle")]
        [SerializeField] private float idleDuration = 3f;
        [SerializeField] private string idleAnimationBool = "isIdle";
        [SerializeField] private AudioClip idleAudio;
        
        [Header("Patrol")]
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private List<Transform> patrolPoints;
        private Transform currentPatrolPoint;
        private int patrolIndex = 0;
        [SerializeField] private string patrolAnimationBool = "isPatrol";
        [SerializeField] private AudioClip patrolAudio;

        [Header("Chase")]
        [SerializeField] private float playerLocationCheckInterval = 0.1f;
        private float currentPlayerLocationCheckTime = 0;
        [SerializeField] private float chaseDuration = 10f;
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private string chaseAnimationBool = "isChase";
        [SerializeField] private AudioClip chaseAudio;
        
        [Header("Attack")]
        [SerializeField] private float outsideAttackRangeDuration = 0.5f;
        [SerializeField] protected float attackCooldown = 1f;
        private float currentAttackCooldown;
        private bool isAttacking = false;
        [SerializeField] private string attackAnimationTrigger = "isAttack";
        [SerializeField] private string idleAttackAnimationBool = "isAttackIdle";
        [SerializeField] private AudioClip attackAudio;
        [SerializeField] private AudioClip idleAttackAudio;

        
        protected float originalDamage;
        protected float originalAttackCooldown;
        protected float originalWindUpTime;
        protected float originalDamageAngle;
        protected float originalDamageRadius;
        private float originalPatrolSpeed;
        private float originalChaseSpeed;
        
        private BaseEnemyState _currentState; protected BaseEnemyState CurrentState => _currentState;
        
        public bool IsWithinAggroRange { get; set; }
        public bool IsWithinAttackRange { get; set; }
        public bool HasReachedPatrolPoint { get; set; }

        public bool IsWithinAttackRangeClose
        {
            get
            {
                if(player == null) return false;
                return Vector3.Distance(player.transform.position, transform.position) < 3f;
            }
        }

        // These variables are used to turn the enemy so that it faces the direction it is moving in,
        // or so that it faces the player when in attack range.
        [Header("Other")]
        [SerializeField] private float rotationTime = 0.3f;
        private float yVelocity;    // This variable does not do anything, just to plug something in the method.

        [Header("States")]
        protected Dictionary<EnemyState, BaseEnemyState> _states;

        public PlayerController GetPlayerController() { return player; }
        public MusicTimeline GetMusicTimeline() {return timeline; }
        public Rigidbody GetRigidbody() { return rigidbody; }
        public NavMeshAgent GetNavMeshAgent() { return navMeshAgent; }

        public float GetHealth() { return health; }

        public void SetHealth(float newHealth)
        {
            if(CanDamage)
                health = newHealth;
            else
            {
                Debug.Log("Enemy is invulnerable");
            }
            
        }
        
        protected virtual bool CanDamage => true;
        public float GetDamage() { return damage; }
        
        public float GetIdleDuration() { return idleDuration; }
        public float GetPatrolSpeed() { return patrolSpeed; }
        public Transform GetCurrentPatrolPoint() { return currentPatrolPoint; }
        public float GetChaseDuration() { return chaseDuration; }
        public float GetChaseSpeed() { return chaseSpeed; }
        public float GetOutsideAttackRangeDuration() { return outsideAttackRangeDuration; }
        public float GetAttackCooldown() { return attackCooldown; }
        protected float GetCurrentAttackCooldown() { return currentAttackCooldown; }
        public void SetCurrentAttackCooldown(float currentAttackCooldown) { this.currentAttackCooldown = currentAttackCooldown; }
        public bool IsAttacking() { return isAttacking; }

        public void SetIsAttacking(bool isAttacking)
        {
            Debug.Log("Setting isAttacking to " + isAttacking);
            this.isAttacking = isAttacking;
        }

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

        public void SetAggroRangeBool(bool isWithinAggroRange) { IsWithinAggroRange = isWithinAggroRange; }
        public void SetAttackRangeBool(bool isWithinAttackRange) { IsWithinAttackRange = isWithinAttackRange; }
        public void SetHasReachedPatrolPointBool(bool hasReachedPatrolPoint) { HasReachedPatrolPoint = hasReachedPatrolPoint; }
        
        public BaseEnemyState GetState(EnemyState state)
        {
            if (_states.TryGetValue(state, out var state1))
            {
                return state1;
            }
            return null;
        }
        
        protected virtual void Awake() {
            player = FindObjectOfType<PlayerController>();
            timeline = FindObjectOfType<MusicTimeline>();
            rigidbody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            
            AddStates();
            
            _currentState = GetState(EnemyState.Idle);
            _currentState.EnterState();

            currentPatrolPoint = patrolPoints[patrolIndex];
            
            
            originalDamage = damage;
            originalAttackCooldown = attackCooldown;
            originalPatrolSpeed = patrolSpeed;
            originalChaseSpeed = chaseSpeed;
        }
        
        protected virtual void AddStates()
        {

            _states = new Dictionary<EnemyState, BaseEnemyState>()
            {
                { EnemyState.Idle, new IdleState(this, navMeshAgent, player) },
                { EnemyState.Patrol, new PatrolState(this, navMeshAgent, player) },
                { EnemyState.Chase, new ChaseState(this, navMeshAgent, player) },
                { EnemyState.Attack, new AttackState(this, navMeshAgent, player) },
                { EnemyState.Flee, new FleeState(this, navMeshAgent, player) }
            };
        }
        
        void Update() {
            _currentState.UpdateState();
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

        public virtual void Idle() {
            // Something
        }

        public virtual void Patrol() {
            RotateTowardsMovementDirection();
        }

        public virtual void Chase() {
            RotateTowardsMovementDirection();
            
            // If the player is dead, do nothing.
            if (player == null) return;

            // No need to check player location every frame.
            currentPlayerLocationCheckTime -= Time.deltaTime;
            if (currentPlayerLocationCheckTime > 0) return;
            currentPlayerLocationCheckTime = playerLocationCheckInterval;
            navMeshAgent.destination = player.transform.position;
        }
        


        public abstract void Attack();

        // Set the next patrol point. If the last patrol point was reached, reset.
        public void NextPatrolPoint() {
            patrolIndex++;
            if (patrolIndex >= patrolPoints.Count) patrolIndex = 0;
            currentPatrolPoint = patrolPoints[patrolIndex];
        }

        // Rotate smoothly towards movement direction / towards player when in attack range.
        protected void RotateTowardsMovementDirection() {
            if (navMeshAgent.velocity != Vector3.zero) {
                float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                                                            Quaternion.LookRotation(navMeshAgent.velocity).eulerAngles.y, 
                                                            ref yVelocity, rotationTime);
                navMeshAgent.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
            }
        }

        // Rotate smoothly towards player when in attack range
        protected void RotateTowardsPlayer() {
            if (player == null) return;
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            Vector3 playerDirectionWithoutY = new Vector3(playerDirection.x, 0, playerDirection.z);
            float lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                                                        Quaternion.LookRotation(playerDirectionWithoutY).eulerAngles.y, 
                                                        ref yVelocity, rotationTime);
            navMeshAgent.transform.rotation = Quaternion.Euler(0, lookDirection, 0);
        }

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
            patrolSpeed = originalPatrolSpeed;
            chaseSpeed = originalChaseSpeed;
        }

        protected abstract IEnumerator SlowTempo(float duration);

        protected abstract IEnumerator FastTempo(float duration);
    }

    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Flee
    }
}