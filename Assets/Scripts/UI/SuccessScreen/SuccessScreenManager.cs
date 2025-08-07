using System.Collections;
using Scenes;
using UnityEngine;
using FMODUnity;
using Timeline;

namespace UI
{
    public class SuccessScreenManager : Singleton<SuccessScreenManager>
    {
        public CanvasGroup successScreenCanvasGroup;
        public float fadeDuration = 0.3f; // Duration of the fade-in

        [Header("FMOD")]
        [SerializeField] private EventReference successScreenMusicEvent;
        private IMusicController musicController;

        public bool isSuccess { get; private set; } = false;

        public bool IsSuccess()
        {
            return isSuccess;
        }

        void Start()
        {
            musicController = new FMODMusicController(successScreenMusicEvent);

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
            Time.timeScale = 0f;
            SoundManager.Instance.StopAllSFX();
            MusicTimeline.instance.PauseMusic();
            musicController.PlayMusic();
            yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(successScreenCanvasGroup, 0, 1, fadeDuration));
        }

        public void HideSuccessScreen()
        {
            isSuccess = false;
            successScreenCanvasGroup.gameObject.SetActive(false);
            successScreenCanvasGroup.alpha = 0;
            musicController.StopMusic();
        }
        
        public void SetMusicVolume(float value)
        {
            musicController.SetMusicVolume(value);
        }
    }
}


