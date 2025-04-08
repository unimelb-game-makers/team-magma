// Author : Peiyu Wang @ Daphatus
// 08 04 2025 04 45

using System;
using Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DefeatMenuUI : Singleton<DefeatMenuUI>
    {
        [SerializeField] private GameObject continueButton;
        [SerializeField] private GameObject settingsButton;
        [SerializeField] private GameObject quitButton;
        
        //bind the buttons to the functions
        private void Start()
        {
            continueButton.GetComponent<Button>().onClick.AddListener(OnContinueButtonClicked);
            settingsButton.GetComponent<Button>().onClick.AddListener(OnSettingsButtonClicked);
            quitButton.GetComponent<Button>().onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnContinueButtonClicked()
        {
            GameManager.Instance.ReloadLevel();
            SceneFadeManager.Instance.StartCoroutine(SceneFadeManager.Instance.FadeIn());
            PauseManager.ResumeGame();
        }
        private void OnSettingsButtonClicked()
        {
            SettingsManager.Instance.OpenSettings();
        }
        private void OnQuitButtonClicked()
        {
            // Quit the game
            Debug.Log("Quit button clicked");
            Application.Quit();
        }
    }
}