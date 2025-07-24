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

    private Sequence _sequence;
    private Sequence _resolveSequence;

    private BeatSpawner _spawner;
    private int _beat;
    
    public void Init(BeatSpawner spawner, int beat, RectTransform leftTarget, RectTransform rightTarget, float distance, float travelTime)
    {
        _spawner = spawner;
        _beat = beat;
        // Set the distance of the hexagons first
        Vector2 leftPos = leftTarget.anchoredPosition;
        leftPos.x -= distance;
        leftHexagon.Rect.anchoredPosition = leftPos;
        
        Vector2 rightPos = rightTarget.anchoredPosition;
        rightPos.x += distance;
        rightHexagon.Rect.anchoredPosition = rightPos;
        
        // Create tweens to move the hexagons
        _sequence = DOTween.Sequence();
        _sequence.Append(leftHexagon.Rect.DOAnchorPos(leftTarget.anchoredPosition, travelTime).SetEase(Ease.Linear));
        _sequence.Join(rightHexagon.Rect.DOAnchorPos(rightTarget.anchoredPosition, travelTime).SetEase(Ease.Linear));
        _sequence.Play().OnComplete(Resolve);
    }

    private void Resolve()
    {
        // Debug.Log($"Resolving Beat {_beat}");
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(leftHexagon.Image.DOFade(0f, FADE_DURATION).SetEase(Ease.InOutCubic));
        _sequence.Join(rightHexagon.Image.DOFade(0f, FADE_DURATION).SetEase(Ease.InOutCubic));
        _sequence.AppendCallback(() => { Destroy(gameObject); });
        
        _spawner.ResolveBeat(_beat);
    }
}
