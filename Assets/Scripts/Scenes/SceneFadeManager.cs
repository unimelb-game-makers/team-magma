using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFadeManager : MonoBehaviour
{
    public Image fadeImage; // Assign the fade image in the Inspector
    public float fadeDuration = 1f; // Duration of the fade effect

    private void Start()
    {
        // Ensure the screen starts fully transparent
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1); // Start fully black
            StartCoroutine(FadeIn()); // Fade into the current scene
        }
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeOutAndSwitchScene(sceneName));
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = 1f - (elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha); // Gradually reduce alpha
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0); // Fully transparent
    }

    private System.Collections.IEnumerator FadeOutAndSwitchScene(string sceneName)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = elapsedTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha); // Gradually increase alpha
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1); // Fully black

        // Switch to the new scene
        SceneManager.LoadScene(sceneName);
    }

    public System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        float epsilon = 0.001f; // Threshold to check if alpha is close to 0 or 1

        // Set the initial alpha value
        canvasGroup.alpha = startAlpha;
        // Set blocksRaycasts based on the initial alpha value
        canvasGroup.blocksRaycasts = (Mathf.Abs(startAlpha - 1f) < epsilon);

        while (elapsedTime < duration)
        {
            // Gradually change the alpha value
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            
            // Block or unblock raycasts based on the current alpha value
            if (Mathf.Abs(canvasGroup.alpha - 0f) < epsilon || Mathf.Abs(canvasGroup.alpha - 1f) < epsilon)
            {
                canvasGroup.blocksRaycasts = true; // Enable raycast blocking when alpha is close to 0 or 1
            }
            else
            {
                canvasGroup.blocksRaycasts = false; // Disable raycast blocking when alpha is between 0 and 1
            }

            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore Time.timeScale
            yield return null;
        }

        // Ensure the final alpha value is set and blocksRaycasts is updated
        canvasGroup.alpha = endAlpha;
        canvasGroup.blocksRaycasts = (Mathf.Abs(endAlpha - 1f) < epsilon); // Set blocksRaycasts based on the final alpha value
    }
}
