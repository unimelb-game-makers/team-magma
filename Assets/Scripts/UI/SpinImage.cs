using UnityEngine;
using DG.Tweening;

public class SpinImage : MonoBehaviour
{
    public float duration = 2f; // Time to complete one full spin
    private Tween rotationTween;

    void OnEnable()
    {
        // Start infinite rotation
        rotationTween = transform.DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetLink(gameObject); // Ensures tween is killed with GameObject
    }

    void OnDisable()
    {
        // Stop rotation if it's active
        if (rotationTween != null && rotationTween.IsActive())
        {
            rotationTween.Kill();
            rotationTween = null;
        }
    }
}

