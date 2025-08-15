using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scenes;

public class InCombatDialogueManager : Singleton<InCombatDialogueManager>
{
    [SerializeField] private CanvasGroup inCombatDialogueCanvasGroup;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float screenFadeDuration = 0.5f;
    [SerializeField] private float typingSpeed = 0.01f;
    private bool isTyping = false;  // Flag to track whether text is still being typed
    private Coroutine typeCoroutine;
    private Coroutine dialogueCoroutine;

    void Start()
    {
        HideDialogueImmediate();
    }

    public IEnumerator ShowAndHideDialogue(string text, float displayDuration)
    {
        // Stop previous dialogue if it's still running
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        dialogueCoroutine = StartCoroutine(ShowAndHideDialogueRoutine(text, displayDuration));
        yield return dialogueCoroutine;
    }

    private IEnumerator ShowAndHideDialogueRoutine(string text, float displayDuration)
    {
        bool wasActive = inCombatDialogueCanvasGroup.gameObject.activeSelf;

        // Make sure canvas is active
        inCombatDialogueCanvasGroup.gameObject.SetActive(true);

        if (!wasActive || inCombatDialogueCanvasGroup.alpha < 1f)
        {
            dialogueText.text = "";
            yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(
                inCombatDialogueCanvasGroup, 0, 1, screenFadeDuration));
        }

        // Display text
        dialogueText.text = "";
        StartTyping(text);

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(
            inCombatDialogueCanvasGroup, 1, 0, screenFadeDuration));

        inCombatDialogueCanvasGroup.gameObject.SetActive(false);
        dialogueCoroutine = null;
    }

    public void HideDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutScreen());
    }

    private IEnumerator FadeOutScreen()
    {
        yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(
            inCombatDialogueCanvasGroup, 1, 0, screenFadeDuration));
        inCombatDialogueCanvasGroup.gameObject.SetActive(false);
    }

    public void HideDialogueImmediate()
    {
        inCombatDialogueCanvasGroup.gameObject.SetActive(false);
        inCombatDialogueCanvasGroup.alpha = 0;
    }

    // Method to start typing the text
    public void StartTyping(string text)
    {
        if (isTyping)
        {
            // If already typing, stop the current typing coroutine and show full text
            StopCoroutine(typeCoroutine);
            dialogueText.text = text;  // Show the full text immediately
            isTyping = false;  // Set the flag to false since the text is fully revealed
        }
        else
        {
            // Start the typewriter effect
            typeCoroutine = StartCoroutine(TypeText(text));
        }
    }

    // Coroutine for typewriter effect
    private IEnumerator TypeText(string text)
    {
        isTyping = true;  // Set the flag to true when typing starts
        dialogueText.text = "";  // Clear the text initially

        foreach (char letter in text)
        {
            dialogueText.text += letter;  // Add one letter at a time
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;  // Set the flag to false when typing is complete
    }
}
