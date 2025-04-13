using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

[RequireComponent(typeof(Image))]
public class UIButtonSFXAndImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // UI Image Transition
    public Sprite normalSprite;
    public Sprite hoverSprite;
    public float transitionDuration = 0.5f;

    private Image image;
    private Coroutine transitionCoroutine;

    // FMOD Sound
    [SerializeField] private EventReference hoverEventReference;
    [SerializeField] private EventReference clickEventReference;

    private EventInstance hoverEventInstance;
    private EventInstance clickEventInstance;

    void Awake()
    {
        image = GetComponent<Image>();
        if (normalSprite != null)
            image.sprite = normalSprite;

        // Ensure image is fully visible at start
        Color c = image.color;
        c.a = 1f;
        image.color = c;
    }

    void Start()
    {
        // FMOD setup
        hoverEventInstance = RuntimeManager.CreateInstance(hoverEventReference);
        clickEventInstance = RuntimeManager.CreateInstance(clickEventReference);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartTransition(hoverSprite);
        SetVolumeBasedOnSetting();
        hoverEventInstance.start();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartTransition(normalSprite);
        hoverEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetVolumeBasedOnSetting();
        clickEventInstance.start();
    }

    private void StartTransition(Sprite targetSprite)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionToSprite(targetSprite));
    }

    private IEnumerator TransitionToSprite(Sprite targetSprite)
    {
        float halfDuration = transitionDuration / 2f;

        // Fade out
        for (float t = 0; t < halfDuration; t += Time.unscaledDeltaTime)
        {
            float normalizedTime = t / halfDuration;
            Color c = image.color;
            c.a = Mathf.Lerp(1f, 0f, normalizedTime);
            image.color = c;
            yield return null;
        }

        // Set the sprite
        image.sprite = targetSprite;

        // Fade in
        for (float t = 0; t < halfDuration; t += Time.unscaledDeltaTime)
        {
            float normalizedTime = t / halfDuration;
            Color c = image.color;
            c.a = Mathf.Lerp(0f, 1f, normalizedTime);
            image.color = c;
            yield return null;
        }

        // Ensure it's fully visible
        Color finalColor = image.color;
        finalColor.a = 1f;
        image.color = finalColor;
    }

    private void SetVolumeBasedOnSetting()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        SetSFXVolume(sfxVolume * masterVolume);
    }

    private void SetSFXVolume(float value)
    {
        hoverEventInstance.setVolume(value);
        clickEventInstance.setVolume(value);
    }

    private void OnDestroy()
    {
        hoverEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        hoverEventInstance.release();
        clickEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        clickEventInstance.release();
    }
}


