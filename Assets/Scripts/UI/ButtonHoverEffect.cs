using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform backgroundImage; // Assign the background image of the button
    public float hoverScale = 1.2f;       // Scale multiplier for hover effect
    public float animationDuration = 0.2f; // Duration of the scaling effect

    private Vector2 originalSize;

    void Start()
    {
        // Save the original size of the background image
        if (backgroundImage != null)
        {
            originalSize = backgroundImage.sizeDelta;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            StopAllCoroutines(); // Stop any ongoing animations
            StartCoroutine(ScaleBackground(originalSize, originalSize * hoverScale, animationDuration));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            StopAllCoroutines(); // Stop any ongoing animations
            StartCoroutine(ScaleBackground(backgroundImage.sizeDelta, originalSize, animationDuration));
        }
    }

    private System.Collections.IEnumerator ScaleBackground(Vector2 from, Vector2 to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            backgroundImage.sizeDelta = Vector2.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        backgroundImage.sizeDelta = to; // Ensure final size is set
    }
}
