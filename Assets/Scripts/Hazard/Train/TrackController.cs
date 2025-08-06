// Author : Peiyu Wang @ Daphatus
// 19 03 2025 03 33

// Updated by Ellen Lyu

using System.Collections;
using UnityEngine;
using PathCreation;
using Utilities.ServiceLocator;
using Tempo;

namespace Hazard
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
            ServiceLocator.Instance.Register<ISyncable>(this);
        }
        
        private void OnDisable()
        {
            if(ServiceLocator.Instance == null) return;
            ServiceLocator.Instance.Unregister<ISyncable>(this);
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

        private void SpawnTrain(TempoMode mode)
        {
            if (!trainPrefab)
            {
                throw new System.Exception("Train prefab is null");
            }
            var o = Instantiate(trainPrefab, pathCreator.path.GetPoint(0), pathCreator.path.GetRotationAtDistance(0));
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
            // force initial position and rotation immediately on spawn
            train.transform.position = pathCreator.path.GetPointAtDistance(0f);
            train.transform.rotation = pathCreator.path.GetRotationAtDistance(0f);
            // set the initial speed based on the current tempo mode
            train.Affect(mode);
        }

        private IEnumerator TrainSpawner()
        {
            float timer = 0f;

            while (true)
            {
                timer += Time.deltaTime;

                if (timer >= _spawnInterval)
                {
                    SpawnTrain(GetCurrentTempoMode());
                    timer = 0f;
                }

                yield return null; // Wait until the next frame
            }
        }

        public override void Affect(TempoMode mode)
        {
            switch (mode)
            {
                case TempoMode.Slow:
                    _spawnInterval = _normalSpawnInterval * _slowEffectValue;
                    break;
                case TempoMode.Fast:
                    _spawnInterval = _normalSpawnInterval * _fastEffectValue;
                    break;
                case TempoMode.Default:
                    _spawnInterval = _normalSpawnInterval;
                    break;
            }
        }

        public TempoMode GetCurrentTempoMode()
        {
            if (Mathf.Approximately(_spawnInterval, _normalSpawnInterval * _slowEffectValue))
                return TempoMode.Slow;
            if (Mathf.Approximately(_spawnInterval, _normalSpawnInterval * _fastEffectValue))
                return TempoMode.Fast;
            return TempoMode.Default;
        }

        private void OnDestroy()
        {
            if (_spawnTrainCoroutine != null)
                StopCoroutine(_spawnTrainCoroutine);
        }
    }
}