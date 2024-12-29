using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour, IHealthManager
    {
        PlayerCharacter playerCharacter;

        public void Awake()
        {
            playerCharacter = GetComponent<PlayerCharacter>();
        }

        public void TakeDamage(float damage)
        {
            var health = playerCharacter.GetHealth();
            health -= damage;
            playerCharacter.SetHealth(health);
            Debug.Log($"Player takes {damage} damage. Health: {health}");
        }

        public bool IsDead()
        {
            return playerCharacter.GetHealth() <= 0;
        }
    }
}
