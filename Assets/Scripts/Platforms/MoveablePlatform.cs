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
        [SerializeField] private float _waitTime = 1f;
        
        [Header("Editor Visualization")]
        [SerializeField] private Color _pathColor = Color.cyan;
        [SerializeField] private float _waypointSize = 0.2f;
        [SerializeField] private bool _showPathInGame = false;

        private Vector3 _initialPosition;
        private Vector3 _endPosition;
        private Vector3 previousPosition;
        private float _originalSpeed;
        private List<Transform> passengers = new List<Transform>();
        private enum PlatformState { MovingToEnd, MovingToStart, Waiting }
        private PlatformState _currentState = PlatformState.MovingToEnd;
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
            _endPosition = _initialPosition + _direction.normalized * _displacement;
        }

        private void MovePlatform()
        {
            previousPosition = transform.position;

            if (_currentState == PlatformState.Waiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    // Determine next movement direction after waiting
                    if (Vector3.Distance(transform.position, _initialPosition) < 0.01f)
                    {
                        _currentState = PlatformState.MovingToEnd;
                    }
                    else
                    {
                        _currentState = PlatformState.MovingToStart;
                    }
                }
                return;
            }

            Vector3 direction = (_endPosition - _initialPosition).normalized;
            float distanceToTravel = _speed * Time.deltaTime;

            if (_currentState == PlatformState.MovingToEnd)
            {
                transform.position += direction * distanceToTravel;
                
                if (Vector3.Distance(transform.position, _endPosition) <= distanceToTravel)
                {
                    transform.position = _endPosition;
                    StartWaiting();
                }
            }
            else if (_currentState == PlatformState.MovingToStart)
            {
                transform.position -= direction * distanceToTravel;
                
                if (Vector3.Distance(transform.position, _initialPosition) <= distanceToTravel)
                {
                    transform.position = _initialPosition;
                    StartWaiting();
                }
            }

            // Move passengers
            Vector3 delta = transform.position - previousPosition;
            passengers.RemoveAll(passenger => passenger == null);
            
            foreach (Transform passenger in passengers)
            {
                passenger.position += delta;
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
                _endPosition = _initialPosition + _direction.normalized * _displacement;
            }

            if (_showPathInGame || !Application.isPlaying)
            {
                Gizmos.color = _pathColor;
                Gizmos.DrawLine(_initialPosition, _endPosition);
                Gizmos.DrawSphere(_initialPosition, _waypointSize);
                Gizmos.DrawSphere(_endPosition, _waypointSize);
                
                if ((_endPosition - _initialPosition).sqrMagnitude > 0.01f)
                {
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
        }
#endif
    }
}