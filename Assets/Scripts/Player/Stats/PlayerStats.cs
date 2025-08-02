using System;
using UnityEngine;

namespace Player.Stats
{
    public class PlayerStats : MonoBehaviour, IHealthManager
    {
        [SerializeField] private PlayerHealth healthStat;
        public PlayerHealth HealthStat => healthStat;

        // For tutorial UI only ##############
        public bool isDamaged = false;
        public void ResetIsDamaged() {
            isDamaged = false;
        }
        // ###################################  

        public void Awake()
        {
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
            isDamaged = true;
        }

        public bool IsDead()
        {
            return healthStat.IsDead;
        }

        private void OnDeath()
        {
            GameManager.Instance.OnPlayerDead();
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
