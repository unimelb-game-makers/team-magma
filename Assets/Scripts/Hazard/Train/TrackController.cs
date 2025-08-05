// Author : Peiyu Wang @ Daphatus
// 19 03 2025 03 33

using System.Collections;
using UnityEngine;
using PathCreation;
using Utilities.ServiceLocator;

namespace Hazard.Train
{
    public class TrackController : Hazard
    {
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private GameObject trainPrefab;
        [SerializeField] private GameObject start;
        [SerializeField] private GameObject end;
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private float _normalSpawnInterval = 20f;
        private Coroutine _spawnTrainCoroutine;
        
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
        
        private void Start()
        {
            _spawnTrainCoroutine = StartCoroutine(TrainSpawner());
            SetTrackEnds();
        }

        private void SetTrackEnds()
        {
            start.transform.position = pathCreator.path.GetPoint(0);
            start.transform.rotation = pathCreator.path.GetRotationAtDistance(0);
            end.transform.position = pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1);
            end.transform.rotation = pathCreator.path.GetRotationAtDistance(pathCreator.path.length);
        }

        private void SpawnTrain()
        {
            if (!trainPrefab)
            {
                throw new System.Exception("Train prefab is null");
            }
            var o = Instantiate(trainPrefab,pathCreator.path.GetPoint(0) , pathCreator.path.GetRotationAtDistance(0));
            if (!o)
            {
                throw new System.Exception("cannot instantiate train");
            }
            var train = o.GetComponent<Train>();
            if (!train)
            {
                throw new System.Exception("Train component not found");
            }
            train.SetPath(pathCreator);
        }

        private IEnumerator TrainSpawner()
        {
            while (true)
            {
                SpawnTrain();
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        public override void Affect(TempoMode mode)
        {
            switch (mode)
            {
                case TempoMode.Slow:
                    _spawnInterval *= _normalSpawnInterval * _slowEffectValue;
                    break;
                case TempoMode.Fast:
                    _spawnInterval *= _normalSpawnInterval * _fastEffectValue;
                    break;
                case TempoMode.Default:
                    _spawnInterval = _normalSpawnInterval;
                    break;
            }
        }

        private void OnDestroy()
        {
            if (_spawnTrainCoroutine != null)
                StopCoroutine(_spawnTrainCoroutine);
        }
    }
}