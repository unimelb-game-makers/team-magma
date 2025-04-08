using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Player.Stats
{
    public class PlayerStats : MonoBehaviour, IHealthManager
    {
        private PlayerCharacter playerCharacter;
        [SerializeField] private PlayerHealth healthStat;
        public PlayerHealth HealthStat => healthStat;
        public void Awake()
        {
            playerCharacter = GetComponent<PlayerCharacter>();
            if (healthStat == null)
            {
                throw new Exception("PlayerStats requires a health stat to function.");
            }
        }
        
        
        private void OnEnable()
        {
            healthStat.OnDeath += OnDeath;
            healthStat.OnValueChanged += OnDamaged;
            healthStat.onDamageImmune += OnImmune;
        }
        
        private void OnDisable()
        {
            healthStat.OnDeath -= OnDeath;
            healthStat.OnValueChanged -= OnDamaged;
            healthStat.onDamageImmune -= OnImmune;
        }

        public void TakeDamage(float damage = 1)
        {
            var absDamage = Mathf.Abs(damage);
            healthStat.Modify(-absDamage);
        }

        public bool IsDead()
        {
            return healthStat.IsDead;
        }

        private void OnDeath()
        {
            DefeatScreenManager.Instance.ShowDefeatScreen();
        }

        private void OnDamaged(float health)
        {
            Debug.Log("PlayerStats Player health: " + health);
        }
        
        private void OnImmune()
        {
            Debug.Log("PlayerStats Player is immune to damage");
        }
        
        public void OnReset()
        {
            healthStat.Reset();
        }
    }
}
