using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.Stats;
using UI;
using UnityEngine;

namespace UI {
    public class TriggerHealthInstructions : MonoBehaviour
    {
        [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;
        private bool hasDisplayed = false;

        private PlayerController _Player;
        public PlayerController Player
        {
            get
            {
                if(!_Player)
                {
                    _Player = GameManager.Instance.PlayerController;
                }
                return _Player;
            }
        }

        private void Update()
        {
            // If HP is reduced, then show HP screen
            if (Player) {
                if (Player.GetComponent<PlayerStats>().isDamaged) {
                    Player.GetComponent<PlayerStats>().ResetIsDamaged();
                    if (Player.GetComponent<PlayerStats>().IsDead()) return;
                    if (!hasDisplayed) {
                        hasDisplayed = true;
                        tutorialInstructionScreenManager.ShowHPScreen();
                    }
                }
            }
        }
    }
}
