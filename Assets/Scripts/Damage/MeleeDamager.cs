using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

public class MeleeDamager : MonoBehaviour, IDamageManager
{
    [SerializeField] private float damage = 10;
    public float Damage { get => damage; set => damage = value; }
    
    /**
     * Deal damage to the object.
     */
    public void DealDamage(Damageable target)
    {
        target.TakeDamage(damage);
    }
}
