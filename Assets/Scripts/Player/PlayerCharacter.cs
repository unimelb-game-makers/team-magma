using System;
using Unity.Collections;
using UnityEngine;
using Utilities.ServiceLocator;

namespace Player
{
    public class PlayerCharacter : MonoBehaviour, ISavePlayer
    {
        
        [InspectorName("Health")] 
        [SerializeField] private float health = 100;
        [SerializeField] private float healthMax = 100;
        [SerializeField] private float batteryLevel = 100;
        [SerializeField] private float batteryMax = 100;

        public float GetHealth()
        {
            return health;
        }

        public void SetHealth(float newHealth)
        {
            health = newHealth;
        }

        private void Awake()
        {
            ServiceLocator.Instance.Register<ISavePlayer>(this);
        }

        private void Start()
        {
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