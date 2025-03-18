using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class TapeSelectionHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.1f;
    public float hoverHeight = 30.0f;   // Movement in the Y direction
    public float hoverWidth = 30.0f;    // Movement in the X direction
    public float duration = 0.2f;
    public Vector2 hoverDirection = new Vector2(0, 1); // Default to moving straight up

    public GameObject descriptionText;  // Reference to the description text GameObject
    public Color hoverColor = Color.grey; // The color to transition to when hovering
    public Color normalColor = Color.white; // The color to transition back to when not hovering

    private Vector3 originalScale;
    private Vector3 originalLocalPosition;  // Store the original local position
    private RectTransform rectTransform;
    private Image buttonImage; // Reference to the Image component of the button

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();  // Ensure using RectTransform for UI elements
        buttonImage = GetComponent<Image>();  // Get the Image component for color transitions
        originalScale = rectTransform.localScale;
        originalLocalPosition = rectTransform.localPosition;  // Store local position

        // Initially hide the description text
        if (descriptionText != null)
        {
            descriptionText.SetActive(false);
        }

        // Set the initial button color
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Kill any existing tween on the rectTransform to ensure smooth transition
        rectTransform.DOKill();
        buttonImage.DOKill(); // Kill any existing color tween

        // Calculate the target local position based on hover direction (using local space)
        Vector3 targetPosition = originalLocalPosition + new Vector3(hoverDirection.x * hoverWidth, hoverDirection.y * hoverHeight, 0);

        // Animate scale and position using DOTween
        rectTransform.DOScale(originalScale * hoverScale, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);
        rectTransform.DOLocalMove(targetPosition, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);

        // Smoothly change the button color to the hover color
        if (buttonImage != null)
        {
            buttonImage.DOColor(hoverColor, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);
        }

        // Show the description text
        if (descriptionText != null)
        {
            descriptionText.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Kill any existing tween on the rectTransform to ensure smooth transition
        rectTransform.DOKill();
        buttonImage.DOKill(); // Kill any existing color tween

        // Hide the description text
        if (descriptionText != null)
        {
            descriptionText.SetActive(false);
        }

        // Smoothly reset to original position and scale
        rectTransform.DOScale(originalScale, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);
        rectTransform.DOLocalMove(originalLocalPosition, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);

        // Smoothly transition back to the normal color
        if (buttonImage != null)
        {
            buttonImage.DOColor(normalColor, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);
        }
    }
}


