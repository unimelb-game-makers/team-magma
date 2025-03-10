// Author : Peiyu Wang @ Daphatus
// 10 03 2025 03 52

using System;
using Player;
using Player.Stats;
using UnityEngine;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        // Start is called before the first frame update
        private PlayerStats _playerStats;

        #region Internal Methods
        private void Start()
        {
            GetPlayerStats();
        }

        private void OnEnable()
        {
            GetPlayerStats();
        }

        private void GetPlayerStats()
        {
            if(_playerStats == null)
            {
                if(GameManager.Instance == null)
                {
                    throw new Exception("GameManager not found in the scene. 1. check if there is a game manager" +
                                        "2. check if it is initialise(Execution order) before this script");
                }
                _playerStats = GameManager.Instance.PlayerCharacter.PlayerStats;
                if(_playerStats == null)
                {
                    throw new Exception("The stats was not set properly, check player character");
                }
                _playerStats.HealthStat.OnValueChanged += UpdateHealthBar;
                _playerStats.HealthStat.OnDeath += UpdateDeath;
                _playerStats.HealthStat.onDamageImmune += UpdateDamageImmune;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from the events
            if(_playerStats != null && !Application.isPlaying)
            {
                _playerStats.HealthStat.OnValueChanged -= UpdateHealthBar;
                _playerStats.HealthStat.OnDeath -= UpdateDeath;
                _playerStats.HealthStat.onDamageImmune -= UpdateDamageImmune;
            }
        }
        

        #endregion

        /// <summary>
        /// Called when the player's health is updated
        /// </summary>
        /// <param name="health"></param>
        private void UpdateHealthBar(float health)
        {
            Debug.Log("HealthUI Health: " + health);
        }
        
        /// <summary>
        /// Called when the player is getting hit when immune to damage
        /// </summary>
        private void UpdateDamageImmune()
        {
            Debug.Log("HealthUI Player is immune to damage");
        }
        
        private void UpdateDeath()
        {
            Debug.Log("HealthUI Player died");
        }
    }
}