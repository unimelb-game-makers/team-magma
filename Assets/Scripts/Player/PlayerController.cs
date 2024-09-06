using System;
using Damage;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Movement speed of the player
        [Header("Movement Setup")]
        [SerializeField] private float movementSpeed = 15f;
        [SerializeField] private OrientationType orientation;

        [Space(10)]

        [Header("Attack Setup")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private LayerMask enemyLayers;  // The layers that should be considered as enemies
        [SerializeField] private float rotationSpeed = 10f;  // Speed at which the player rotates
        [SerializeField] private GameObject MeleeAttackPrefab;
        [SerializeField] private float hitRange = 0.5f;
        [SerializeField] private float meleeAttackRecoverTime = 0.7f;

        [Space(10)]

        [Header("Dodge Setup")]
        [SerializeField] private float dodgeSpeed = 40f;
        [SerializeField] private float dodgeRecoverTime = 1f;
        [SerializeField] private float dodgeTime = 0.15f;
        [SerializeField] private float isInvulnerableTime = 0.1f;


        public enum OrientationType
        {
            TowardMouse,
            BasedOnInput
        }

        // Rigidbody component for physics-based movement
        private Rigidbody _rigidbody;

        private bool _leftMouseButtonDown;

        private bool _leftShiftButtonDown;

        private bool _DodgeButtonDown;

        private Camera _mainCamera;  // Reference to the main camera

        private MeleeAttackBox _meleeAttackBox = null;

        private float _previousMeleeAttack;

        private bool _isMovingHorizontally;
        private bool _isMovingVertically;
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

            _previousMeleeAttack = Time.time - meleeAttackRecoverTime;
            _previousDodge = Time.time - dodgeRecoverTime;
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");

            TrackMovementInput();

            Move();

            Rotate();

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
                Fire(GetMouseWorldPosition());
                _leftMouseButtonDown = true;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                _leftMouseButtonDown = false;
            }

            if (Input.GetButtonDown("Fire3") && !_leftShiftButtonDown)
            {
                MeleeAttack(GetMouseWorldPosition());
                Debug.LogError("Melee attack");
                _leftShiftButtonDown = true;
            }

            if (Input.GetButtonUp("Fire3"))
            {
                _leftShiftButtonDown = false;
            }
        }

        private void Move()
        {
            // Get input from the horizontal and vertical axes
            

            if (Input.GetButtonDown("Dodge") && !_DodgeButtonDown && Time.time > _previousDodge + dodgeRecoverTime)
            {
                _dodgeDirection = new Vector3(_horizontalInput, 0, _verticalInput);
                _previousDodge = Time.time;
                _DodgeButtonDown = true;

                if (!TrackMovementInput()) {
                    // Move toward the direction the player is facing
                    _dodgeDirection = transform.forward;
                }

                GetComponent<Damageable>().setIsInvulnerable(true);
            }

            if (Input.GetButtonUp("Dodge"))
            { 
                _DodgeButtonDown = false;
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
                //Slow down the speed by 1/sqrt(2) if both keys pressed
                if (_isMovingHorizontally && _isMovingVertically)
                {
                    movement = new Vector3(_horizontalInput * movementSpeed * 0.69f, _rigidbody.velocity.y, _verticalInput * movementSpeed * 0.69f);
                }
                else
                {
                    movement = new Vector3(_horizontalInput * movementSpeed, _rigidbody.velocity.y, _verticalInput * movementSpeed);
                }

                // Apply the movement to the Rigidbody
                //_rigidbody.MovePosition(_rigidbody.position + movement * Time.deltaTime);

                // Apply velocity to the Rigidbody
                _rigidbody.velocity = movement;
            }
        }
        

        private void Fire(Vector3 targetPosition)
        {
            //spawn a projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            //get the Projectile component from the projectile object
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            //check if the Projectile component exists
            if (projectileComponent != null)
            {
                var canAttackList = new List<string> { "Enemy" };
                projectileComponent.SendMessage("EditCanAttack", canAttackList);

                //set the initial direction of the projectile
                projectileComponent.SetInitialDirection((targetPosition - transform.position).normalized);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            // Create a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Create a plane at the player's position with normal facing up
            Plane plane = new Plane(Vector3.up, transform.position);
            // Calculate the distance from the ray origin to the plane
            if (plane.Raycast(ray, out float distance))
            {
                // Get the point on the plane where the ray intersects
                Vector3 hitPoint = ray.GetPoint(distance);
                return hitPoint;
            }
            return Vector3.zero;
        }

        private void MeleeAttack(Vector3 targetPosition)
        {
            Vector3 origin = transform.position;
            Vector3 forward = (transform.forward * hitRange) + origin;

            if (_meleeAttackBox == null && Time.time > _previousMeleeAttack + meleeAttackRecoverTime) {
                _previousMeleeAttack = Time.time;
                GameObject attackBox = Instantiate(MeleeAttackPrefab, forward, transform.rotation);
                _meleeAttackBox = attackBox.GetComponent<MeleeAttackBox>();
                _meleeAttackBox.transform.parent = gameObject.transform;
                if (_meleeAttackBox != null)
                {
                    var canAttackList = new List<string> { "Enemy" };
                    _meleeAttackBox.SendMessage("EditCanAttack", canAttackList);
                }
            }
        }

        void Rotate()
        {
            Quaternion targetRotation;
            switch (orientation)
            {
                case OrientationType.BasedOnInput:
                    if (TrackMovementInput()) {
                        targetRotation = Quaternion.LookRotation(new Vector3(_horizontalInput, 0, _verticalInput));
                        // Smoothly rotate towards the target rotation
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                    }
                    break;
                case OrientationType.TowardMouse:
                    // Get the mouse position in the world space
                    Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);  // Define a plane at the ground level

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
    }
}