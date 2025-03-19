// Author : Peiyu Wang @ Daphatus
// 19 03 2025 03 33

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Platforms;
using Tempo;

namespace Hazard.Train
{
    public class TrackController : MonoBehaviour, ISyncable
    {
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private GameObject trainPrefab;
        [SerializeField] private GameObject start;
        [SerializeField] private GameObject end;
        [SerializeField] private float _spawnInterval = 5f;
        private Coroutine _spawnTrainCoroutine;
        
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
            SpawnTrain();
            yield return new WaitForSeconds(_spawnInterval);
            _spawnTrainCoroutine = StartCoroutine(TrainSpawner());
        }

        public void Affect(TapeType tapeType, float duration, float effectValue)
        {
            
        }
    }
}