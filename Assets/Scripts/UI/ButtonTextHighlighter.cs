using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.UI; // For standard Text
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonTextHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI tmpText; // Assign this for TMP
    public Text uiText; // Assign this for standard Text

    private bool isTMP => tmpText != null;

    public Color normalColor = Color.black; // Default color
    public Color highlightColor = Color.green; // Hover color
    public float transitionDuration = 0.2f; // Duration of the color transition

    private Coroutine colorCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isTMP || uiText != null)
        {
            if (colorCoroutine != null) StopCoroutine(colorCoroutine);
            HighlightText();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isTMP || uiText != null)
        {
            if (colorCoroutine != null) StopCoroutine(colorCoroutine);
            UnHighlightText();
        }
    }

    public void HighlightText()
    {
        if (isTMP)
        {
            tmpText.fontStyle = FontStyles.Bold;
            tmpText.color = highlightColor; // Directly change color
        }
        else if (uiText != null)
        {
            uiText.fontStyle = FontStyle.Bold;
            uiText.color = highlightColor; // Directly change color
        }
    }

    public void UnHighlightText()
    {
        if (isTMP)
        {
            tmpText.fontStyle = FontStyles.Normal;
            tmpText.color = normalColor; // Directly change color
        }
        else if (uiText != null)
        {
            uiText.fontStyle = FontStyle.Normal;
            uiText.color = normalColor; // Directly change color
        }
    }

}

