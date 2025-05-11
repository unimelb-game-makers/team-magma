using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Unity.VisualScripting;

[RequireComponent(typeof(Collider))]
public class TriggerTutorialInstruction : MonoBehaviour
{
    [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;
    [SerializeField] private TutorialScreenType screenToShow;

    private bool isTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            PauseMenuController.BlockEscapeKey(true);
            SelectionWheelManager.BlockRightClick(true);
            isTriggered = true;
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
                case TutorialScreenType.BasicAttack1:
                    tutorialInstructionScreenManager.ShowBasicAttack1Screen();
                    break;
            }
        }
    }

    // Destroy so the instruction screen cannot be retriggered.
    private void OnTriggerExit(Collider other)
    {
        if (!isTriggered) return;

        if (other.CompareTag("Player"))
        {
            PauseMenuController.BlockEscapeKey(false);
            SelectionWheelManager.BlockRightClick(false);
            Destroy(gameObject);
        }
    }
}