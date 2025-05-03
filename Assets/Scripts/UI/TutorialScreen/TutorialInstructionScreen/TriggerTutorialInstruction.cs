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
            isTriggered = true;
            switch (screenToShow)
            {
                case TutorialScreenType.Move:
                    tutorialInstructionScreenManager.ShowMoveScreen();
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
        }
    }

    // Destroy so the instruction screen cannot be retriggered.
    private void OnTriggerExit(Collider other)
    {
        if (!isTriggered) return;

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}