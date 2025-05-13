using System.Collections;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using Microsoft.Unity.VisualStudio.Editor;
#endif

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
        BasicAttack1,
        BasicAttack2,
        BasicAttack3,
        RangedAttack,
        ShieldAttack,
        HP
    }

    public class TutorialInstructionScreenManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup moveCanvasGroup;
        [SerializeField] private CanvasGroup smallMoveCanvasGroup;

        [SerializeField] private CanvasGroup jumpCanvasGroup;
        [SerializeField] private CanvasGroup smallJumpCanvasGroup;

        [SerializeField] private CanvasGroup slowTapeCanvasGroup;
        [SerializeField] private CanvasGroup smallSlowTapeCanvasGroup;

        [SerializeField] private CanvasGroup basicAttack1CanvasGroup;
        [SerializeField] private CanvasGroup basicAttack2CanvasGroup;
        [SerializeField] private CanvasGroup basicAttack3CanvasGroup;
        [SerializeField] private CanvasGroup smallBasicAttack1CanvasGroup;

        [SerializeField] private CanvasGroup rangedAttackCanvasGroup;
        [SerializeField] private CanvasGroup smallRangedAttackCanvasGroup;

        [SerializeField] private CanvasGroup shieldAttackCanvasGroup;
        [SerializeField] private CanvasGroup smallShieldAttackCanvasGroup;

        [SerializeField] private CanvasGroup HPCanvasGroup;

        [SerializeField] private float screenFadeDuration = 0.5f;

        [SerializeField] private float textTimeToAppear = 1f;
        [SerializeField] private float textFadeDuration = 1.5f;

        public float GetTimeToAppear() {
            return textTimeToAppear;
        }

        public float GetTextFadeDuration() {
            return textFadeDuration;
        }

        void Start()
        {
            Time.timeScale = 1f;
            HideAllInstructionScreens();
        }

        public void ShowMoveScreen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(moveCanvasGroup));
        }
        public void ShowSmallMoveScreen() {
            StartCoroutine(FadeOutScreen(moveCanvasGroup));
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallMoveCanvasGroup));
        }
        public void HideSmallMoveScreen() {
            StartCoroutine(FadeOutScreen(smallMoveCanvasGroup));
        }

        public void ShowJumpScreen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(jumpCanvasGroup));
        }
        public void ShowSmallJumpScreen() {
            StartCoroutine(FadeOutScreen(jumpCanvasGroup));
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallJumpCanvasGroup));
        }
        public void HideSmallJumpScreen() {
            StartCoroutine(FadeOutScreen(smallJumpCanvasGroup));
        }

        public void ShowSlowTapeScreen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(slowTapeCanvasGroup));
        }
        public void ShowSmallSlowTapeScreen() {
            StartCoroutine(FadeOutScreen(slowTapeCanvasGroup));
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallSlowTapeCanvasGroup));
        }
        public void HideSmallSlowTapeScreen() {
            StartCoroutine(FadeOutScreen(smallSlowTapeCanvasGroup));
        }

        public void ShowBasicAttack1Screen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(basicAttack1CanvasGroup));
        }
        public void ShowBasicAttack2Screen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(basicAttack2CanvasGroup));
        }
        public void ShowBasicAttack3Screen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(basicAttack3CanvasGroup));
        }
        public void ShowSmallBasicAttack1Screen() {
            StartCoroutine(FadeOutScreen(basicAttack1CanvasGroup));
            StartCoroutine(FadeOutScreen(basicAttack2CanvasGroup));
            StartCoroutine(FadeOutScreen(basicAttack3CanvasGroup));
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallBasicAttack1CanvasGroup));
        }
        public void HideSmallBasicAttack1Screen() {
            StartCoroutine(FadeOutScreen(smallBasicAttack1CanvasGroup));
        }

        public void ShowRangedAttackScreen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(rangedAttackCanvasGroup));
        }
        public void ShowSmallRangedAttackScreen() {
            StartCoroutine(FadeOutScreen(rangedAttackCanvasGroup));
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallRangedAttackCanvasGroup));
        }
        public void HideSmallRangedAttackScreen() {
            StartCoroutine(FadeOutScreen(smallRangedAttackCanvasGroup));
        }

        public void ShowShieldAttackScreen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(shieldAttackCanvasGroup));
        }
        public void ShowSmallShieldAttackScreen() {
            StartCoroutine(FadeOutScreen(shieldAttackCanvasGroup));
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallShieldAttackCanvasGroup));
        }
        public void HideSmallShieldAttackScreen() {
            StartCoroutine(FadeOutScreen(smallShieldAttackCanvasGroup));
        }

        public void ShowHPScreen()
        {
            HideAllInstructionScreens();
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(HPCanvasGroup));
        }
        public void HideHPScreen() {
            StartCoroutine(FadeOutScreen(HPCanvasGroup));
            PauseManager.ResumeGame();
        }

        private IEnumerator FadeInScreen(CanvasGroup canvasGroup)
        {
            canvasGroup.gameObject.SetActive(true);
            // Start the fade-in and wait for it to complete
            yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(canvasGroup, 0, 1, screenFadeDuration));
        }

        private IEnumerator FadeOutScreen(CanvasGroup canvasGroup)
        {
            // Start the fade-out and wait for it to complete
            yield return StartCoroutine(SceneFadeManager.Instance.FadeCanvasGroup(canvasGroup, 1, 0, screenFadeDuration));
            canvasGroup.gameObject.SetActive(false);
        }

        private void HideScreen(CanvasGroup canvasGroup)
        {
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.alpha = 0;
        }
        private void HideAllInstructionScreens()
        {
            HideScreen(moveCanvasGroup);
            HideScreen(smallMoveCanvasGroup);

            HideScreen(jumpCanvasGroup);
            HideScreen(smallJumpCanvasGroup);

            HideScreen(slowTapeCanvasGroup);
            HideScreen(smallSlowTapeCanvasGroup);

            HideScreen(basicAttack1CanvasGroup);
            HideScreen(basicAttack2CanvasGroup);
            HideScreen(basicAttack3CanvasGroup);
            HideScreen(smallBasicAttack1CanvasGroup);

            HideScreen(rangedAttackCanvasGroup);
            HideScreen(smallRangedAttackCanvasGroup);

            HideScreen(shieldAttackCanvasGroup);
            HideScreen(smallShieldAttackCanvasGroup);

            HideScreen(HPCanvasGroup);
        }
    }
}