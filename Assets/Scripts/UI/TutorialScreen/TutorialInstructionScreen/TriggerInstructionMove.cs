using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Unity.VisualScripting;

[RequireComponent(typeof(Collider))]
public class TriggerInstructionMove : MonoBehaviour
{
    private bool isTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            TutorialInstructionScreenManager.Instance.ShowMoveScreen();
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