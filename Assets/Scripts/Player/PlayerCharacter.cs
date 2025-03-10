using System;
using Player.Stats;
using Unity.Collections;
using UnityEngine;
using Utilities.ServiceLocator;

namespace Player
{
    public class PlayerCharacter : MonoBehaviour, ISavePlayer
    {
        private PlayerStats _playerStats; public PlayerStats PlayerStats => _playerStats;
        private void Awake()
        {
            _playerStats = GetComponent<PlayerStats>();
            ServiceLocator.Instance.Register<ISavePlayer>(this);
        }

        object ISaveGame.OnSaveData()
        {
            return null;
        }

        void ISaveGame.OnLoadData(object data)
        {
            
        }

        public void LoadDefaultData()
        {
            
        }
    }
}