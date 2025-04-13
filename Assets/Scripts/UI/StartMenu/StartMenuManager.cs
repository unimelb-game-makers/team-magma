using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Scenes;

public class StartMenuManager : Singleton<StartMenuManager>
{
    public CanvasGroup startMenuCanvasGroup;
    public float fadeDuration = 0.5f; // Duration for the fade effect
    public bool isStartMenu = true;

    // Start is called before the first frame update
    void Start()
    {
        if (isStartMenu)
        {
            PauseManager.PauseGame();
            startMenuCanvasGroup.gameObject.SetActive(true);
            startMenuCanvasGroup.alpha = 1;
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

        StartCoroutine(FadeInStartMenu());
    }

    private System.Collections.IEnumerator FadeInStartMenu()
    {
        yield return SceneFadeManager.Instance.FadeCanvasGroup(startMenuCanvasGroup, 0, 1, fadeDuration);

        // Hide the pause menu ui
        PauseMenuController.Instance.HideUI();
        PauseMenuController.Instance.isPauseMenu = false;
    }

    public void HideStartMenu()
    {
        isStartMenu = false;
        StartCoroutine(FadeOutStartMenu());
    }

    private System.Collections.IEnumerator FadeOutStartMenu()
    {
        yield return SceneFadeManager.Instance.FadeCanvasGroup(startMenuCanvasGroup, 1, 0, fadeDuration);
        PauseManager.ResumeGame();

        // Deselect the currently selected UI element
        EventSystem.current.SetSelectedGameObject(null);

        startMenuCanvasGroup.gameObject.SetActive(false);
    }
}
