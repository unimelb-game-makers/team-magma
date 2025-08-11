using System;
using System.Collections;
using Tempo;
using UnityEngine;
using Utilities.ServiceLocator;
using System.Collections.Generic;

namespace Platforms
{
    public class MoveablePlatform : PlatformComponent
    {
        [SerializeField] private float slow_speed = 0.5f;
        [SerializeField] private float fast_speed = 1.5f;
     
        [SerializeField] private float _displacement = 1;
        /**
         * Speed of the platform.
         */
        [SerializeField] private float _speed = 1;
        /**
         * Direction of the platform.
         */
        [SerializeField] private Vector3 _direction = Vector3.right;
        /**
         * initial position of the platform
         */
        private Vector3 _initialPosition;
        /**
         * End position of the platform
         */
        private Vector3 _endPosition;
        private Vector3 previousPosition;
        
        /**
         * Time for the platform to move.
         */
        private float _time = 0;
        private bool _reverse = false;
        private float _originalSpeed;
        private List<Transform> passengers = new List<Transform>();
        
        public void Awake()
        {
            _originalSpeed = _speed;
            _initialPosition = transform.position;
            previousPosition = transform.position;
            CalculateEndPosition();
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }

        public void FixedUpdate()
        {
            MovePlatform();
        }
        /**
         * Calculate the end position of the platform.
         */
        private void CalculateEndPosition()
        {
            _endPosition = _initialPosition + _direction * _displacement;
        }

        /**
         * Move the platform in the direction and speed.
         */
        private void MovePlatform()
        {
            // Store previous position
            previousPosition = transform.position;

            if (_time > 1)
            {
                _reverse = true;
            }
            else if (_time < 0)
            {
                _reverse = false;
            }
            if (_reverse)
            {
                _time -= Time.deltaTime * _speed;
            }
            else
            {
                _time += Time.deltaTime * _speed;
            }

            transform.position = Vector3.Lerp(_initialPosition, _endPosition, _time);
            
            // Move passengers
            Vector3 delta = transform.position - previousPosition;
            foreach (Transform passenger in passengers)
            {
                // Store current rotation
                Quaternion rotation = passenger.rotation;

                passenger.position += delta;
                
                // Restore rotation
                passenger.rotation = rotation;
            }
        }
    
        /**
         * Affect the platform with the tape type.
         */
        public override void Affect(TempoMode mode)
        {
            switch (mode)
            {
                // Slow down the platform
                case TempoMode.Slow:
                    _speed = slow_speed;
                    break;
                case TempoMode.Fast:
                    _speed = fast_speed;
                    break;
                // Reset the platform speed
                case TempoMode.Default:
                    _speed = _originalSpeed;
                    break;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                passengers.Add(collision.transform);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                passengers.Remove(collision.transform);
            }
        }
    }
}