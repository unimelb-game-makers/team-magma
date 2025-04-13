using System.Collections;
using Scenes;
using UnityEngine;

namespace UI
{
    public class SuccessScreenManager : Singleton<SuccessScreenManager>
    {
        public CanvasGroup successScreenCanvasGroup;
        public float fadeDuration = 0.3f; // Duration of the fade-in

        void Start()
        {
            Time.timeScale = 1f;
            HideSuccessScreen();
        }

        public void ShowSuccessScreen()
        {
            successScreenCanvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeInSuccessScreen());
        }

        private IEnumerator FadeInSuccessScreen()
        {
            // Start the fade-in and wait for it to complete
            yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(successScreenCanvasGroup, 0, 1, fadeDuration));

            Time.timeScale = 0f;

            SoundManager.Instance.StopAllSFX();
        }

        public void HideSuccessScreen()
        {
            successScreenCanvasGroup.gameObject.SetActive(false);
            successScreenCanvasGroup.alpha = 0;
        }
    }
}

