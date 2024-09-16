using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyController : MonoBehaviour
    {
        protected PlayerController player;
        protected NavMeshAgent navMeshAgent;
        protected Animator animator;

        [Header("Idle")]
        [SerializeField] private float idleDuration = 3f;
        [SerializeField] private AnimationClip idleAnimation;
        [SerializeField] private AudioClip idleAudio;
        
        [Header("Patrol")]
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private List<Transform> patrolPoints;
        private Transform currentPatrolPoint;
        private int patrolIndex = 0;
        [SerializeField] private AnimationClip patrolAnimation;
        [SerializeField] private AudioClip patrolAudio;

        [Header("Chase")]
        [SerializeField] private float chaseDuration = 10f;
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private float chaseRange = 5f;
        [SerializeField] private AnimationClip chaseAnimation;
        [SerializeField] private AudioClip chaseAudio;

        [Header("Attack")]
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] protected AnimationClip attackAnimation;
        [SerializeField] protected AnimationClip idleAttackAnimation;
        [SerializeField] protected AudioClip attackAudio;

        // These variables are used to turn the enemy so that it faces the direction it is moving in,
        // or so that it faces the player when in attack range.
        [Header("Other")]
        [SerializeField] private float rotationTime = 0.3f;
        private float yVelocity;

        [Header("States")]
        private IEnemyState currentState;
        private readonly IdleState idleState = new IdleState();
        private readonly PatrolState patrolState = new PatrolState();
        private readonly ChaseState chaseState = new ChaseState();
        private readonly AttackState attackState = new AttackState();

        public PlayerController GetPlayer() { return player; }
        public NavMeshAgent GetNavMeshAgent() { return navMeshAgent; }

        public float GetIdleDuration() { return idleDuration; }

        public float GetPatrolSpeed() { return patrolSpeed; }
        public Transform GetCurrentPatrolPoint() { return currentPatrolPoint; }

        public float GetChaseDuration() { return chaseDuration; }
        public float GetChaseRange() { return chaseRange; }
        public float GetChaseSpeed() { return chaseSpeed; }

        protected float GetAttackCooldown() { return attackCooldown; }
        public float GetAttackRange() { return attackRange; }

        public IdleState GetIdleState() { return idleState; }
        public PatrolState GetPatrolState() { return patrolState; }
        public ChaseState GetChaseState() { return chaseState; }
        public AttackState GetAttackState() { return attackState; }

        void Start()
        {
            player = FindObjectOfType<PlayerController>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            animator = GetComponent<Animator>();

            currentState = idleState;
            currentState.EnterState(this);

            currentPatrolPoint = patrolPoints[patrolIndex];
        }

        // Update is called once per frame
        void Update()
        {
            currentState.UpdateState(this);
        }

        public void TransitionToState(IEnemyState newState) {
            currentState = newState;
            newState.EnterState(this);
        }

        public void Idle() {
            animator.Play(idleAnimation.name);
        }

        public void Patrol() {
            RotateTowardsMovementDirection();
            animator.Play(patrolAnimation.name);
        }

        public void NextPatrolPoint() {
            patrolIndex++;
            if (patrolIndex >= patrolPoints.Count) {
                patrolIndex = 0;
            }
            currentPatrolPoint = patrolPoints[patrolIndex];
        }

        public void Chase() {
            navMeshAgent.destination = player.transform.position;
            RotateTowardsMovementDirection();
            animator.Play(chaseAnimation.name);
        }

        public abstract void Attack();

        // Rotate smoothly towards movement direction / towards player when in attack range.
        private void RotateTowardsMovementDirection() {
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