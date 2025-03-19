// Author : Peiyu Wang @ Daphatus
// 19 03 2025 03 52

using PathCreation;
using Platforms;
using UnityEngine;

namespace Hazard.Train
{
    public class Train : Hazard
    {
        [SerializeField] private float speed = 20f;
        private PathCreator _pathCreator;
        
        private float _dstTravelled;
        private bool _end;
        
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            
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