using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BeatPopupItem : MonoBehaviour
{
    private readonly Color neutralColour = new Color(1, 1, 1, 0.8f);
    private readonly Color perfectColour = new Color(0.2f, 0.8f, 0.1f, 0.8f);
    private readonly Color goodColour = new Color(0.8f, 0.4f, 0.1f, 0.8f);
    private readonly Color failedColour = new Color(0.9f, 0.2f, 0.3f, 0.8f);

    // Determines how long the beat will take to fade out
    private const float FADE_DURATION = 0.5f;
    // Determines the time when the beat will fade into the player's view
    private const float APPEAR_TIME = 0.1f;
    private const float APPEAR_DURATION = 0.05f;
    [SerializeField] private BeatHexagonPopupItem leftHexagon;
    [SerializeField] private BeatHexagonPopupItem rightHexagon;

    private RectTransform _leftTarget;
    private RectTransform _rightTarget;

    private Sequence _sequence;
    private Sequence _appearSequence;
    private Sequence _resolveSequence;

    private BeatSpawner _spawner;
    private int _beat;
    [SerializeField] private BeatHitEffect sampleHitEffect;
    public void Init(BeatSpawner spawner, int beat, RectTransform leftTarget, RectTransform rightTarget, float distance, float travelTime)
    {
        _spawner = spawner;
        _beat = beat;
        _leftTarget = leftTarget;
        _rightTarget = rightTarget;
        // Set the distance of the hexagons first
        Vector2 leftPos = _leftTarget.anchoredPosition;
        leftPos.x -= distance;
        leftHexagon.Rect.anchoredPosition = leftPos;

        Vector2 rightPos = _rightTarget.anchoredPosition;
        rightPos.x += distance;
        rightHexagon.Rect.anchoredPosition = rightPos;

        // Set colour to neutral
        leftHexagon.Image.color = neutralColour;
        rightHexagon.Image.color = neutralColour;

        // Create tweens to move the hexagons
        _sequence = DOTween.Sequence();
        _sequence.Append(leftHexagon.Rect.DOAnchorPos(_leftTarget.anchoredPosition, travelTime).SetEase(Ease.Linear));
        _sequence.Join(rightHexagon.Rect.DOAnchorPos(_rightTarget.anchoredPosition, travelTime).SetEase(Ease.Linear));
        _sequence.Play().OnComplete(Resolve);

        // Create a tween to control when the beat will appear
        float timeToAppear = travelTime - APPEAR_TIME;
        if (timeToAppear > 0f)
        {
            SetImageAlpha(leftHexagon.Image, 0f);
            SetImageAlpha(rightHexagon.Image, 0f);
            _appearSequence = DOTween.Sequence();
            _appearSequence.AppendInterval(timeToAppear);
            _appearSequence.Append(leftHexagon.Image.DOFade(1f, APPEAR_DURATION)).SetEase(Ease.OutCubic);
            _appearSequence.Join(rightHexagon.Image.DOFade(1f, APPEAR_DURATION)).SetEase(Ease.OutCubic);
            _appearSequence.Play();
        }
        else
        {
            SetImageAlpha(leftHexagon.Image, 1f);
            SetImageAlpha(rightHexagon.Image, 1f);
        }
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color colour = image.color;
        colour.a = alpha;
        image.color = colour;
    }

    public void OnTempoChanged(TempoMode _, BeatHandler beatHandler)
    {
        float remainingBeat = _beat - beatHandler.CurrentBeat;
        if (remainingBeat <= 0) return;

        float travelTime = remainingBeat * beatHandler.BeatInterval;

        // If we have a remaining beat, then we want to adjust our time to travel
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(leftHexagon.Rect.DOAnchorPos(_leftTarget.anchoredPosition, travelTime).SetEase(Ease.Linear));
        _sequence.Join(rightHexagon.Rect.DOAnchorPos(_rightTarget.anchoredPosition, travelTime).SetEase(Ease.Linear));
        _sequence.Play().OnComplete(Resolve);
    }

    private void Resolve()
    {
        _appearSequence.Kill();
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(leftHexagon.Image.DOFade(0f, FADE_DURATION).SetEase(Ease.InOutCubic));
        _sequence.Join(rightHexagon.Image.DOFade(0f, FADE_DURATION).SetEase(Ease.InOutCubic));
        _sequence.AppendCallback(() =>
        {
            _spawner.ResolveBeat(_beat);
            Destroy(gameObject);
        });
    }

    public void Resolve(Grade grade)
    {
        _appearSequence.Kill();
        _sequence.Kill();
        _sequence = DOTween.Sequence();

        Color targetColor = Color.white;
        BeatHitEffect hitEffect;
        float fadeStrength = 0f;
        switch (grade)
        {
            // If perfect, the hexagons should pop before fading away
            // Also set the positions right on the center for maximum feel-good
            case Grade.Perfect:
                targetColor = perfectColour;
                leftHexagon.Rect.anchoredPosition = _leftTarget.anchoredPosition;
                rightHexagon.Rect.anchoredPosition = _rightTarget.anchoredPosition;

                _sequence.Append(leftHexagon.Rect.DOScale(1.5f, 0.1f).SetEase(Ease.OutCubic));
                _sequence.Join(rightHexagon.Rect.DOScale(1.5f, 0.1f).SetEase(Ease.OutCubic));
                _sequence.Append(leftHexagon.Rect.DOScale(1f, 0.1f).SetEase(Ease.OutCubic));
                _sequence.Join(rightHexagon.Rect.DOScale(1f, 0.1f).SetEase(Ease.OutCubic));

                hitEffect = Instantiate(sampleHitEffect, leftHexagon.Rect);
                hitEffect.Init(targetColor, FADE_DURATION, 2f);
                hitEffect = Instantiate(sampleHitEffect, rightHexagon.Rect);
                hitEffect.Init(targetColor, FADE_DURATION, 2f);

                break;
            // If good, just set the colour to green
            case Grade.Good:
                targetColor = goodColour;
                hitEffect = Instantiate(sampleHitEffect, leftHexagon.Rect);
                hitEffect.Init(targetColor, FADE_DURATION, 1.2f);
                hitEffect = Instantiate(sampleHitEffect, rightHexagon.Rect);
                hitEffect.Init(targetColor, FADE_DURATION, 1.2f);

                break;
            // If failed, just set the colour to red
            case Grade.Failed:
                targetColor = failedColour;
                fadeStrength = 1f;
                _sequence.Append(leftHexagon.Rect.DOShakeAnchorPos(0.5f, 10f, 20, 90f));
                _sequence.Join(rightHexagon.Rect.DOShakeAnchorPos(0.5f, 10f, 20, 90f));

                break;
        }

        // Set the colour according to the grade
        leftHexagon.Image.color = targetColor;
        rightHexagon.Image.color = targetColor;

        // Fade Out
        _sequence.Append(leftHexagon.Image.DOFade(fadeStrength, FADE_DURATION).SetEase(Ease.InOutCubic));
        _sequence.Join(rightHexagon.Image.DOFade(fadeStrength, FADE_DURATION).SetEase(Ease.InOutCubic));

        _sequence.AppendCallback(() =>
        {
            _spawner.ResolveBeat(_beat);
            Destroy(gameObject);
        });
    }

    void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

}
