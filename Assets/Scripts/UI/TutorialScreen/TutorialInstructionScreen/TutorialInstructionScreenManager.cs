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

        [SerializeField] private float fadeDuration = 0.3f;

        void Start()
        {
            Time.timeScale = 1f;
            HideAllInstructionScreens();
        }

        public void ShowMoveScreen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(moveCanvasGroup));
        }
        public void ShowSmallMoveScreen() {
            HideScreen(moveCanvasGroup);
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallMoveCanvasGroup));
        }
        public void HideSmallMoveScreen() {
            HideScreen(smallMoveCanvasGroup);
        }

        public void ShowJumpScreen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(jumpCanvasGroup));
        }
        public void ShowSmallJumpScreen() {
            HideScreen(jumpCanvasGroup);
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallJumpCanvasGroup));
        }
        public void HideSmallJumpScreen() {
            HideScreen(smallJumpCanvasGroup);
        }

        public void ShowSlowTapeScreen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(slowTapeCanvasGroup));
        }
        public void ShowSmallSlowTapeScreen() {
            HideScreen(slowTapeCanvasGroup);
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallSlowTapeCanvasGroup));
        }
        public void HideSmallSlowTapeScreen() {
            HideScreen(smallSlowTapeCanvasGroup);
        }

        public void ShowBasicAttack1Screen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(basicAttack1CanvasGroup));
        }
        public void ShowBasicAttack2Screen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(basicAttack2CanvasGroup));
        }
        public void ShowBasicAttack3Screen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(basicAttack3CanvasGroup));
        }
        public void ShowSmallBasicAttack1Screen() {
            HideScreen(basicAttack1CanvasGroup);
            HideScreen(basicAttack2CanvasGroup);
            HideScreen(basicAttack3CanvasGroup);
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallBasicAttack1CanvasGroup));
        }
        public void HideSmallBasicAttack1Screen() {
            HideScreen(smallBasicAttack1CanvasGroup);
        }

        public void ShowRangedAttackScreen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(rangedAttackCanvasGroup));
        }
        public void ShowSmallRangedAttackScreen() {
            HideScreen(rangedAttackCanvasGroup);
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallRangedAttackCanvasGroup));
        }
        public void HideSmallRangedAttackScreen() {
            HideScreen(smallRangedAttackCanvasGroup);
        }

        public void ShowShieldAttackScreen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(shieldAttackCanvasGroup));
        }
        public void ShowSmallShieldAttackScreen() {
            HideScreen(shieldAttackCanvasGroup);
            PauseManager.ResumeGame();
            StartCoroutine(FadeInScreen(smallShieldAttackCanvasGroup));
        }
        public void HideSmallShieldAttackScreen() {
            HideScreen(smallShieldAttackCanvasGroup);
        }

        public void ShowHPScreen()
        {
            // Pause the game, but do not pause music
            PauseManager.PauseGame(false);
            StartCoroutine(FadeInScreen(HPCanvasGroup));
        }
        public void HideHPScreen() {
            HideScreen(HPCanvasGroup);
            PauseManager.ResumeGame();
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
        }
    }
}