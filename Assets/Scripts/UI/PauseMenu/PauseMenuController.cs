using Scenes;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuController : Singleton<PauseMenuController>
{
    public CanvasGroup pauseMenuCanvasGroup;
    public float fadeDuration = 0.5f; // Duration for the fade effect
    public bool isPauseMenu = false;
    private float inputCooldown = 0.1f;  // Cooldown period (seconds)
    private float lastResumeTime;

    void Start()
    {
        HideUI();
    }

    void Update()
    {
        // Check if enough time has passed since resuming
        if (Time.time - lastResumeTime < inputCooldown)
        {
            return;
        }

        // Toggle the pause menu when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (!(PauseManager.IsPaused && !isPauseMenu))
        {
            // if it is paused because of things other than pause menu, remain paused
            PauseManager.TogglePause();
        }

        if (PauseManager.IsPaused)
        {
            // Pause the game and fade in the menu
            ShowUI();
            StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(pauseMenuCanvasGroup, 0, 1, fadeDuration));
            PauseManager.PauseGame();
        }
        else
        {
            // Unpause the game and fade out the menu
            StartCoroutine(FadeOutAndUnpause());
        }
        isPauseMenu = !isPauseMenu;
    }

    private System.Collections.IEnumerator FadeOutAndUnpause()
    {
        yield return SceneFadeManager.Instance.FadeCanvasGroup(pauseMenuCanvasGroup, 1, 0, fadeDuration);
        PauseManager.ResumeGame();

        // Deselect the currently selected UI element
        EventSystem.current.SetSelectedGameObject(null);

        HideUI();
    }

    // These are to toggle the pause canvas, otherwise it blocks UI raycasts on other UI canvas elements.
    public void ShowUI()
    {
        pauseMenuCanvasGroup.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        pauseMenuCanvasGroup.gameObject.SetActive(false);
    }

    public void ToggleUI()
    {
        pauseMenuCanvasGroup.gameObject.SetActive(!pauseMenuCanvasGroup.gameObject.activeSelf);
    }
}
