// Author : Peiyu Wang @ Daphatus
// 10 03 2025 03 52

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Player.Stats;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        public List<Image> healthImages;
        public Sprite healthSprite;
        public Sprite lostHealthSprite;
        public Color normalColor = Color.white;
        public float warningRatio = 0.6f;
        public Color warningColor = new Color(1.0f, 0.65f, 0.0f);
        public float dangerRatio = 0.2f;
        public Color dangerColor = Color.red;
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
            int maxHealth = Mathf.CeilToInt(_playerStats.HealthStat.MaxValue);
            float healthRatio = health / maxHealth;

            // Change color based on health ratio
            Color healthColor = normalColor;
            if (healthRatio <= dangerRatio)
            {
                healthColor = dangerColor;
            }
            else if (healthRatio <= warningRatio)
            {
                healthColor = warningColor;
            }

            for (int i = 0; i < healthImages.Count; i++)
            {
                float imageRatio = (float)(i + 1) / healthImages.Count;

                if (imageRatio <= healthRatio)
                {
                    healthImages[i].sprite = healthSprite;
                }
                else
                {
                    healthImages[i].sprite = lostHealthSprite;
                }

                // Apply the color to each image
                healthImages[i].color = healthColor;
            }
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