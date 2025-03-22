// Author : Peiyu Wang @ Daphatus
// 23 03 2025 03 39

using System.Level;
using UnityEngine;

namespace System
{
    /// <summary>
    /// Manager that controls the sub-levels of the game.
    /// </summary>
    public class SubGameManager : Singleton<SubGameManager>
    {
        [SerializeField] private LevelSpawnPoint _levelSpawnPoint;
        public Transform LevelSpawnPoint
        {
            get
            {
                if(_levelSpawnPoint == null)
                {
                    _levelSpawnPoint = FindObjectOfType<LevelSpawnPoint>();
                    if(_levelSpawnPoint == null)
                    {
                        throw new Exception("No PlayerSpawnPoint found in the scene.");
                    }
                }
                return _levelSpawnPoint.gameObject.transform;
            }
        }
    }
}