using System.Collections;
using UnityEngine;

public class DefeatScreenManager : MonoBehaviour
{
    public SceneFadeManager sceneFadeManager;
    public CanvasGroup defeatScreenCanvasGroup;
    public float fadeDuration = 1.0f; // Duration of the fade-in
    public float delayBeforeFade = 0.5f; // Optional delay before the fade

    public void ShowDefeatScreen()
    {
        StartCoroutine(FadeInDefeatScreen());
    }

    private IEnumerator FadeInDefeatScreen()
    {
        // Optional delay before the fade-in starts
        yield return new WaitForSeconds(delayBeforeFade);

        StartCoroutine(sceneFadeManager.FadeCanvasGroup(defeatScreenCanvasGroup, 0, 1, fadeDuration));
        defeatScreenCanvasGroup.gameObject.SetActive(true);
    }
}
