using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damage
{
    /**
     * Component that deals damage to other objects on collision.
     */
    [RequireComponent(typeof(Collider))]
    public class DamageComponent : MonoBehaviour
    {
        /**
         * the amount of damage to deal on target.
         */
        [SerializeField] private int damage = 10;
        [SerializeField] private List<string> canAttack = new List<string>();
        
        /**
         * Deal damage to the target on collision.
         */
        private void OnTriggerEnter(Collider other)
        {
            if(other==null) return;
            if(other.gameObject.GetComponent<Damageable>()==null) return;
            if (canAttack.Contains(other.gameObject.GetComponent<Damageable>().GetTag()))
            {
                Debug.LogError(gameObject.name + " hit " + other.gameObject.name);
                ApplyDamage(other);
                Destroy(gameObject);
            }
        }
    
        private void ApplyDamage(Collider other)
        {
            other.gameObject.GetComponent<Damageable>().ApplyDamage(damage);
        }

        public List<string> GetCanAttack()
        {
            return canAttack;
        }

        public void EditCanAttack(List<string> canAttackList)
        {
            canAttack = new List<string>();
            canAttack.AddRange(canAttackList);
        }
    }
}
