using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ButtonHoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite normalSprite;
    public Sprite hoverSprite;
    public float transitionDuration = 0.5f;

    private Image image;
    private Coroutine transitionCoroutine;

    void Awake()
    {
        image = GetComponent<Image>();
        if (normalSprite != null)
            image.sprite = normalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartTransition(hoverSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartTransition(normalSprite);
    }

    private void StartTransition(Sprite targetSprite)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionToSprite(targetSprite));
    }

    private IEnumerator TransitionToSprite(Sprite targetSprite)
    {
        float time = 0f;
        Sprite startSprite = image.sprite;

        // Create temporary GameObjects for crossfade if needed
        if (startSprite != targetSprite)
        {
            // Optionally crossfade by alpha (if using multiple image layers)
            image.CrossFadeAlpha(0f, transitionDuration / 2f, false);
            yield return new WaitForSeconds(transitionDuration / 2f);
            image.sprite = targetSprite;
            image.CrossFadeAlpha(1f, transitionDuration / 2f, false);
        }
    }
}
