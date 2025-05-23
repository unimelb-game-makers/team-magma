using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.EnemyStates;
using Enemies.EnemyTypes;
using Platforms;
using Player;
using Tempo;
using Timeline;
using UI;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyController : MonoBehaviour, ISyncable
    {
        private bool wasGamePaused = false;
        #region Enemy Variables
        private bool agentStatus;
        protected NavMeshAgent agent;

        [SerializeField] private float health = 100f;

        // Idle Variables
        [Header("Idle Variables")]
        [Tooltip("How long the enemy will idle before switching to patrol states.")]
        [SerializeField] private float idleDuration = 3f;
        
        // Patrol Variables
        [Header("Patrol Variables")]
        [Tooltip("Does the enemy have a preset patrol route (In the inspector)?")]
        [SerializeField] private bool presetPatrolRoute = false;
        [SerializeField] private float patrolSpeed = 5f;
        private Vector3 currentPatrolPoint;

        // Manually set the patrol points.
        [Tooltip("The preset patrol points.")]
        [SerializeField] private List<Transform> patrolPoints;
        private int patrolIndex = 0;
        
        // Chase Variables
        [Header("Chase Variables")]
        [SerializeField] private float chaseSpeed = 12f;
        [Tooltip("How long the enemy will chase the Player outside its sight range before stopping.")]
        [SerializeField] private float chaseDuration = 3f;
        [Tooltip("How often the enemy checks the Player's location - Lower means more resource-intensive and can cause lag.")]
        [SerializeField] private float locationCheckInterval = 0.1f;
        private float currentLocationCheckTime_Chase = 0f;
        private float currentLocationCheckTime_General = 0f;
        
        // Attack Variables
        [Header("Attack Variables")]
        [Tooltip("How much damage the enemy deals at default tempo.")]
        [SerializeField] protected float originalDamage;
        [Tooltip("The attack cooldown at default tempo.")]
        [SerializeField] protected float originalAttackCooldown;
        [Tooltip("How long the enemy will try to attack the Player outside its attack range before stopping.")]
        [SerializeField] protected float outsideAttackRangeDuration;
        private bool isAttacking;
        
        // Current Damage Variables for the current tempo.
        protected float damage;
        protected float attackCooldown;

        // Destination, Sight, Attack Ranges
        [Header("Destination Tolerance, Sight, Attack Ranges")]
        [Tooltip("How close to the patrol point the enemy should be to register as having arrived.")]
        [SerializeField] protected float destinationToleranceRange = 1.5f;
        [Tooltip("The aggro range.")]
        [SerializeField] protected float sightRange = 10f;
        [Tooltip("The attack range.")]
        [SerializeField] protected float attackRange = 2f;
        private bool enemyInDestinationRange, PlayerInSightRange, PlayerInAttackRange;

        // States
        [Header("States")]
        protected Dictionary<EnemyState, BaseEnemyState> _states;
        private BaseEnemyState _currentState;
        protected BaseEnemyState CurrentState => _currentState;
        #endregion

        #region Audio SFX
        [Header("Audio SFX")]
        [Tooltip("How loud the enemy sfx should be.")]
        [SerializeField] protected float sfxModifier = 0.2f;
        [Tooltip("Idling sound.")]
        [SerializeField] private FMODUnity.EventReference idleSoundReference;
        [Tooltip("Patrol sound.")]
        [SerializeField] private FMODUnity.EventReference patrolSoundReference;
        [Tooltip("Chase sound.")]
        [SerializeField] private FMODUnity.EventReference chaseSoundReference;
        [Tooltip("Attack sound.")]
        [SerializeField] private FMODUnity.EventReference attackSoundReference;
        private FMOD.Studio.EventInstance idleSound;
        private FMOD.Studio.EventInstance patrolSound;
        private FMOD.Studio.EventInstance chaseSound;
        private FMOD.Studio.EventInstance attackSound;
        #endregion
        
        #region Enemy State Getters and Setters
        [SerializeField] protected Animator animator;

        private PlayerController _Player;
        public PlayerController Player
        {
            get
            {
                if(!_Player)
                {
                    _Player = GameManager.Instance.PlayerController;
                }
                return _Player;
            }
        }
        public NavMeshAgent GetNavMeshAgent() { return agent; }
        
        // Health
        public float GetHealth() { return health; }

        public void SetHealth(float newHealth)
        {
            health = newHealth;
        }
        
        // State Variables
        public float GetIdleDuration() { return idleDuration; }
        public bool GetPresetPatrolPoints() {return presetPatrolRoute; }
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
        public bool PlayerIsInSightRange() { return PlayerInSightRange; }
        public bool PlayerIsInAttackRange() { return PlayerInAttackRange; }

        public FMOD.Studio.EventInstance GetIdleSound() {
            return idleSound;
        }
        public FMOD.Studio.EventInstance GetPatrolSound() {
            return patrolSound;
        }
        public FMOD.Studio.EventInstance GetChaseSound() {
            return chaseSound;
        }
        public FMOD.Studio.EventInstance GetAttackSound() {
            return attackSound;
        }
        
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
            agent = GetComponent<NavMeshAgent>();
            agent.speed = patrolSpeed;

            idleSound = FMODUnity.RuntimeManager.CreateInstance(idleSoundReference);
            patrolSound = FMODUnity.RuntimeManager.CreateInstance(patrolSoundReference);
            chaseSound = FMODUnity.RuntimeManager.CreateInstance(chaseSoundReference);
            attackSound = FMODUnity.RuntimeManager.CreateInstance(attackSoundReference);

            // timeline = FindObjectOfType<MusicTimeline>();
            // animator = GetComponent<Animator>();

            if (patrolPoints.Count > 0)
                currentPatrolPoint = patrolPoints[patrolIndex].position;
            else
                currentPatrolPoint = transform.position;
            DefaultTempo();
        }

        protected virtual void Start() {
            AddStates();
            _currentState = GetState(EnemyState.Idle);
            _currentState.EnterState();
        }
        
        protected virtual void AddStates()
        {
            _states = new Dictionary<EnemyState, BaseEnemyState>()
            {
                { EnemyState.Idle, new IdleState(this, agent, Player) },
                { EnemyState.Patrol, new PatrolState(this, agent, Player) },
                { EnemyState.Chase, new ChaseState(this, agent, Player) },
                { EnemyState.Attack, new AttackState(this, agent, Player) },
            };
        }
        
        public virtual void Update() {
            if (PauseManager.IsPaused && !wasGamePaused) {
                wasGamePaused = true;
                agentStatus = agent.isStopped;
                agent.isStopped = true;
                return;
            } else if (!PauseManager.IsPaused && wasGamePaused) {
                wasGamePaused = false;
                agent.isStopped = agentStatus;
            } else if (wasGamePaused) {
                return;
            }

            if (DefeatScreenManager.Instance.IsDefeat()) {
                agent.isStopped = true;
                return;
            }

            // No need to check update locations/sounds every frame.
            currentLocationCheckTime_General -= Time.deltaTime;
            if (currentLocationCheckTime_General < 0) {
                currentLocationCheckTime_General = locationCheckInterval;

                // Update ranges
                enemyInDestinationRange = Vector3.Distance(transform.position, currentPatrolPoint) <= destinationToleranceRange;
                if (Player != null)
                {
                    PlayerInSightRange = Vector3.Distance(transform.position, Player.transform.position) <= sightRange;
                    PlayerInAttackRange = Vector3.Distance(transform.position, Player.transform.position) <= attackRange;
                }

                // Update the locations where the sounds should be played.
                idleSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                patrolSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                chaseSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                attackSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            }

            _currentState?.UpdateState();
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
            // No need to check Player location every frame.
            currentLocationCheckTime_Chase -= Time.deltaTime;
            if (currentLocationCheckTime_Chase > 0) return;
            currentLocationCheckTime_Chase = locationCheckInterval;
            if (Player != null) {
                agent.SetDestination(Player.transform.position);
            }
        }

        public abstract void Attack();

        // Apply a knockback to the enemy
        public void ApplyKnockback(Vector3 direction, float distance)
        {
            // Apply knockback, then resume enemy AI after
            if (agent.isOnNavMesh) {
                agent.isStopped = true;
                StartCoroutine(KnockbackRoutine(direction, distance));
            }
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

        public virtual void Die() {
            _currentState?.ExitState();
            StopSFX();
            ReleaseSFX();
            Destroy(gameObject);
        }

        // Safeguard in case the enemy is destroyed without calling Die method.
        void OnDestroy()
        {
            _currentState?.ExitState();
            StopSFX();
            ReleaseSFX();
        }

        public virtual void SetAudioVolume(float masterVolume, float sfxVolume) {
            if (idleSound.isValid()) {
                idleSound.setVolume(sfxVolume * masterVolume * sfxModifier);
            }
            if (patrolSound.isValid()) {
                patrolSound.setVolume(sfxVolume * masterVolume * sfxModifier);
            }
            if (chaseSound.isValid()) {
                chaseSound.setVolume(sfxVolume * masterVolume * sfxModifier);
            }
            if (attackSound.isValid()) {
                attackSound.setVolume(sfxVolume * masterVolume * sfxModifier);
            }
        }

        public virtual void PauseAudio(bool pause) {
            idleSound.setPaused(pause);
            patrolSound.setPaused(pause);
            chaseSound.setPaused(pause);
            attackSound.setPaused(pause);
        }

        public virtual void StopSFX() {
            idleSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            patrolSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            chaseSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            attackSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        // Release the sounds so FMOD can discard them.
        protected virtual void ReleaseSFX() {
            idleSound.release();
            patrolSound.release();
            chaseSound.release();
            attackSound.release();
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
        public Animator GetAnimator(){
            return animator;
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