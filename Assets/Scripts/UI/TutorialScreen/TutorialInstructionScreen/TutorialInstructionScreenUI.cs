using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Scenes;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class TutorialInstructionScreenUI: MonoBehaviour
    {
        [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;
        [SerializeField] private TutorialScreenType screenToShow;
        [SerializeField] private float timerDuration = 1f;
        private float timer = 0;

        private void Update()
        {
            timer += Time.unscaledDeltaTime;
            if (timer < timerDuration) return;

            // If any key is pressed
            if (Input.anyKeyDown)
            {
                switch (screenToShow)
                {
                    case TutorialScreenType.Move:
                        tutorialInstructionScreenManager.ShowSmallMoveScreen();
                        break;
                    case TutorialScreenType.Jump:
                        tutorialInstructionScreenManager.ShowSmallJumpScreen();
                        break;
                    case TutorialScreenType.SlowTape:
                        tutorialInstructionScreenManager.ShowSmallSlowTapeScreen();
                        break;
                    case TutorialScreenType.BasicAttack1:
                        tutorialInstructionScreenManager.ShowSmallBasicAttack1Screen();
                        break;
                    case TutorialScreenType.BasicAttack2:
                        tutorialInstructionScreenManager.ShowSmallBasicAttack1Screen();
                        break;
                    case TutorialScreenType.BasicAttack3:
                        tutorialInstructionScreenManager.ShowSmallBasicAttack1Screen();
                        break;
                    case TutorialScreenType.RangedAttack:
                        tutorialInstructionScreenManager.ShowSmallRangedAttackScreen();
                        break;
                    case TutorialScreenType.ShieldAttack:
                        tutorialInstructionScreenManager.ShowSmallShieldAttackScreen();
                        break;
                    case TutorialScreenType.HP:
                        tutorialInstructionScreenManager.HideHPScreen();
                        break;
                }

                PauseMenuController.BlockEscapeKey(false);
                SelectionWheelManager.BlockRightClick(false);

                // Deselect the currently selected UI element
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
