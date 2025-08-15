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
    [SerializeField] private float displayDuration = 8f; // Time to show dialogue before auto-hiding
    private bool isTyping = false;  // Flag to track whether text is still being typed
    private Coroutine currentCoroutine;

    void Start()
    {
        inCombatDialogueCanvasGroup.gameObject.SetActive(false);
        inCombatDialogueCanvasGroup.alpha = 0;
    }

    public IEnumerator ShowAndHideDialogue(string text)
    {
        // Fade in
        dialogueText.text = "";
        inCombatDialogueCanvasGroup.gameObject.SetActive(true);
        yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(
            inCombatDialogueCanvasGroup, 0, 1, screenFadeDuration));
        
        // Start typing effect
        StartTyping(text);
        
        // Wait for both typing AND display duration
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(
            inCombatDialogueCanvasGroup, 1, 0, screenFadeDuration));
        
        inCombatDialogueCanvasGroup.gameObject.SetActive(false);
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

    // Method to start typing the text
    public void StartTyping(string text)
    {
        if (isTyping)
        {
            // If already typing, stop the current typing coroutine and show full text
            StopCoroutine(currentCoroutine);
            dialogueText.text = text;  // Show the full text immediately
            isTyping = false;  // Set the flag to false since the text is fully revealed
        }
        else
        {
            // Start the typewriter effect
            currentCoroutine = StartCoroutine(TypeText(text));
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
