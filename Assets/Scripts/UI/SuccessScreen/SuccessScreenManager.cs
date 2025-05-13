using System.Collections;
using Scenes;
using UnityEngine;

namespace UI
{
    public class SuccessScreenManager : Singleton<SuccessScreenManager>
    {
        public CanvasGroup successScreenCanvasGroup;
        public float fadeDuration = 0.3f; // Duration of the fade-in

        public bool isSuccess { get; private set; } = false;

        public bool IsSuccess()
        {
            return isSuccess;
        }

        void Start()
        {
            Time.timeScale = 1f;
            HideSuccessScreen();
        }

        public void ShowSuccessScreen()
        {
            isSuccess = true;
            successScreenCanvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeInSuccessScreen());
        }

        private IEnumerator FadeInSuccessScreen()
        {
            yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(successScreenCanvasGroup, 0, 1, fadeDuration));
            Time.timeScale = 0f;
            SoundManager.Instance.StopAllSFX();
        }

        public void HideSuccessScreen()
        {
            isSuccess = false;
            successScreenCanvasGroup.gameObject.SetActive(false);
            successScreenCanvasGroup.alpha = 0;
        }
    }
}


