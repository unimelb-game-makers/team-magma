using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TapeNotificationManager : Singleton<TapeNotificationManager>
{
    public RectTransform uiGroup;              // Group of UI elements
    public Image tapeIcon;                     // Image for the tape
    public Sprite fastTapeSprite;
    public Sprite slowTapeSprite;
    public TextMeshProUGUI timerText;          // Text to show remaining time
    public float moveDistance = 50f;           // How far the UI moves in
    public float moveDuration = 0.3f;          // Animation duration
    public CanvasGroup canvasGroup;            // For fade/spark effect

    [Header("Spark Flash Settings")]
    public float flashBaseSpeed = 1f;          // Slowest flash rate (start of spark)
    public float flashMaxSpeed = 2f;           // Fastest flash rate (right before end)
    public float sparkThreshold = 3f;        // Time left when sparking begins

    private Coroutine currentRoutine;
    private Coroutine countdownRoutine;
    private bool isFadingOut = false;
    private Vector2 initialPosition;

    void Start()
    {
        initialPosition = uiGroup.anchoredPosition;
    }

    public void ActivateTapeUI(TapeType tape, float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);

        isFadingOut = false;
        currentRoutine = StartCoroutine(HandleTapeUI(tape, duration));
    }

    private IEnumerator HandleTapeUI(TapeType tape, float duration)
    {
        // Set correct sprite
        tapeIcon.sprite = tape == TapeType.Fast ? fastTapeSprite : slowTapeSprite;

        // Move UI in
        uiGroup.anchoredPosition = initialPosition;
        Vector2 startPos = initialPosition;
        Vector2 targetPos = startPos - new Vector2(moveDistance, 0);
        float elapsed = 0f;
        canvasGroup.alpha = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            uiGroup.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, Mathf.Clamp01(t * 0.5f)); // Fade in slower
            yield return null;
        }

        uiGroup.anchoredPosition = targetPos;
        canvasGroup.alpha = 1f;

        // Start countdown
        countdownRoutine = StartCoroutine(CountdownCoroutine(duration));

        // Wait for countdown to end or fade-out trigger
        yield return countdownRoutine;

        // Proceed to fade out
        yield return FadeOutCoroutine();
    }

    private IEnumerator CountdownCoroutine(float duration)
    {
        float timeLeft = duration;
        float sparkElapsed = 0f;
        float effectiveSparkThreshold = Mathf.Min(sparkThreshold, duration - moveDuration);

        while (timeLeft > moveDuration && !isFadingOut)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";

            if (timeLeft <= effectiveSparkThreshold)
            {
                float flashProgress = 1f - (timeLeft / effectiveSparkThreshold);
                flashProgress = Mathf.Clamp01(flashProgress);
                float flashRate = Mathf.Lerp(flashBaseSpeed, flashMaxSpeed, flashProgress);
                sparkElapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.PingPong(sparkElapsed * flashRate, 1f);
            }
            else
            {
                canvasGroup.alpha = 1f;
            }

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        isFadingOut = true;
    }

    public void FadeOutUI()
    {
        if (isFadingOut) return;

        isFadingOut = true;

        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        currentRoutine = StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        Vector2 currentPos = uiGroup.anchoredPosition;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            uiGroup.anchoredPosition = Vector2.Lerp(currentPos, initialPosition, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Min(1f, t * 2f)); // Fade out faster
            yield return null;
        }

        // Fully reset
        ResetTapeNotification();
    }

    public void ResetTapeNotification()
    {
        uiGroup.anchoredPosition = initialPosition;
        canvasGroup.alpha = 0f;
        timerText.text = "";
        countdownRoutine = null;
        currentRoutine = null;
        isFadingOut = false;
    }
}

