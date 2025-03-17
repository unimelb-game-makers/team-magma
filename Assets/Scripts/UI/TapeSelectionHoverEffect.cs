using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TapeSelectionHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.1f;
    public float hoverHeight = 30.0f;   // Movement in the Y direction
    public float hoverWidth = 30.0f;    // Movement in the X direction
    public float duration = 0.2f;
    public Vector2 hoverDirection = new Vector2(0, 1); // Default to moving straight up

    private Vector3 originalScale;
    private Vector3 originalLocalPosition;  // Store the original local position
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();  // Ensure using RectTransform for UI elements
        originalScale = rectTransform.localScale;
        originalLocalPosition = rectTransform.localPosition;  // Store local position
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Kill any existing tween on the rectTransform to ensure smooth transition
        rectTransform.DOKill();

        // Calculate the target local position based on hover direction (using local space)
        Vector3 targetPosition = originalLocalPosition + new Vector3(hoverDirection.x * hoverWidth, hoverDirection.y * hoverHeight, 0);

        // Animate scale and position using DOTween
        rectTransform.DOScale(originalScale * hoverScale, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true)  // Use unscaledDeltaTime
            .WaitForCompletion();
        rectTransform.DOLocalMove(targetPosition, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true)  // Use unscaledDeltaTime
            .WaitForCompletion();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Kill any existing tween on the rectTransform to ensure smooth transition
        rectTransform.DOKill();

        // Reset to original position and scale
        rectTransform.DOScale(originalScale, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true)  // Use unscaledDeltaTime
            .WaitForCompletion();
        rectTransform.DOLocalMove(originalLocalPosition, duration).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true)  // Use unscaledDeltaTime
            .WaitForCompletion();
    }
}

