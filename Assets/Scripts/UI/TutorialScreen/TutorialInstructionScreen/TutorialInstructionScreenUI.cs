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

        private void Update()
        {
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

                // Deselect the currently selected UI element
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
