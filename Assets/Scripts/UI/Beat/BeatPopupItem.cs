using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BeatPopupItem : MonoBehaviour
{
    private const float FADE_DURATION = 0.5f;
    [SerializeField] private BeatHexagonPopupItem leftHexagon;
    [SerializeField] private BeatHexagonPopupItem rightHexagon;

    private RectTransform _leftTarget;
    private RectTransform _rightTarget;

    private Sequence _moveSequence;
    
    public void Init(RectTransform leftTarget, RectTransform rightTarget, float distance, float travelTime)
    {
        // Set the distance of the hexagons first
        Vector2 leftPos = leftTarget.anchoredPosition;
        leftPos.x -= distance;
        leftHexagon.Rect.anchoredPosition = leftPos;
        
        Vector2 rightPos = rightTarget.anchoredPosition;
        rightPos.x += distance;
        rightHexagon.Rect.anchoredPosition = rightPos;
        
        // Create tweens to move the hexagons
        _moveSequence = DOTween.Sequence();
        _moveSequence.Append(leftHexagon.Rect.DOAnchorPos(leftTarget.anchoredPosition, travelTime).SetEase(Ease.Linear));
        _moveSequence.Join(rightHexagon.Rect.DOAnchorPos(rightTarget.anchoredPosition, travelTime).SetEase(Ease.Linear));
        _moveSequence.Play();
    }

    public void Resolve()
    {
        _moveSequence.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(leftHexagon.Image.DOFade(0f, FADE_DURATION).SetEase(Ease.InOutCubic));
        sequence.Join(rightHexagon.Image.DOFade(0f, FADE_DURATION).SetEase(Ease.InOutCubic));
        sequence.AppendCallback(() => { Destroy(gameObject); });
    }
}
