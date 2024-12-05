using System;
using System.Collections;
using Tempo;
using UnityEngine;
using Utilities.ServiceLocator;

namespace Platforms
{public class MoveablePlatform : PlatformComponent
    { 
     
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
        
        /**
         * Time for the platform to move.
         */
        private float _time = 0;
        private bool _reverse = false;
        public void Awake()
        { 
            _initialPosition = transform.position;
            CalculateEndPosition();
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }

        public void Update()
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
        }
    
        /**
         * Affect the platform with the tape type.
         */
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            if(tapeType == TapeType.Slow)
            {
                _speed = effectValue;
                StartCoroutine(AffectTimer(duration));
            }
        }
        /**
         * Affect the platform with the tape type.
         */
        private IEnumerator AffectTimer(float duration)
        {
            yield return new WaitForSeconds(duration);
            _speed = 1;
        }
    }
}