using System.Collections;
using Scenes;
using UnityEngine;

namespace UI
{
    public class DefeatScreenManager : Singleton<DefeatScreenManager>
    {
        public SceneFadeManager sceneFadeManager;
        public CanvasGroup defeatScreenCanvasGroup;
        public float fadeDuration = 1.0f; // Duration of the fade-in
        public float delayBeforeFade = 0.5f; // Optional delay before the fade

        void Start()
        {
            Time.timeScale = 1f;
            HideDefeatScreen();
        }

        public void ShowDefeatScreen()
        {
            defeatScreenCanvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeInDefeatScreen());
        }

        private IEnumerator FadeInDefeatScreen()
        {
            // Optional delay before the fade-in starts
            yield return new WaitForSeconds(delayBeforeFade);

            // Start the fade-in and wait for it to complete
            yield return StartCoroutine(sceneFadeManager.FadeCanvasGroup(defeatScreenCanvasGroup, 0, 1, fadeDuration));

            Time.timeScale = 0f;
        }

        public void HideDefeatScreen()
        {
            defeatScreenCanvasGroup.gameObject.SetActive(false);
            defeatScreenCanvasGroup.alpha = 0;
        }
    }
}
