using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using TMPro;
using UI;
using UnityEngine;

public class FadeText : MonoBehaviour
{
    [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;
    private TextMeshProUGUI uiText;
    private float timerDurationToAppear;
    private float timer = 0f;
    private float fadeInDuration;
    private bool hasFadedIn = false;

    private void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        var originalColor = uiText.color;
        uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        timerDurationToAppear = tutorialInstructionScreenManager.GetTimeToAppear();
        fadeInDuration = tutorialInstructionScreenManager.GetTextFadeDuration();
    }

    private void Update()
    {
        if (hasFadedIn) return;

        timer += Time.unscaledDeltaTime;
        if (timer < timerDurationToAppear) return;

        StartCoroutine(FadeIn());
        hasFadedIn = true;
    }

    private IEnumerator FadeIn()
    {
        Color originalColor = uiText.color;
        float elapsed = 0f;

        // Set initial alpha to 0
        uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (elapsed < fadeInDuration)
        {
            float alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
