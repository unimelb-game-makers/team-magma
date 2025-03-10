// Author : Peiyu Wang @ Daphatus
// 10 03 2025 03 47

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Player.Stats
{
    public class PlayerHealth : PlayerStat
    {
        /// <summary>
        /// Event that can be used to trigger player death when health reaches zero.
        /// </summary>
        public UnityAction OnDeath;
        public bool IsDead => CurrentValue <= 0;
        
        [Header("Modify to adjust the time the player is immune to damage after taking damage.")]
        [SerializeField] private float damageImmuneTime = 1f;

        /// <summary>
        /// When damage is taken but the player is invincible.
        /// </summary>
        public event Action onDamageImmune;
        
        public override StatType StatType => StatType.Health;
        
        private Coroutine _damageImmuneCoroutine;

        public override float Modify(float amount)
        {
            //if the player is not invincible, take damage
            if (_damageImmuneCoroutine == null)
            {
                _damageImmuneCoroutine = StartCoroutine(DamageImmune());
                CurrentValue += amount;
                if (CurrentValue <= 0)
                {
                    CurrentValue = 0;
                    OnDeath?.Invoke();
                }
                return amount;
            }
            //Didn't take damage
            OnDamageImmune();
            return 0;
        }
        
        /// <summary>
        /// If health reaches zero, trigger death event.
        /// </summary>
        /// <param name="amount"></param>
        public override void DecreaseMaxValue(float amount)
        {
            base.DecreaseMaxValue(amount);
            if(MaxValue <= 0)
            {
                OnDeath?.Invoke();
            }
        }
        
        private void OnDamageImmune()
        {
            onDamageImmune?.Invoke();
        }
        
        private IEnumerator DamageImmune()
        {
            OnDamageImmune();
            yield return new WaitForSeconds(damageImmuneTime);
            _damageImmuneCoroutine = null;
        }
    }
}