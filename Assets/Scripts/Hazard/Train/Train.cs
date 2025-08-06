// Author : Peiyu Wang @ Daphatus
// 19 03 2025 03 52

// Updated by Ellen Lyu

using System;
using System.Collections;
using PathCreation;
using Platforms;
using UnityEngine;
using Utilities.ServiceLocator;
using Tempo;

namespace Hazard
{
    public class Train : Hazard 
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private float normalSpeed = 20f;
        private PathCreator _pathCreator;
        
        private float _dstTravelled;
        private bool _end;

        private void OnEnable()
        {
            if(ServiceLocator.Instance == null) return;
            ServiceLocator.Instance.Register<ISyncable>(this);
        }
        
        private void OnDisable()
        {
            if(ServiceLocator.Instance == null) return;
            ServiceLocator.Instance.Unregister<ISyncable>(this);
        }

        public override void Affect(TempoMode mode)
        {
            switch (mode)
            {
                case TempoMode.Slow:
                    speed = normalSpeed * _slowEffectValue;
                    break;
                case TempoMode.Fast:
                    speed = normalSpeed * _fastEffectValue;
                    break;
                default:
                    speed = normalSpeed;
                    break;
            }
        }
        
        //Move along the path
        private void Update()
        {
            if (_pathCreator)
            {
                _dstTravelled += speed * Time.deltaTime;
                transform.position = _pathCreator.path.GetPointAtDistance(_dstTravelled);
                transform.rotation = _pathCreator.path.GetRotationAtDistance(_dstTravelled);
                if (_dstTravelled >= _pathCreator.path.length)
                {
                    if (!_end)
                    {
                        _end = true;
                        OnReachEnd();
                    }
                }
            }
            
        }
        
        public void SetPath(PathCreator p)
        {
            _pathCreator = p;
        }
        
        private void OnReachEnd()
        {
            Destroy(gameObject);
        }
    }
}