using System.Collections.Generic;
using UnityEngine;

namespace Damage
{
    /**
     * Component that deals damage to other objects on collision.
     */
    [RequireComponent(typeof(Collider))]
    public class Damager : MonoBehaviour
    {
        private IDamageManager damageManager;

        [SerializeField] private List<string> canAttack = new List<string>();

        public void Awake()
        {
            // Get the damage manager component.
            damageManager = GetComponent<IDamageManager>();
            if (damageManager == null)
            {
                Debug.LogError("No IDamageManager implementation found on the GameObject.");
            }
        }

        public void Damage(Collider other)
        {
            if(other==null) return;
            if(other.gameObject.GetComponent<Damageable>()==null) return;
            if (canAttack.Contains(other.gameObject.tag))
            {
                damageManager.DealDamage(other.gameObject.GetComponent<Damageable>());
            }
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
