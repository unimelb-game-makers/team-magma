using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.UI; // For standard Text
using System.Collections;

public class ButtonTextHighlighter : MonoBehaviour
{
    public TextMeshProUGUI tmpText; // Assign this for TMP
    public Text uiText; // Assign this for standard Text

    private bool isTMP => tmpText != null;

    public Color normalColor = Color.black; // Default color
    public Color highlightColor = Color.green; // Hover color
    public float transitionDuration = 0.2f; // Duration of the color transition

    private Coroutine colorCoroutine;

    public void HighlightText()
    {
        if (isTMP)
        {
            Debug.Log("hi");
            tmpText.fontStyle = FontStyles.Bold; // TMP-specific bold
        }
        else if (uiText != null)
        {
            uiText.fontStyle = FontStyle.Bold; // Unity Text bold
        }

        // Start the color transition
        StartColorTransition(highlightColor);
    }

    public void UnHighlightText()
    {
        if (isTMP)
        {
            tmpText.fontStyle = FontStyles.Normal; // TMP-specific normal
        }
        else if (uiText != null)
        {
            uiText.fontStyle = FontStyle.Normal; // Unity Text normal
        }

        // Start the color transition back to normal
        StartColorTransition(normalColor);
    }

    private void StartColorTransition(Color targetColor)
    {
        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }

        if (isTMP)
        {
            colorCoroutine = StartCoroutine(ColorTransition(tmpText, targetColor));
        }
        else if (uiText != null)
        {
            colorCoroutine = StartCoroutine(ColorTransition(uiText, targetColor));
        }
    }

    private IEnumerator ColorTransition(TextMeshProUGUI text, Color targetColor)
    {
        Color currentColor = text.color;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            text.color = Color.Lerp(currentColor, targetColor, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.color = targetColor;
    }

    private IEnumerator ColorTransition(Text text, Color targetColor)
    {
        Color currentColor = text.color;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            text.color = Color.Lerp(currentColor, targetColor, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.color = targetColor;
    }
}
