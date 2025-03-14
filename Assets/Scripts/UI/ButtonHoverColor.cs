using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image backgroundImage;        // Assign the background image of the button
    public Color hoverColor = Color.gray; // Color to change on hover
    public float animationDuration = 0.2f; // Duration of the color transition

    private Color originalColor;

    void Start()
    {
        // Save the original color of the background image
        if (backgroundImage != null)
        {
            originalColor = backgroundImage.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            StopAllCoroutines(); // Stop any ongoing animations
            StartCoroutine(ChangeColor(originalColor, hoverColor, animationDuration));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            StopAllCoroutines(); // Stop any ongoing animations
            StartCoroutine(ChangeColor(backgroundImage.color, originalColor, animationDuration));
        }
    }

    private System.Collections.IEnumerator ChangeColor(Color from, Color to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            backgroundImage.color = Color.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        backgroundImage.color = to; // Ensure final color is set
    }
}

