using System;
using Damage;
using UnityEngine;
using System.Collections.Generic;
using Platforms; // Why is TapeType in platforms?
using Tempo;
using Timeline;
using Utilities.ServiceLocator;
using UI;
using Player.Stats;

namespace Player
{
    public enum PlayerState
    {
        Normal,
        Attacking,
    }
    
    public enum PlayerAttack
    {
        Weak,
        Strong,
    }
    
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Speed = Animator.StringToHash("speed");

        // Movement speed of the player
        [Header("Movement Setup")]
        [SerializeField] private float movementSpeed = 15f;
        [SerializeField] private Animator animator;
        static readonly int SpeedID = Animator.StringToHash("speed");
        public Animator Animator => animator;
        public float MovementSpeed
        {
            get => movementSpeed;
            set => movementSpeed = value;
        }
        [SerializeField] private float jumpHeight = 2.0f;
    	[SerializeField] private float jumpForce = 2.0f;
        [SerializeField] private int maxAirJumps = 1;
        private int airJumpsRemaining = 0;
    
    	
        [SerializeField] private OrientationType lookOrientation;
        [SerializeField] private OrientationType moveOrientation;

        [Space(10)]

        [Header("Attack Setup")]
        [SerializeField] private LayerMask enemyLayers;  // The layers that should be considered as enemies
        [SerializeField] private float rotationSpeed = 10f;  // Speed at which the player rotates
        [SerializeField] public GameObject MeleeAttackPrefab;
        [SerializeField] public float weakAttackRange = 1f;
        [SerializeField] public float strongAttackRange = 2f;
        [SerializeField] public float weakMeleeAttackRecoverTime = 0.5f;
        [SerializeField] public float strongMeleeAttackRecoverTime = 0.3f;
        [SerializeField] public float weakAttackDamage = 10;
        [SerializeField] public float strongAttackDamage = 20;
        [SerializeField] public float attackForwardOffset = 1f;
        
        // To implement feat: Knockback

        [Space(10)]

        [Header("Dodge Setup")]
        [SerializeField] private float dodgeRecoverTime = 1f;
        [SerializeField] private float dodgeTime = 0.15f;
        [SerializeField] private float isInvulnerableTime = 0.1f;
        [SerializeField] private float dodgeForce = 20f; // New parameter for dodge force

        [Space(10)]

        [Header("Ground Setup")]

        [SerializeField] private float groundStickDistance = 0.5f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float maxAirStickTime = 0.3f;

        [Space(10)]
        
        private BeatSpawner beatSpawner;
        private PlayerState _state = PlayerState.Normal;

        public enum OrientationType
        {
            TowardMouse,
            BasedOnInput
        }

        // Rigidbody component for physics-based movement
        private Rigidbody _rigidbody;

        private bool _DodgeButtonDown;
        private Camera _mainCamera;

        private Camera Camera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                    if (_mainCamera == null)
                    {
                        throw new Exception("No Camera found in the scene.");
                    }
                }
                return _mainCamera;
            }
        }

        private float _attackTime;
        private float _attackAnimTimer;

        private bool _isMovingHorizontally;
        private bool _isMovingVertically;
        private bool _isGrounded = true;

        private float _previousDodge;
        private Vector3 _dodgeDirection;

        private float _horizontalInput;
        private float _verticalInput;

        private bool _isDodging;

        private bool _canControl = true;
        private static readonly int StrongAttackAnim = Animator.StringToHash("StrongAttack");
        private static readonly int WeakAttackAnim = Animator.StringToHash("WeakAttack");
        private TempoMode _mode = TempoMode.Default;
        private static readonly int JumpAnim = Animator.StringToHash("Jump");
        private static readonly int InAirAnim = Animator.StringToHash("inAir");
        private float groundedCooldown = 1f;
        private float groundedCooldownTimer = 0f;
        private float timeSinceLeftGround = 0f;

        private void Awake()
        {
            MusicTimeline.OnTempoChanged += OnTempoChanged;
        }

        private void Start()
        {
            // Get the Rigidbody component attached to this GameObject
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError("Rigidbody component is missing from the player object.");
            }

            _previousDodge = Time.time - dodgeRecoverTime;
        }

        private void Update()
        {
            if (PauseManager.IsPaused || DefeatScreenManager.Instance.IsDefeat() || SuccessScreenManager.Instance.IsSuccess() || !_canControl) return;

            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");

            switch (_state)
            {
                case PlayerState.Normal:
                    TrackMovementInput();
                    Rotate();
                    Move();
                    TrackAttackInput();
                    break;
                case PlayerState.Attacking:
                    TrackMovementInput();
                    Move();

                    AttackUpdate();
                    break;
            }

            if (transform.position.y < -500)
            {
                GetComponent<PlayerHealth>().Death();
            }
        }

        private void OnTempoChanged(TempoMode mode)
        {
            _mode = mode;
            Animator.speed = TempoSetting.GetPlayerMovementMultiplier(mode);
        }

        private void TrackAttackInput()
        {
            if (PlayerStateManager.Instance == null || !PlayerStateManager.Instance.IsCombat()) return;
            if (Input.GetButtonDown("Fire1"))
            {
                BeatResult result = MusicTimeline.instance.ProcessAction();
                switch (result.grade)
                {
                    case Grade.Perfect:
                        Attack(PlayerAttack.Strong);
                        break;
                    case Grade.Good:
                        Attack(PlayerAttack.Weak);
                        break;
                    case Grade.Failed:
                        break;
                }
            }
        }
        
        private void Attack(PlayerAttack attack)
        {
            SetAttackState();
            animator.SetTrigger(attack == PlayerAttack.Strong ? StrongAttackAnim : WeakAttackAnim);
            _attackTime = attack == PlayerAttack.Strong ? strongMeleeAttackRecoverTime : weakMeleeAttackRecoverTime * TempoSetting.GetPlayerMovementMultiplier(_mode);
            _attackAnimTimer = 0f;
        }

        private void SetAttackState()
        {
            _state = PlayerState.Attacking;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void AttackUpdate()
        {
            _attackAnimTimer += Time.deltaTime;
            if (_attackAnimTimer >= _attackTime)
            {
                _state = PlayerState.Normal;
            }
        }

        private void Move()
        {
            // avoid immediate sticking on jump
            if (groundedCooldownTimer > 0f)
            {
                groundedCooldownTimer -= Time.fixedDeltaTime;
                if (groundedCooldownTimer <= 0f)
                {
                    _isGrounded = false;
                }
            }
            else if (!_isGrounded)
            {
                // Increase air time counter
                timeSinceLeftGround += Time.fixedDeltaTime;

                // Only stick if the player has been off the ground for less than maxAirStickTime (so that the jumping & droping won't count)
                if (timeSinceLeftGround < maxAirStickTime)
                {
                    StickToGround();
                }
                else
                {
                    // Too long in air — no stick; keep _isGrounded false
                    _isGrounded = false;
                }
            }
            else
            {
                // Player is grounded — reset air timer
                timeSinceLeftGround = 0f;
            }

            if (Input.GetButtonDown("Dodge") && !_DodgeButtonDown && Time.time > _previousDodge + dodgeRecoverTime && !_isDodging)
            {
                _dodgeDirection = transform.forward;
                _previousDodge = Time.time;
                _DodgeButtonDown = true;
                _isDodging = true;

                if (!TrackMovementInput())
                {
                    // Move toward the direction the player is facing
                    _dodgeDirection = transform.forward;
                }

                GetComponent<Damage.Damageable>().setIsInvulnerable(true);

                // Apply dodge force instead of using MovePosition
                _rigidbody.AddForce(_dodgeDirection * dodgeForce, ForceMode.Impulse);
            }

            if (Input.GetButtonUp("Dodge"))
            { 
                _DodgeButtonDown = false;
            }

            if (Input.GetButtonDown("Jump")) {
                if (_isGrounded) {
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z); // Reset Y
                    _rigidbody.AddForce(new Vector3(0.0f, jumpHeight, 0.0f) * jumpForce, ForceMode.Impulse);
                    airJumpsRemaining = maxAirJumps; // Reset air jumps on ground
                    groundedCooldownTimer = groundedCooldown;
                    animator.SetTrigger(JumpAnim);

                } else if (airJumpsRemaining > 0) {
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z); // Reset Y
                    _rigidbody.AddForce(new Vector3(0.0f, jumpHeight, 0.0f) * jumpForce, ForceMode.Impulse);
                    airJumpsRemaining--;
                    animator.SetTrigger(JumpAnim);

                }
            }

            if (Time.time > _previousDodge + isInvulnerableTime && GetComponent<Damageable>().getIsInvulnerable()) {
                GetComponent<Damageable>().setIsInvulnerable(false);
            }

            if (Time.time > _previousDodge + dodgeTime) {
                _isDodging = false;
            }

            if (!_isDodging) {
                Vector3 movement;
                // Calculate the movement vector
                // Slow down the speed by 1/sqrt(2) if both keys pressed
                if (_isMovingHorizontally && _isMovingVertically)
                {
                    movement = new Vector3(_horizontalInput * MovementSpeed * 0.69f, _rigidbody.velocity.y, _verticalInput * MovementSpeed * 0.69f);
                }
                else
                {
                    movement = new Vector3(_horizontalInput * MovementSpeed, _rigidbody.velocity.y, _verticalInput * MovementSpeed);
                }

                switch (moveOrientation)
                {
                    case OrientationType.BasedOnInput:
                        //if (TrackMovementInput()) {
                            // Rotate the movement vector direction based on camera angle
                            movement = Quaternion.AngleAxis(Camera.transform.rotation.eulerAngles.y, Vector3.up) * movement;
                        //}
                        break;

                    // Movement besides foward is a bit weird but it works.
                    case OrientationType.TowardMouse: 
                        // Get the mouse position in the world space
                        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);  // Define a plane at the ground level

                        // Check where the ray intersects the plane
                        if (groundPlane.Raycast(ray, out float distance))
                        {
                            Vector3 targetPoint = ray.GetPoint(distance);  // Get the point on the plane
                            Vector3 direction = (targetPoint - transform.position).normalized;  // Calculate the direction

                            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                            // Rotate the movement vector direction based on direction to mouse
                            movement = Quaternion.AngleAxis(targetRotation.eulerAngles.y, Vector3.up) * movement;
                        }
                        break;
                }

                // Apply the movement to the Rigidbody
                //_rigidbody.MovePosition(_rigidbody.position + movement * Time.deltaTime);

                // Apply velocity to the Rigidbody
                Vector3 finalMovement = movement * TempoSetting.GetPlayerMovementMultiplier(_mode);
                _rigidbody.velocity = new Vector3(finalMovement.x, _rigidbody.velocity.y, finalMovement.z);
                // set animation speed
                Animator.SetFloat(SpeedID, finalMovement.magnitude / MovementSpeed);
            }
        }

        public void SetControlEnabled(bool enabled)
        {
            _canControl = enabled;
        }

        private Vector3 GetMouseWorldPosition()
        {
            // Create a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Create a plane at the player's position with normal facing up
            Plane plane = new Plane(Vector3.up, transform.position + Vector3.down);
            // Calculate the distance from the ray origin to the plane
            if (plane.Raycast(ray, out float distance))
            {
                // Get the point on the plane where the ray intersects
                Vector3 hitPoint = ray.GetPoint(distance);
                // Adjust y to be level with player so not vertically angled 
                Vector3 directionPoint = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
                return directionPoint;
            }
            return Vector3.zero;
        }

        void Rotate()
        {
            Quaternion targetRotation;
            switch (lookOrientation)
            {
                case OrientationType.BasedOnInput:
                    Vector3 playerDirection = _rigidbody.velocity.normalized;
                    playerDirection.y = 0; // Don't rotate along the y-axis
                    
                    if (TrackMovementInput() && playerDirection != Vector3.zero)
                    {
                        targetRotation = Quaternion.LookRotation(playerDirection);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                    }
                    break;
                case OrientationType.TowardMouse:
                    // Cast a ray from the camera through the mouse position
                    Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                    
                    // Define a flat ground plane at y = transform.position.y (same height as player)
                    Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

                    // Check intersection with the plane
                    if (groundPlane.Raycast(ray, out float distance))
                    {
                        Vector3 hitPoint = ray.GetPoint(distance); // Where the mouse points on the plane
                        Vector3 direction = hitPoint - transform.position;
                        direction.y = 0f; // Ignore vertical difference

                        if (direction.sqrMagnitude > 0.001f)
                        {
                            targetRotation = Quaternion.LookRotation(direction);
                            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                        }
                    }
                    break;
            }
        }


        private bool TrackMovementInput()
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                _isMovingHorizontally = true;
            }
            if (Input.GetButtonDown("Vertical"))
            {
                _isMovingVertically = true; 
            }
            if (Input.GetButtonUp("Horizontal"))
            {
                _isMovingHorizontally = false;
            }
            if (Input.GetButtonUp("Vertical"))
            {
                _isMovingVertically = false; 
            }
            return _isMovingHorizontally || _isMovingVertically; 
        }

        void OnCollisionStay(Collision collision)
    	{
    		foreach (ContactPoint contact in collision.contacts)
            {
                // Check if the normal of the contact is pointing mostly upwards
                if (Vector3.Angle(contact.normal, Vector3.up) < 45f)
                {
                    _isGrounded = true;
                    Animator.SetBool(InAirAnim, false);

                    return;
                }
            }

            _isGrounded = false; // In case all contacts are walls/ceilings
            Animator.SetBool(InAirAnim, !_isGrounded);
    	}

        void OnCollisionExit(Collision collision)
        {
            _isGrounded = false;
            Animator.SetBool(InAirAnim, true);
        }

        void StickToGround()
        {
            BoxCollider col = GetComponent<BoxCollider>();
            // Calculate a ray starting just above the bottom of the collider
            Vector3 rayOrigin = transform.position + col.center - Vector3.up * (col.size.y / 2 - 0.05f);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundStickDistance, groundLayer))
            {
                // If the player is only a tiny bit above ground, snap down
                if (hit.distance <= groundStickDistance - 0.05f)
                {
                    _rigidbody.position = new Vector3(_rigidbody.position.x, _rigidbody.position.y - hit.distance + 0.05f, _rigidbody.position.z);
                    _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
                    _isGrounded = true;
                    Animator.SetBool(InAirAnim, false);
                }
            }
        }

        private void OnDestroy()
        {
            MusicTimeline.OnTempoChanged -= OnTempoChanged;
        }
        
    }
}