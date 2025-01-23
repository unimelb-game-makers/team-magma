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
}
