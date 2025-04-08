using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Enemies
{
    public class EnemyHealth : MonoBehaviour, IHealthManager
    {
        EnemyController enemyController;

        public void Awake()
        {
            enemyController = GetComponent<EnemyController>();
        }
        public void TakeDamage(float damage)
        {
            var health = enemyController.GetHealth();
            health -= damage;
            enemyController.SetHealth(health);
            Debug.Log($"Enemy takes {damage} damage. Health: {health}");
        }

        public bool IsDead()
        {
            return enemyController.GetHealth() <= 0;
        }
    }
}