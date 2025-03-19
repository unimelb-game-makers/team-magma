// Author : Peiyu Wang @ Daphatus
// 19 03 2025 03 52

using System;
using System.Collections;
using PathCreation;
using Platforms;
using UnityEngine;
using Utilities.ServiceLocator;

namespace Hazard.Train
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
            ServiceLocator.Instance.Register(this);
        }
        
        private void OnDisable()
        {
            if(ServiceLocator.Instance == null) return;
            ServiceLocator.Instance.Unregister(this);
        }

        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            switch (tapeType)
            {
                case TapeType.Slow:
                    StartCoroutine(SlowTempo(duration, effectValue));
                    break;
                case TapeType.Fast:
                    StartCoroutine(FastTempo(duration, effectValue));
                    break;
                default:
                    speed = normalSpeed;
                    break;
            }
        }
        
        private IEnumerator SlowTempo(float duration, float effectValue)
        {
            speed *= effectValue;
            yield return new WaitForSeconds(duration);
            speed = normalSpeed;
        }
        
        private IEnumerator FastTempo(float duration, float effectValue)
        {
            speed *= effectValue;
            yield return new WaitForSeconds(duration);
            speed = normalSpeed;
        }
        
        //Move along the path
        public void Update()
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