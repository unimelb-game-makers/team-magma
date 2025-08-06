using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Scenes;
using FMODUnity;

public class StartMenuManager : Singleton<StartMenuManager>
{
    public CanvasGroup startMenuCanvasGroup;
    public float fadeDuration = 0.5f;
    public bool isStartMenu = true;
    private float _volume = 1.0f;

    [Header("FMOD")]
    [SerializeField] private EventReference startMenuMusicEvent;
    private FMOD.Studio.EventInstance startMenuMusicInstance;

    void Start()
    {
        if (isStartMenu)
        {
            PauseManager.PauseGame();
            startMenuCanvasGroup.gameObject.SetActive(true);
            startMenuCanvasGroup.alpha = 1;

            PlayStartMenuMusic();
        }
        else
        {
            startMenuCanvasGroup.alpha = 0;
            startMenuCanvasGroup.gameObject.SetActive(false);
        }
    }

    public void OpenStartMenu()
    {
        isStartMenu = true;
        PauseManager.PauseGame();
        startMenuCanvasGroup.gameObject.SetActive(true);

        PlayStartMenuMusic();

        StartCoroutine(FadeInStartMenu());
    }

    private IEnumerator FadeInStartMenu()
    {
        yield return SceneFadeManager.Instance.FadeCanvasGroup(startMenuCanvasGroup, 0, 1, fadeDuration);

        PauseMenuController.Instance.HideUI();
        PauseMenuController.Instance.isPauseMenu = false;
    }

    public void HideStartMenu()
    {
        isStartMenu = false;
        StopStartMenuMusic();
        StartCoroutine(FadeOutStartMenu());
    }

    private IEnumerator FadeOutStartMenu()
    {
        yield return SceneFadeManager.Instance.FadeCanvasGroup(startMenuCanvasGroup, 1, 0, fadeDuration);
        PauseManager.ResumeGame();

        EventSystem.current.SetSelectedGameObject(null);
        startMenuCanvasGroup.gameObject.SetActive(false);
    }

    private void PlayStartMenuMusic()
    {
        if (!startMenuMusicInstance.isValid())
        {
            startMenuMusicInstance = RuntimeManager.CreateInstance(startMenuMusicEvent);
            startMenuMusicInstance.start();
        }
    }

    private void StopStartMenuMusic()
    {
        if (startMenuMusicInstance.isValid())
        {
            startMenuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            startMenuMusicInstance.release();
        }
    }

    public void SetMusicVolume(float volume)
    {
        _volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        startMenuMusicInstance.setVolume(_volume);
    }
}

