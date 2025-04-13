using System.Collections;
using Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class DefeatScreenManager : Singleton<DefeatScreenManager>
    {
        public CanvasGroup defeatScreenCanvasGroup;
        public float fadeDuration = 1.0f; // Duration of the fade-in
        public float delayBeforeFade = 0.5f; // Optional delay before the fade
        private bool isDefeatScreen = false;

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the event
        }

        void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent memory leaks
        }

        // Runs once when a scene is loaded.
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Time.timeScale = 1f;
            HideDefeatScreen();
            isDefeatScreen = false;
        }

        public bool IsDefeat()
        {
            return isDefeatScreen;
        }

        public void ShowDefeatScreen()
        {
            if (!isDefeatScreen)
            {
                isDefeatScreen = true;
                defeatScreenCanvasGroup.gameObject.SetActive(true);
                StartCoroutine(FadeInDefeatScreen());
            }
        }

        private IEnumerator FadeInDefeatScreen()
        {
            Time.timeScale = 0;

            // Optional delay before the fade-in starts
            yield return new WaitForSecondsRealtime(delayBeforeFade);

            // Stop all SFX in the scene.
            if (SoundManager.Instance) SoundManager.Instance.StopAllSFX();

            // Start the fade-in and wait for it to complete
            yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(defeatScreenCanvasGroup, 0, 1, fadeDuration));
        }

        public void HideDefeatScreen()
        {
            defeatScreenCanvasGroup.gameObject.SetActive(false);
            defeatScreenCanvasGroup.alpha = 0;
            isDefeatScreen = false;
        }
    }
}
