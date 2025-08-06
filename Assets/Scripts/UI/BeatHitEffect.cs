using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BeatHitEffect : MonoBehaviour
{
    private Sequence _sequence;
    private Image image;
    public void Init(Color color, float duration, float scale)
    {
        image = GetComponent<Image>();
        image.color = color;
        _sequence.Append(image.DOFade(0f, duration).SetEase(Ease.InOutCubic));
        _sequence.Join(image.transform.DOScale(scale, duration).SetEase(Ease.InOutCubic));

        _sequence.AppendCallback(() =>
        {
            Destroy(gameObject);
        });

    }

}
