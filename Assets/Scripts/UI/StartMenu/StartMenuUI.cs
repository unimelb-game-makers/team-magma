using System;
using Scenes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class StartMenuUI : Singleton<StartMenuUI>
    {
        [SerializeField] private GameObject tutorialButton;
        [SerializeField] private GameObject continueButton;
        [SerializeField] private GameObject newGameButton;
        [SerializeField] private GameObject settingsButton;
        [SerializeField] private GameObject creditButton;
        [SerializeField] private GameObject quitButton;

        [SerializeField] private readonly string tutorialSceneName = "Scenes/TutorialLevel";
        
        //bind the buttons to the functions
        private void Start()
        {
            tutorialButton.GetComponent<Button>().onClick.AddListener(OnTutorialButtonClicked);
            continueButton.GetComponent<Button>().onClick.AddListener(OnContinueButtonClicked);
            newGameButton.GetComponent<Button>().onClick.AddListener(OnNewGameButtonClicked);
            creditButton.GetComponent<Button>().onClick.AddListener(OnCreditButtonClicked);
            settingsButton.GetComponent<Button>().onClick.AddListener(OnSettingsButtonClicked);
            quitButton.GetComponent<Button>().onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnTutorialButtonClicked()
        {
            GameManager.Instance.LoadTutorial(tutorialSceneName);
            SceneFadeManager.Instance.StartCoroutine(SceneFadeManager.Instance.FadeIn());
            StartMenuManager.Instance.HideStartMenu();
            PauseManager.ResumeGame();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void OnContinueButtonClicked()
        {
            GameManager.Instance.LoadContinueGame();
            SceneFadeManager.Instance.StartCoroutine(SceneFadeManager.Instance.FadeIn());
            StartMenuManager.Instance.HideStartMenu();
            PauseManager.ResumeGame();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void OnNewGameButtonClicked()
        {
            GameManager.Instance.LoadNewGame();
            SceneFadeManager.Instance.StartCoroutine(SceneFadeManager.Instance.FadeIn());
            StartMenuManager.Instance.HideStartMenu();
            PauseManager.ResumeGame();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void OnCreditButtonClicked()
        {
            CreditPageManager.Instance.ShowFirstCreditPage();
        }
        private void OnSettingsButtonClicked()
        {
            SettingsManager.Instance.OpenSettings();

            // Deselect the currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void OnQuitButtonClicked()
        {
            // Quit the game
            Debug.Log("Quit button clicked");
            Application.Quit();
        }
    }
}
