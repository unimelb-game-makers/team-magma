using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class EnemyController : MonoBehaviour, ITriggerCheckable
    {
        private PlayerController player;
        private Rigidbody rigidbody;
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private AudioSource audioSource;

        [Header("Health and Damage")]
        [SerializeField] private float health = 100;
        [SerializeField] private float damage = 10;

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
        [SerializeField] private float attackCooldown = 1f;
        private float currentAttackCooldown;
        private bool isAttacking = false;
        [SerializeField] private string attackAnimationTrigger = "isAttack";
        [SerializeField] private string idleAttackAnimationBool = "isAttackIdle";
        [SerializeField] private AudioClip attackAudio;
        [SerializeField] private AudioClip idleAttackAudio;

        public bool IsWithinAggroRange { get; set; }
        public bool IsWithinAttackRange { get; set; }
        public bool HasReachedPatrolPoint { get; set; }

        // These variables are used to turn the enemy so that it faces the direction it is moving in,
        // or so that it faces the player when in attack range.
        [Header("Other")]
        [SerializeField] private float rotationTime = 0.3f;
        private float yVelocity;    // This variable does not do anything, just to plug something in the method.

        [Header("States")]
        private BaseEnemyState currentState;
        private IdleState idleState;
        private PatrolState patrolState;
        private ChaseState chaseState;
        private AttackState attackState;

        public PlayerController GetPlayerController() { return player; }
        public Rigidbody GetRigidbody() { return rigidbody; }
        public NavMeshAgent GetNavMeshAgent() { return navMeshAgent; }

        public float GetHealth() { return health; }
        public void SetHealth(float newHealth) { health = newHealth; }
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
        public void SetIsAttacking(bool isAttacking) { this.isAttacking = isAttacking; }

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

        public IdleState GetIdleState() { return idleState; }
        public PatrolState GetPatrolState() { return patrolState; }
        public ChaseState GetChaseState() { return chaseState; }
        public AttackState GetAttackState() { return attackState; }

        void Awake() {
            player = FindObjectOfType<PlayerController>();
            rigidbody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            idleState = new IdleState(this, navMeshAgent, player);
            patrolState = new PatrolState(this, navMeshAgent, player);
            chaseState = new ChaseState(this, navMeshAgent, player);
            attackState = new AttackState(this, navMeshAgent, player);

            currentState = idleState;
            currentState.EnterState();

            currentPatrolPoint = patrolPoints[patrolIndex];
        }

        void Update() {
            currentState.UpdateState();
        }

        // Exit the current state, and enter the new state.
        public void TransitionToState(BaseEnemyState newState) {
            currentState.ExitState();
            currentState = newState;
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
    }
}