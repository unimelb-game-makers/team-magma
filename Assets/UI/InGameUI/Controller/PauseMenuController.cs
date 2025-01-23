using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public CanvasGroup pauseMenuCanvasGroup;
    public float fadeDuration = 0.5f; // Duration for the fade effect

    public string[] tagsToDisable = { "Player", "Enemy" };

    void Update()
    {
        // Toggle the pause menu when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        PauseManager.TogglePause();

        if (PauseManager.IsPaused)
        {
            // Pause the game and fade in the menu
            Time.timeScale = 0;
            StartCoroutine(FadeCanvasGroup(pauseMenuCanvasGroup, 0, 1, fadeDuration));
            DisableObjects();
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
        EnableObjects();
    }

    private void DisableObjects()
    {
        // Loop through each tag to disable relevant GameObjects
        foreach (string tag in tagsToDisable)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    script.enabled = false; // Disable all scripts on this GameObject
                }
            }
        }
    }

    private void EnableObjects()
    {
        // Loop through each tag to enable relevant GameObjects
        foreach (string tag in tagsToDisable)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    script.enabled = true; // Enable all scripts on this GameObject
                }
            }
        }
    }
}

