// Author : Peiyu Wang @ Daphatus
// 19 03 2025 03 37

using System;
using System.Collections;
using UnityEngine;

namespace Damage
{
    /// <summary>
    /// This implementation is ok if not considering enemy gets instantly killed in the area
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DamageVolume : MonoBehaviour
    {
        [SerializeField] private float damage = 1f;
        [SerializeField] private float damageInterval = 1f;
        [SerializeField] private GameObject visualEffect;
        private bool _canDamage = true;
        private bool _isActivated = false;
        public void OnTriggerStay(Collider other)
        {
            if(_isActivated && _canDamage && other.gameObject.GetComponent<Damageable>() is { } damageable)
            {
                damageable.TakeDamage(1);
                StartCoroutine(DamageInterval());
            }
        }

        private IEnumerator DamageInterval()
        {
            yield return new WaitForSeconds(damageInterval);
        }
        
        public void Activate()
        {
            _isActivated = true;
            if (visualEffect != null)
                visualEffect?.SetActive(true);
        }
        
        public void Deactivate()
        {
            _isActivated = false;
            if (visualEffect != null)
                visualEffect?.SetActive(false);
        }
    }
}