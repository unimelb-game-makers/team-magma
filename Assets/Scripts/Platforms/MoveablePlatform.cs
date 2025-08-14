using System.Collections;
using Tempo;
using UnityEngine;
using Utilities.ServiceLocator;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Platforms
{
    public class MoveablePlatform : PlatformComponent
    {
        [Header("Movement Settings")]
        [SerializeField] private float slow_multiplier = 0.5f;
        [SerializeField] private float fast_multiplier = 1.5f;
        [SerializeField] private float _displacement = 1;
        [SerializeField] private float _speed = 5;
        [SerializeField] private Vector3 _direction = Vector3.right;
        [SerializeField] private float _waitTime = 1f; // Time to wait at each end
        
        [Header("Editor Visualization")]
        [SerializeField] private Color _pathColor = Color.cyan;
        [SerializeField] private float _waypointSize = 0.2f;
        [SerializeField] private bool _showPathInGame = false;

        private Vector3 _initialPosition;
        private Vector3 _endPosition;
        private Vector3 previousPosition;
        private bool _reverse = false;
        private float _originalSpeed;
        private List<Transform> passengers = new List<Transform>();
        private enum PlatformState { Moving, Waiting }
        private PlatformState _currentState = PlatformState.Moving;
        private float _waitTimer = 0f;

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

        private void CalculateEndPosition()
        {
            _endPosition = _initialPosition + _direction * _displacement;
        }

        private void MovePlatform()
        {
            previousPosition = transform.position;

            if (_currentState == PlatformState.Waiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    _currentState = PlatformState.Moving;
                }
                return;
            }

            // Calculate direction
            Vector3 direction = (_endPosition - _initialPosition).normalized;

            // Move platform at constant speed
            if (_reverse)
            {
                transform.position -= direction * _speed * Time.deltaTime;
                
                // Check if we've reached the initial position
                if (Vector3.Distance(transform.position, _initialPosition) < 0.01f)
                {
                    transform.position = _initialPosition; // Snap to exact position
                    _reverse = false;
                    StartWaiting();
                }
            }
            else
            {
                transform.position += direction * _speed * Time.deltaTime;
                
                // Check if we've reached the end position
                if (Vector3.Distance(transform.position, _endPosition) < 0.01f)
                {
                    transform.position = _endPosition; // Snap to exact position
                    _reverse = true;
                    StartWaiting();
                }
            }

            // Move passengers
            Vector3 delta = transform.position - previousPosition;
            passengers.RemoveAll(passenger => passenger == null);
            
            foreach (Transform passenger in passengers)
            {
                Quaternion rotation = passenger.rotation;
                passenger.position += delta;
                passenger.rotation = rotation;
            }
        }

        private void StartWaiting()
        {
            _currentState = PlatformState.Waiting;
            _waitTimer = _waitTime;
        }

        public override void Affect(TempoMode mode)
        {
            switch (mode)
            {
                case TempoMode.Slow: _speed = _originalSpeed * slow_multiplier; break;
                case TempoMode.Fast: _speed = _originalSpeed * fast_multiplier; break;
                case TempoMode.Default: _speed = _originalSpeed; break;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                passengers.Add(collision.transform);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                passengers.Remove(collision.transform);
        }

        private void OnDisable() => passengers.Clear();
        private void OnDestroy() => passengers.Clear();

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                _initialPosition = transform.position;
                _endPosition = _initialPosition + _direction * _displacement;
            }

            if (_showPathInGame || !Application.isPlaying)
            {
                Gizmos.color = _pathColor;
                Gizmos.DrawLine(_initialPosition, _endPosition);
                Gizmos.DrawSphere(_initialPosition, _waypointSize);
                Gizmos.DrawSphere(_endPosition, _waypointSize);
                
                // Draw direction arrow
                Vector3 dir = (_endPosition - _initialPosition).normalized;
                float arrowSize = Mathf.Min(_waypointSize * 2, _displacement * 0.3f);
                Handles.color = _pathColor;
                Handles.ArrowHandleCap(0, 
                    _initialPosition + dir * (_displacement * 0.5f), 
                    Quaternion.LookRotation(dir), 
                    arrowSize, 
                    EventType.Repaint);
            }
        }
#endif
    }
}