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

    [Header("FMOD")]
    [SerializeField] private EventReference startMenuMusicEvent;
    private IMusicController musicController;

    void Start()
    {
        musicController = new FMODMusicController(startMenuMusicEvent);

        if (isStartMenu)
        {
            PauseManager.PauseGame();
            startMenuCanvasGroup.gameObject.SetActive(true);
            startMenuCanvasGroup.alpha = 1;

            musicController.PlayMusic();
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

        musicController.PlayMusic();

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
        musicController.StopMusic();
        StartCoroutine(FadeOutStartMenu());
    }

    private IEnumerator FadeOutStartMenu()
    {
        yield return SceneFadeManager.Instance.FadeCanvasGroup(startMenuCanvasGroup, 1, 0, fadeDuration);
        PauseManager.ResumeGame();

        EventSystem.current.SetSelectedGameObject(null);
        startMenuCanvasGroup.gameObject.SetActive(false);
    }

    public void SetMusicVolume(float value)
    {
        musicController.SetMusicVolume(value);
    }
}

