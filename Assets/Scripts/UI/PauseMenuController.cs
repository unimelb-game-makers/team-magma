using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public CanvasGroup pauseMenuCanvasGroup;
    public float fadeDuration = 0.5f; // Duration for the fade effect
    public PauseObjectController pauseObjectController;
    public bool isPauseMenu = false;

    void Update()
    {
        // Toggle the pause menu when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
            isPauseMenu = !isPauseMenu;
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
            Time.timeScale = 0;
            StartCoroutine(FadeCanvasGroup(pauseMenuCanvasGroup, 0, 1, fadeDuration));
            pauseObjectController.DisableObjects();
        }
        else
        {
            // Unpause the game and fade out the menu
            StartCoroutine(FadeOutAndUnpause());
        }
    }

    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore Time.timeScale
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // Ensure the final alpha value is set
    }

    private System.Collections.IEnumerator FadeOutAndUnpause()
    {
        yield return FadeCanvasGroup(pauseMenuCanvasGroup, 1, 0, fadeDuration);
        Time.timeScale = 1; // Unpause the game after fading out
        pauseObjectController.EnableObjects();
    }
}

