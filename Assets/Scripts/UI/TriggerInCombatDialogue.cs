using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Unity.VisualScripting;

[RequireComponent(typeof(Collider))]
public class TriggerInCombatDialogue : MonoBehaviour
{
    [SerializeField] private string text;
    private bool isTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("triggered");
            isTriggered = true;
            StartCoroutine(InCombatDialogueManager.Instance.ShowAndHideDialogue(text));
        }
    }
}
