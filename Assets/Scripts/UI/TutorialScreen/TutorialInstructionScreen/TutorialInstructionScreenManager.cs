using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Unity.VisualStudio.Editor;
using Scenes;
using Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public enum TutorialScreenType
    {
        Move,
        Jump,
        SlowTape,
        Temp
    }

    public enum ButtonToClose
    {
        Enter,
        A
    }

    public class TutorialInstructionScreenManager : MonoBehaviour
    {
        private int currentPage;
        [SerializeField] private CanvasGroup[] moveCanvasGroup;
        [SerializeField] private CanvasGroup[] jumpCanvasGroup;
        [SerializeField] private CanvasGroup[] slowTapeCanvasGroup;
        [SerializeField] private float fadeDuration = 0.3f;

        void Start()
        {
            Time.timeScale = 1f;
            HideAllInstructionScreens();
            ResetCurrentPage();
        }

        private void ResetCurrentPage()
        {
            currentPage = -1;
        }

        public void ShowMoveScreen()
        {
            ShowNextScreen(moveCanvasGroup);
        }

        public void ShowJumpScreen()
        {
            ShowNextScreen(jumpCanvasGroup);
        }

        public void ShowSlowTapeScreen()
        {
            ShowNextScreen(slowTapeCanvasGroup);
        }

        private void ShowNextScreen(CanvasGroup[] canvasGroups)
        {
            if (currentPage != -1) HideScreen(canvasGroups[currentPage]);
            currentPage++;

            // If no more move pages, return
            if (currentPage == canvasGroups.Length) 
            {
                PauseManager.ResumeGame();
                ResetCurrentPage();
                return;
            }
            else if (currentPage == 0)
            {
                // Do not pause music
                PauseManager.PauseGame(false);
            }

            print(currentPage);

            StartCoroutine(FadeInScreen(canvasGroups[currentPage]));
        }

        private IEnumerator FadeInScreen(CanvasGroup canvasGroup)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 1;

            // Start the fade-in and wait for it to complete
            // yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(canvasGroup, 0, 1, fadeDuration));
            yield return null;

            // Then fade in children
        }

        private void HideScreen(CanvasGroup canvasGroup)
        {
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.alpha = 0;
        }

        // FadeInImages and HideImages

        // Set all children of a canvas group to inactive first.
        // Then set the canvas to inactive.
        // The next time the canvas group is set to active, its children will remain inactive.
        // Then children can be selected to be set active and fade in.
        private void HideAllInstructionScreens()
        {
            foreach (CanvasGroup canvasGroup in moveCanvasGroup)
            {
                // Hide the children (i.e. images) first
                HideScreen(canvasGroup);
            }
            foreach (CanvasGroup canvasGroup in jumpCanvasGroup)
            {
                HideScreen(canvasGroup);
            }
            foreach (CanvasGroup canvasGroup in slowTapeCanvasGroup)
            {
                HideScreen(canvasGroup);
            }
        }
    }
}