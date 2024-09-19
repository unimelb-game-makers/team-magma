using System;
using Unity.Collections;
using UnityEngine;
using Utilities.ServiceLocator;

namespace Player
{
    public class PlayerCharacter : MonoBehaviour, ISavePlayer
    {
        
        [SerializeField] private PlayerStats defaultPlayerStats;
        [InspectorName("Health")] 
        [SerializeField] private float health = 100;
        [SerializeField] private float healthMax = 100;
        [SerializeField] private float batteryLevel = 100;
        [SerializeField] private float batteryMax = 100;

        private void Awake()
        {
            ServiceLocator.Instance.Register<ISavePlayer>(this);
        }

        private void Start()
        {
        }

        object ISaveGame.OnSaveData()
        {
            var currPlayerStats = ScriptableObject.CreateInstance<PlayerStats>();
            currPlayerStats.health = health;
            currPlayerStats.healthMax = healthMax;
            currPlayerStats.batteryLevel = batteryLevel;
            currPlayerStats.batteryMax = batteryMax;
            return currPlayerStats;
        }

        void ISaveGame.OnLoadData(object data)
        {
            var stats = (PlayerStats) data;
            health = (int) stats.health;
            healthMax = (int) stats.healthMax;
            batteryLevel = (int) stats.batteryLevel;
            batteryMax = (int) stats.batteryMax;
            Debug.Log("Player Stats: " + health + " healthMax: " + healthMax + " batteryLevel: " + batteryLevel + " batteryMax: " + batteryMax);

        }

        void ISaveGame.LoadDefaultData()
        {
            health = defaultPlayerStats.health;
            healthMax = defaultPlayerStats.healthMax;
            batteryLevel = defaultPlayerStats.batteryLevel;
            batteryMax = defaultPlayerStats.batteryMax;
        }
    }
}