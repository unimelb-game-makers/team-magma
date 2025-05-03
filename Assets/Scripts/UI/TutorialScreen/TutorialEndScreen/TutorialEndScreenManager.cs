using System.Collections;
using Scenes;
using UnityEngine;

namespace UI
{
    public class TutorialEndScreenManager : Singleton<TutorialEndScreenManager>
    {
        public CanvasGroup tutorialEndScreenCanvasGroup;
        public float fadeDuration = 0.3f; // Duration of the fade-in

        void Start()
        {
            Time.timeScale = 1f;
            HideTutorialEndScreen();
        }

        public void ShowTutorialEndScreen()
        {
            tutorialEndScreenCanvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeInTutorialEndScreen());
        }

        private IEnumerator FadeInTutorialEndScreen()
        {
            // Start the fade-in and wait for it to complete
            yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(tutorialEndScreenCanvasGroup, 0, 1, fadeDuration));

            Time.timeScale = 0f;

            SoundManager.Instance.StopAllSFX();
        }

        public void HideTutorialEndScreen()
        {
            tutorialEndScreenCanvasGroup.gameObject.SetActive(false);
            tutorialEndScreenCanvasGroup.alpha = 0;
        }
    }
}

