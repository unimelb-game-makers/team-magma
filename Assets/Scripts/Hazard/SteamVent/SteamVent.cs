// Author : Peiyu Wang @ Daphatus, William Alexander Tang Wai @ Jalapeno
// 05 12 2024 12 54

using Platforms;
using Tempo;
using Utilities.ServiceLocator;
using UnityEngine;
using System.Collections;
using System;
using Damage;

namespace Hazard
{
    public class SteamVent : Hazard, IDamageManager
    {
        /**
         * The 'steamVentArea' object is the object that detects characters and 
         * calls the damage function in the steamvent class.
         */
        private GameObject steamVentArea;

        /**
         * Damage dealt by the steamvent to player/enemies.
         */
        [SerializeField] private float damage = 10;

        /**
         * Interval between when player/enemies take damage.
         */
        [SerializeField] private float damageInterval = 1;

        public float GetDamageInterval()
        {
            return damageInterval;
        }

        public void Awake()
        {
            // The 'steamVentArea' object is the child of the 'SteamVent' object.
            steamVentArea = transform.Find("SteamVentArea").gameObject;
        }

        public void Start()
        {
            ServiceLocator.Instance.Register<ISyncable>(this);
        }

        /**
         * Damage the characters that enter the 'steamVentArea' object.
         */
        public void DealDamage(Damageable target)
        {
            target.TakeDamage(damage);

            // Apply a knockback: Should be a method in the playerController.
            // This knockback could apply whenever the player receives damage
            // from any source.
        }

        /**
         * Switch the steamvent off for 'duration' seconds.
         */
        public override void Affect(TapeType tapeType, float duration, float effectValue)
        {
            if(tapeType == TapeType.Slow)
            {
                // Disable the 'steamVentArea' object so no damage is dealt.
                steamVentArea.SetActive(false);

                // Code for Animations and Sounds.

                StartCoroutine(AffectTimer(duration));
            }
        }

        /**
         * After 'duration' seconds, the 'steamVentArea' object is switched on again.
         */
        private IEnumerator AffectTimer(float duration)
        {
            yield return new WaitForSeconds(duration);

            // Code for Animations and Sounds.
            
            steamVentArea.SetActive(true);
        }
    }
}