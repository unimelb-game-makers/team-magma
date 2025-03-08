using System;
using Damage;
using UnityEngine;
using System.Collections.Generic;
using Platforms; // Why is TapeType in platforms?
using Tempo;
using Utilities.ServiceLocator;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Movement speed of the player
        [Header("Movement Setup")]
        [SerializeField] private float movementSpeed = 15f;
        [SerializeField] private OrientationType orientation;
        [SerializeField] private float jumpHeight = 2.0f;
    	[SerializeField] private float jumpForce = 2.0f;
    
    	
        [SerializeField] private OrientationType lookOrientation;
        [SerializeField] private OrientationType moveOrientation;

        [Space(10)]

        [Header("Attack Setup")]
        [SerializeField] private LayerMask enemyLayers;  // The layers that should be considered as enemies
        [SerializeField] private float rotationSpeed = 10f;  // Speed at which the player rotates
        [SerializeField] private GameObject MeleeAttackPrefab;
        [SerializeField] private float weakAttackRange = 1f;
        [SerializeField] private float strongAttackRange = 2f;
        [SerializeField] private float weakMeleeAttackRecoverTime = 0.5f;
        [SerializeField] private float strongMeleeAttackRecoverTime = 0.3f;
        [SerializeField] private float weakAttackDamage = 10;
        [SerializeField] private float strongAttackDamage = 20;
        
        // To implement feat: Knockback

        [Space(10)]

        [Header("Dodge Setup")]
        [SerializeField] private float dodgeSpeed = 40f;
        [SerializeField] private float dodgeRecoverTime = 1f;
        [SerializeField] private float dodgeTime = 0.15f;
        [SerializeField] private float isInvulnerableTime = 0.1f;

        [Space(10)]

        [Header("Beat Setup")]
        [SerializeField] private BeatSpawner beatSpawner;

        public enum OrientationType
        {
            TowardMouse,
            BasedOnInput
        }

        // Rigidbody component for physics-based movement
        private Rigidbody _rigidbody;

        private bool _leftMouseButtonDown;
        private bool _DodgeButtonDown;
        private Camera _mainCamera;  // Reference to the main camera

        private MeleeAttackBox _meleeAttackBox = null;

        private float _previousMeleeAttack;

        private bool _isMovingHorizontally;
        private bool _isMovingVertically;
        private bool _isGrounded;

        private float _previousDodge;
        private Vector3 _dodgeDirection;

        private float _horizontalInput;
        private float _verticalInput;

        private void Start()
        {
            // Get the Rigidbody component attached to this GameObject
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                Debug.LogError("Rigidbody component is missing from the player object.");
            }
            // Get the main camera
            _mainCamera = Camera.main;

            _previousMeleeAttack = Time.time - weakMeleeAttackRecoverTime;
            _previousDodge = Time.time - dodgeRecoverTime;
        }

        private void Update()
        {
            if (PauseManager.IsPaused) return;
            
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");

            TrackMovementInput();

            Rotate();
            
            Move();

            Attack();
        }

        private void FixedUpdate()
        {
            //Move();

            //Rotate();

            //Attack();
        }

        private void Attack()
        {
            //if the player has attacked need to release the mouse to attack again
            if (Input.GetButtonDown("Fire1") && !_leftMouseButtonDown)
            {
                // Check if the attack was on beat here
                if (beatSpawner.AnyBeatsHittable()) {
                    // Strong melee attack
                    Debug.Log("Strong Melee attack");
                    MeleeAttack(strongAttackRange, strongMeleeAttackRecoverTime, strongAttackDamage);
                } else {
                    // Weak melee attack
                    Debug.Log("Weak Melee attack");
                    MeleeAttack(weakAttackRange, weakMeleeAttackRecoverTime, weakAttackDamage);
                }

                // The current fire method should be replaced by melee attack (i.e the player
                // cannot shoot projectiles, only melee attack)
                
                _leftMouseButtonDown = true;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                _leftMouseButtonDown = false;
            }
            
            if (Input.GetButtonDown("Fire2"))
            {
                // Reconfiging this to use Selection Wheel
                //PlayTapeEffect();
            }
        }

        private void Move()
        {
            // Get input from the horizontal and vertical axes
            

            if (Input.GetButtonDown("Dodge") && !_DodgeButtonDown && Time.time > _previousDodge + dodgeRecoverTime)
            {
                _dodgeDirection = transform.forward; 
                _previousDodge = Time.time;
                _DodgeButtonDown = true;

                if (!TrackMovementInput()) {
                    // Move toward the direction the player is facing
                    _dodgeDirection = transform.forward;
                }

                GetComponent<Damage.Damageable>().setIsInvulnerable(true);
            }

            if (Input.GetButtonUp("Dodge"))
            { 
                _DodgeButtonDown = false;
            }

            if(Input.GetButtonDown("Jump") && _isGrounded){
    
    			_rigidbody.AddForce(new Vector3(0.0f, jumpHeight, 0.0f) * jumpForce, ForceMode.Impulse);
    			_isGrounded = false;
    		}

            if (Time.time > _previousDodge + isInvulnerableTime && GetComponent<Damageable>().getIsInvulnerable()) {
                GetComponent<Damageable>().setIsInvulnerable(false);
            }

            if (Time.time > _previousDodge && Time.time < _previousDodge + dodgeTime)
            // If boost of movement is needed (for dodge)
            {
                // Apply the movement to the Rigidbody
                _rigidbody.MovePosition(_rigidbody.position + _dodgeDirection * dodgeSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 movement;
                // Calculate the movement vector
                // Slow down the speed by 1/sqrt(2) if both keys pressed
                if (_isMovingHorizontally && _isMovingVertically)
                {
                    movement = new Vector3(_horizontalInput * movementSpeed * 0.69f, _rigidbody.velocity.y, _verticalInput * movementSpeed * 0.69f);
                }
                else
                {
                    movement = new Vector3(_horizontalInput * movementSpeed, _rigidbody.velocity.y, _verticalInput * movementSpeed);
                }

                switch (moveOrientation)
                {
                    case OrientationType.BasedOnInput:
                        //if (TrackMovementInput()) {
                            // Rotate the movement vector direction based on camera angle
                            movement = Quaternion.AngleAxis(_mainCamera.transform.rotation.eulerAngles.y, Vector3.up) * movement;
                        //}
                        break;

                    // Movement besides foward is a bit weird but it works.
                    case OrientationType.TowardMouse: 
                        // Get the mouse position in the world space
                        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                        Plane groundPlane = new Plane(Vector3.up, Vector3.down);  // Define a plane at the ground level

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
                _rigidbody.velocity = movement;
            }
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

        private void MeleeAttack(float attackRange, float attackRecoverTime, float attackDamage)
        {
            Vector3 origin = transform.position;
            Vector3 forward = (transform.forward * weakAttackRange) + origin;

            // Check if the player has attacked recently
            if (_meleeAttackBox == null && Time.time > _previousMeleeAttack + attackRecoverTime) {
                // update timer
                _previousMeleeAttack = Time.time;
                
                //spawn damage area and adjust its size
                GameObject attackBox = Instantiate(MeleeAttackPrefab, forward, transform.rotation);
                attackBox.transform.localScale = new Vector3(attackRange, 1, attackRange);
                
                // the meleeAttackBox will move together with the player
                _meleeAttackBox = attackBox.GetComponent<MeleeAttackBox>();
                _meleeAttackBox.transform.parent = gameObject.transform;
                
                // set the attack damage
                attackBox.GetComponent<MeleeDamager>().Damage = attackDamage;

                // the meleeAttackBox can damage enemies
                if (_meleeAttackBox != null)
                {
                    var canAttackList = new List<string> { "Enemy" };
                    _meleeAttackBox.SendMessage("EditCanAttack", canAttackList);
                }
            }
        }

        
        /**
         * Placeholder for playing the tape effect
         */
        private void PlayTapeEffect()
        {
            //get IAffectServices from service locator
            var affectServices = ServiceLocator.Instance.Get<ISyncable>();
            foreach (var o in affectServices)
            {
                o.Affect(TapeType.Slow, 5, 0.5f); // Why is TapeType in Platforms namespace?
            }
        }

        void Rotate()
        {
            Quaternion targetRotation;
            switch (lookOrientation)
            {
                case OrientationType.BasedOnInput:
                    if (TrackMovementInput()) {
                        targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_mainCamera.transform.rotation.eulerAngles.y, Vector3.up) * new Vector3(_horizontalInput, 0, _verticalInput));
                        // Smoothly rotate towards the target rotation
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                    }
                    break;
                case OrientationType.TowardMouse:
                    // Get the mouse position in the world space
                    Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                    Plane groundPlane = new Plane(Vector3.up, Vector3.down);  // Define a plane at the ground level

                    // Check where the ray intersects the plane
                    if (groundPlane.Raycast(ray, out float distance))
                    {
                        Vector3 targetPoint = ray.GetPoint(distance);  // Get the point on the plane
                        Vector3 direction = (targetPoint - transform.position).normalized;  // Calculate the direction

                        targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                        // Smoothly rotate towards the target rotation
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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

        void OnCollisionStay()
    	{
    		_isGrounded = true;
    	}
    }
}