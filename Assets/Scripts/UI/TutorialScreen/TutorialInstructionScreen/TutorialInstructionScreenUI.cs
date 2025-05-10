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
        [SerializeField] private ButtonToClose buttonToClose = ButtonToClose.Enter;
        private KeyCode keyCode;

        void Start()
        {
            switch (buttonToClose)
            {
                case ButtonToClose.Enter:
                    keyCode = KeyCode.Return;
                    break;
                case ButtonToClose.A:
                    keyCode = KeyCode.A;
                    break;
            }
        }

        private void Update()
        {
            // If the Enter key is pressed
            if (Input.GetKeyDown(keyCode))
            {
                switch (screenToShow)
                {
                    case TutorialScreenType.Move:
                        tutorialInstructionScreenManager.ShowMoveScreen();
                        break;
                    case TutorialScreenType.Jump:
                        tutorialInstructionScreenManager.ShowJumpScreen();
                        break;
                    case TutorialScreenType.SlowTape:
                        tutorialInstructionScreenManager.ShowSlowTapeScreen();
                        break;
                    case TutorialScreenType.HP:
                        tutorialInstructionScreenManager.ShowHPScreen();
                        break;
                    case TutorialScreenType.Beat:
                        tutorialInstructionScreenManager.ShowBeatScreen();
                        break;
                    case TutorialScreenType.Tape:
                        tutorialInstructionScreenManager.ShowTapeScreen();
                        break;
                }

                // May have to remove this; test
                // Deselect the currently selected UI element
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
