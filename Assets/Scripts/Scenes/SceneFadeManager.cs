using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages scene fading and canvas group fading for transitions between scenes.
/// Implements a singleton pattern to ensure a single instance across scenes.
/// </summary>
public class SceneFadeManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of SceneFadeManager.
    /// </summary>
    public static SceneFadeManager Instance { get; private set; }

    [Header("Fade Settings")]
    [Tooltip("The UI Image used for fading between scenes.")]
    public Image fadeImage;

    [Tooltip("The duration of the fade effect in seconds.")]
    public float fadeDuration = 1f;

    /// <summary>
    /// Ensures that only one instance of SceneFadeManager exists and persists between scenes.
    /// </summary>
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make it persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    /// <summary>
    /// Initializes the fade image to be fully black and starts fading in.
    /// </summary>
    private void Start()
    {
        // Ensure the screen starts fully transparent
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1); // Start fully black
            StartCoroutine(FadeIn()); // Fade into the current scene
        }
    }

    /// <summary>
    /// Initiates a fade-out transition and switches to the specified scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene to switch to.</param>
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeOutAndSwitchScene(sceneName));
    }

    /// <summary>
    /// Gradually fades the screen in from black to transparent.
    /// </summary>
    /// <returns>An IEnumerator to control the fade-in animation.</returns>
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

    /// <summary>
    /// Gradually fades the screen out to black and switches scenes.
    /// </summary>
    /// <param name="sceneName">Name of the scene to switch to.</param>
    /// <returns>An IEnumerator to control the fade-out animation and scene transition.</returns>
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

    /// <summary>
    /// Gradually fades a CanvasGroup between start and end alpha values.
    /// </summary>
    /// <param name="canvasGroup">The CanvasGroup to fade.</param>
    /// <param name="startAlpha">The initial alpha value.</param>
    /// <param name="endAlpha">The target alpha value.</param>
    /// <param name="duration">The duration of the fade effect.</param>
    /// <returns>An IEnumerator to control the fade animation.</returns>
    public System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        float epsilon = 0.001f; // Threshold to check if alpha is close to 0 or 1

        // Set the initial alpha value
        canvasGroup.alpha = startAlpha;
        canvasGroup.blocksRaycasts = (Mathf.Abs(startAlpha - 1f) < epsilon);

        while (elapsedTime < duration)
        {
            // Gradually change the alpha value
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);

            // Block or unblock raycasts based on the current alpha value
            if (Mathf.Abs(canvasGroup.alpha - 0f) < epsilon || Mathf.Abs(canvasGroup.alpha - 1f) < epsilon)
            {
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                canvasGroup.blocksRaycasts = false;
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure the final alpha value is set and blocksRaycasts is updated
        canvasGroup.alpha = endAlpha;
        canvasGroup.blocksRaycasts = (Mathf.Abs(endAlpha - 1f) < epsilon);
    }
}

