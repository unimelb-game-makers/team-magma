using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerInCombatDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private string text;
    [SerializeField] private bool resetOnStart = true; // New serialized toggle
    
    private bool isTriggered = false;
    private Coroutine currentDialogueRoutine;

    private void Start()
    {
        if (resetOnStart)
        {
            ResetTrigger();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered || !other.CompareTag("Player")) return;
        isTriggered = true;
        
        currentDialogueRoutine = StartCoroutine(
            InCombatDialogueManager.Instance.ShowAndHideDialogue(text)
        );
    }

    private void OnDestroy()
    {
        CleanupDialogue();
    }

    public void ResetTrigger()
    {
        isTriggered = false;
        
        if (currentDialogueRoutine != null)
        {
            StopCoroutine(currentDialogueRoutine);
            currentDialogueRoutine = null;
        }
        
    }

    private void CleanupDialogue()
    {
        if (!isTriggered) return;
        
        if (currentDialogueRoutine != null)
        {
            StopCoroutine(currentDialogueRoutine);
        }
        
        if (InCombatDialogueManager.Instance != null)
        {
            InCombatDialogueManager.Instance.HideDialogueImmediate();
        }
    }
}
